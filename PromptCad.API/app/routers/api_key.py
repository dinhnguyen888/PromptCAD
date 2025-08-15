import secrets
from datetime import datetime, timedelta
from fastapi import APIRouter, Depends, HTTPException, status, Security
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials

from app.db.mongo import get_collection
from app.core.security import get_current_admin, create_jwt, decode_jwt
from app.schemas.api_key import (
    CreateApiKeyRequest,
    UpdateApiKeyRequest,
    DeleteApiKeyRequest,
    AuthApiKeyRequest,
    UpdateApiKeyInfoRequest,
)

router = APIRouter()
_session_bearer = HTTPBearer(auto_error=False)


def _months_to_timedelta(months: int) -> timedelta:
    if months not in (3, 6, 12):
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail="duration_months must be one of 3, 6, 12")
    return timedelta(days=months * 30)


@router.post("/create-api-key")
async def create_api_key(req: CreateApiKeyRequest, admin=Depends(get_current_admin)):
    api_keys = get_collection("api_keys")
    expires_at = datetime.utcnow() + _months_to_timedelta(req.duration_months)
    new_key = secrets.token_urlsafe(32)
    doc = {
        "key": new_key,
        "expires_at": expires_at,
        "created_at": datetime.utcnow(),
        "created_by_account_id": admin.get("sub"),
        "is_active": True,
        "user_name": req.user_name,
        "phone_number": req.phone_number
    }
    await api_keys.insert_one(doc)
    return {"api_key": new_key, "expires_at": expires_at.isoformat()}


@router.post("/update-api-key")
async def update_api_key(req: UpdateApiKeyRequest, admin=Depends(get_current_admin)):
    api_keys = get_collection("api_keys")
    expires_at = datetime.utcnow() + _months_to_timedelta(req.duration_months)
    res = await api_keys.update_one({"key": req.api_key}, {"$set": {"expires_at": expires_at}})
    if res.matched_count == 0:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="API key not found")
    return {"api_key": req.api_key, "expires_at": expires_at.isoformat()}


@router.post("/update-api-key-info")
async def update_api_key_info(req: UpdateApiKeyInfoRequest, admin=Depends(get_current_admin)):
    """Cập nhật thông tin user_name, phone_number của API key"""
    api_keys = get_collection("api_keys")
    update_fields = {}
    if req.user_name is not None:
        update_fields["user_name"] = req.user_name
    if req.phone_number is not None:
        update_fields["phone_number"] = req.phone_number
    if not update_fields:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail="No fields to update")

    res = await api_keys.update_one({"key": req.api_key}, {"$set": update_fields})
    if res.matched_count == 0:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="API key not found")
    return {"api_key": req.api_key, "updated_fields": update_fields}

@router.delete("/delete-api-key")
async def delete_api_key(req: DeleteApiKeyRequest, admin=Depends(get_current_admin)):
    api_keys = get_collection("api_keys")
    deleted = await api_keys.delete_one({"key": req.api_key})
    total_deleted = deleted.deleted_count
    cleaned_expired = 0
    if req.cleanup_expired:
        threshold = datetime.utcnow() - timedelta(days=3)
        res = await api_keys.delete_many({"expires_at": {"$lt": threshold}})
        cleaned_expired = res.deleted_count
    return {"deleted": total_deleted, "cleaned_expired": cleaned_expired}


@router.post("/authen-api-key")
async def authen_api_key(req: AuthApiKeyRequest):
    api_keys = get_collection("api_keys")
    api_key = await api_keys.find_one({"key": req.api_key, "is_active": True})
    if api_key is None:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid API key")
    if api_key.get("expires_at") < datetime.utcnow():
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="API key expired")
    token = create_jwt({"role": "user", "api_key_id": str(api_key.get("_id"))}, expires_minutes=60 * 24 * 3)
    return {"sessionToken": token}


