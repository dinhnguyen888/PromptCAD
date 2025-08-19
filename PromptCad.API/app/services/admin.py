from datetime import datetime, timedelta
from typing import Optional

from app.db.mongo import get_collection
from app.core.config import get_admin_bootstrap
from app.core.security import hash_password, create_jwt
from app.models.entities import Account


async def get_or_create_admin_account(email: str, password: str) -> Optional[dict]:
    """Find an admin account by email or bootstrap it if env matches.

    Returns the account document (dict) when available, otherwise None.
    """
    accounts = get_collection("accounts")
    account = await accounts.find_one({"email": email})
    if account is not None:
        return account

    env_admin_email, env_admin_password = get_admin_bootstrap()
    if (
        env_admin_email
        and env_admin_password
        and email.lower() == env_admin_email.lower()
        and password == env_admin_password
    ):
        new_account = Account(email=email, password_hash=hash_password(password), role="admin")
        insert_res = await accounts.insert_one(new_account.dict(by_alias=True))
        account = await accounts.find_one({"_id": insert_res.inserted_id})
        return account

    return None


async def create_admin_session(account_id: str) -> str:
    """Create a new admin session token and persist it in admin_tokens."""
    token = create_jwt({"sub": account_id, "role": "admin"}, expires_minutes=60 * 24)
    admin_tokens = get_collection("admin_tokens")
    expires_at = datetime.utcnow() + timedelta(minutes=60 * 24)
    await admin_tokens.insert_one(
        {
            "account_id": account_id,
            "token": token,
            "created_at": datetime.utcnow(),
            "expires_at": expires_at,
        }
    )
    return token


async def invalidate_admin_token(token: str) -> bool:
    """Invalidate an admin token. Returns True if a token was deleted."""
    admin_tokens = get_collection("admin_tokens")
    res = await admin_tokens.delete_one({"token": token})
    return res.deleted_count > 0


async def find_admin_token(token: str) -> Optional[dict]:
    """Find a stored admin token document by token string."""
    admin_tokens = get_collection("admin_tokens")
    return await admin_tokens.find_one({"token": token})


