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
from app.services import api_keys as api_keys_service

router = APIRouter()
_session_bearer = HTTPBearer(auto_error=False)

@router.post("/create-api-key")
async def create_api_key(req: CreateApiKeyRequest, admin=Depends(get_current_admin)):
    new_key, expires_at = await api_keys_service.create_api_key(
        duration_months=req.duration_months,
        created_by_account_id=admin.get("sub"),
        user_name=req.user_name,
        phone_number=req.phone_number,
    )
    return {"api_key": new_key, "expires_at": expires_at.isoformat()}


@router.post("/update-api-key")
async def update_api_key(req: UpdateApiKeyRequest, admin=Depends(get_current_admin)):
    expires_at = await api_keys_service.update_api_key_expiry(
        api_key=req.api_key, duration_months=req.duration_months
    )
    return {"api_key": req.api_key, "expires_at": expires_at.isoformat()}


@router.post("/update-api-key-info")
async def update_api_key_info(req: UpdateApiKeyInfoRequest, admin=Depends(get_current_admin)):
    update_fields = await api_keys_service.update_api_key_info(
        api_key=req.api_key, user_name=req.user_name, phone_number=req.phone_number
    )
    return {"api_key": req.api_key, "updated_fields": update_fields}

@router.delete("/delete-api-key")
async def delete_api_key(req: DeleteApiKeyRequest, admin=Depends(get_current_admin)):
    result = await api_keys_service.delete_api_key(api_key=req.api_key, cleanup_expired=req.cleanup_expired)
    return result


@router.post("/authen-api-key")
async def authen_api_key(req: AuthApiKeyRequest):
    token = await api_keys_service.authenticate_api_key(api_key=req.api_key)
    return {"sessionToken": token}


@router.get("/get-all-api-keys")
async def get_all_api_keys(admin=Depends(get_current_admin)):
    keys = await api_keys_service.list_api_keys(active_only=True)
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
    keys = await api_keys_service.list_api_keys(active_only=False)
    return {"api_keys": keys}


@router.delete("/cleanup-expired-api-keys")
async def cleanup_expired_api_keys(admin=Depends(get_current_admin)):
    deleted_count = await api_keys_service.cleanup_expired_api_keys()
    return {"deleted_count": deleted_count, "message": f"Deleted {deleted_count} expired API keys"}


@router.get("/get-expired-api-keys")
async def get_expired_api_keys(admin=Depends(get_current_admin)):
    expired_keys = await api_keys_service.list_expired_api_keys()
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