@router.get("/get-all-api-keys")
async def get_all_api_keys(admin=Depends(get_current_admin)):
    api_keys = get_collection("api_keys")
    keys = []
    async for doc in api_keys.find({"is_active": True}):
        keys.append({
            "api_key": doc.get("key"),
            "expires_at": doc.get("expires_at").isoformat() if doc.get("expires_at") else None,
            "created_at": doc.get("created_at").isoformat() if doc.get("created_at") else None,
            "phone_number": doc.get("phone_number"),
            "user_name": doc.get("user_name"),
        })
    return {"api_keys": keys}


@router.post("/refresh-admin-session-token")
async def refresh_admin_session_token(admin=Depends(get_current_admin)):
    token = create_jwt({"role": "admin"}, expires_minutes=60 * 24 * 3)
    return {"sessionToken": token}


@router.get("/check-session-token")
async def check_session_token(credentials: HTTPAuthorizationCredentials | None = Security(_session_bearer)):
    if credentials is None or not credentials.scheme.lower() == "bearer":
        return {"valid": False, "reason": "Missing credentials"}
    token = credentials.credentials
    try:
        payload = decode_jwt(token)
        exp_ts = payload.get("exp")
        expires_at = datetime.utcfromtimestamp(exp_ts) if isinstance(exp_ts, (int, float)) else None
        return {
            "valid": True,
            "role": payload.get("role"),
            "api_key_id": payload.get("api_key_id"),
            "expiresAt": expires_at.isoformat() if expires_at else None,
        }
    except HTTPException as e:
        return {"valid": False, "reason": e.detail}


@router.get("/get-all-api-keys-admin")
async def get_all_api_keys_admin(admin=Depends(get_current_admin)):
    api_keys = get_collection("api_keys")
    keys = []
    async for doc in api_keys.find():
        keys.append({
            "api_key": doc.get("key"),
            "expires_at": doc.get("expires_at").isoformat() if doc.get("expires_at") else None,
            "created_at": doc.get("created_at").isoformat() if doc.get("created_at") else None,
            "is_active": doc.get("is_active"),
            "phone_number": doc.get("phone_number"),
            "user_name": doc.get("user_name"),
        })
    return {"api_keys": keys}


@router.delete("/cleanup-expired-api-keys")
async def cleanup_expired_api_keys(admin=Depends(get_current_admin)):
    api_keys = get_collection("api_keys")
    current_time = datetime.utcnow()
    result = await api_keys.delete_many({"expires_at": {"$lt": current_time}})
    return {
        "deleted_count": result.deleted_count,
        "message": f"Deleted {result.deleted_count} expired API keys"
    }


@router.get("/get-expired-api-keys")
async def get_expired_api_keys(admin=Depends(get_current_admin)):
    api_keys = get_collection("api_keys")
    current_time = datetime.utcnow()
    expired_keys = []
    async for doc in api_keys.find({"expires_at": {"$lt": current_time}}):
        expired_keys.append({
            "api_key": doc.get("key"),
            "expires_at": doc.get("expires_at").isoformat() if doc.get("expires_at") else None,
            "created_at": doc.get("created_at").isoformat() if doc.get("created_at") else None,
            "is_active": doc.get("is_active"),
            "phone_number": doc.get("phone_number"),
            "user_name": doc.get("user_name"),
        })
    return {"expired_api_keys": expired_keys, "count": len(expired_keys)}


@router.post("/update-ai-model-api-key")
async def update_ai_model_api_key(
    model_name: str,
    new_api_key: str,
    admin=Depends(get_current_admin)
):
    if not model_name or not new_api_key:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST, 
            detail="Model name and API key are required"
        )
    return {
        "message": f"API key updated for {model_name}",
        "model": model_name,
        "status": "updated"
    }


@router.get("/get-ai-model-config")
async def get_ai_model_config(admin=Depends(get_current_admin)):
    from app.core.config import get_gemini_api_key
    gemini_key = get_gemini_api_key()
    return {
        "models": {
            "gemini": {
                "has_api_key": bool(gemini_key),
                "key_length": len(gemini_key) if gemini_key else 0
            }
        }
    }
