import secrets
from datetime import datetime, timedelta
from typing import List, Dict, Tuple

from fastapi import HTTPException, status

from app.db.mongo import get_collection
from app.core.security import create_jwt


def _months_to_timedelta(months: int) -> timedelta:
    if months not in (3, 6, 12):
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail="duration_months must be one of 3, 6, 12",
        )
    return timedelta(days=months * 30)


async def create_api_key(duration_months: int, created_by_account_id: str, user_name: str, phone_number: str) -> Tuple[str, datetime]:
    api_keys = get_collection("api_keys")
    expires_at = datetime.utcnow() + _months_to_timedelta(duration_months)
    new_key = secrets.token_urlsafe(32)
    doc = {
        "key": new_key,
        "expires_at": expires_at,
        "created_at": datetime.utcnow(),
        "created_by_account_id": created_by_account_id,
        "is_active": True,
        "user_name": user_name,
        "phone_number": phone_number,
    }
    await api_keys.insert_one(doc)
    return new_key, expires_at


async def update_api_key_expiry(api_key: str, duration_months: int) -> datetime:
    api_keys = get_collection("api_keys")
    expires_at = datetime.utcnow() + _months_to_timedelta(duration_months)
    res = await api_keys.update_one({"key": api_key}, {"$set": {"expires_at": expires_at}})
    if res.matched_count == 0:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="API key not found")
    return expires_at


async def update_api_key_info(api_key: str, user_name: str | None, phone_number: str | None) -> Dict:
    api_keys = get_collection("api_keys")
    update_fields: Dict = {}
    if user_name is not None:
        update_fields["user_name"] = user_name
    if phone_number is not None:
        update_fields["phone_number"] = phone_number
    if not update_fields:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail="No fields to update")
    res = await api_keys.update_one({"key": api_key}, {"$set": update_fields})
    if res.matched_count == 0:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="API key not found")
    return update_fields


async def delete_api_key(api_key: str, cleanup_expired: bool) -> Dict[str, int]:
    api_keys = get_collection("api_keys")
    deleted = await api_keys.delete_one({"key": api_key})
    total_deleted = deleted.deleted_count
    cleaned_expired = 0
    if cleanup_expired:
        threshold = datetime.utcnow() - timedelta(days=3)
        res = await api_keys.delete_many({"expires_at": {"$lt": threshold}})
        cleaned_expired = res.deleted_count
    return {"deleted": total_deleted, "cleaned_expired": cleaned_expired}


async def authenticate_api_key(api_key: str) -> str:
    api_keys = get_collection("api_keys")
    doc = await api_keys.find_one({"key": api_key, "is_active": True})
    if doc is None:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid API key")
    if doc.get("expires_at") < datetime.utcnow():
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="API key expired")
    token = create_jwt({"role": "user", "api_key_id": str(doc.get("_id"))}, expires_minutes=60 * 24 * 3)
    return token


async def list_api_keys(active_only: bool) -> List[Dict]:
    api_keys = get_collection("api_keys")
    keys: List[Dict] = []
    filter_query = {"is_active": True} if active_only else {}
    async for doc in api_keys.find(filter_query):
        keys.append(
            {
                "api_key": doc.get("key"),
                "expires_at": doc.get("expires_at").isoformat() if doc.get("expires_at") else None,
                "created_at": doc.get("created_at").isoformat() if doc.get("created_at") else None,
                "is_active": doc.get("is_active"),
                "phone_number": doc.get("phone_number"),
                "user_name": doc.get("user_name"),
            }
        )
    return keys


async def cleanup_expired_api_keys() -> int:
    api_keys = get_collection("api_keys")
    current_time = datetime.utcnow()
    result = await api_keys.delete_many({"expires_at": {"$lt": current_time}})
    return result.deleted_count


async def list_expired_api_keys() -> List[Dict]:
    api_keys = get_collection("api_keys")
    current_time = datetime.utcnow()
    expired_keys: List[Dict] = []
    async for doc in api_keys.find({"expires_at": {"$lt": current_time}}):
        expired_keys.append(
            {
                "api_key": doc.get("key"),
                "expires_at": doc.get("expires_at").isoformat() if doc.get("expires_at") else None,
                "created_at": doc.get("created_at").isoformat() if doc.get("created_at") else None,
                "is_active": doc.get("is_active"),
                "phone_number": doc.get("phone_number"),
                "user_name": doc.get("user_name"),
            }
        )
    return expired_keys


