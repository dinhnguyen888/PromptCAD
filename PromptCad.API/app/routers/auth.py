from datetime import datetime, timedelta
from fastapi import APIRouter, HTTPException, status, Depends
from fastapi.security import HTTPAuthorizationCredentials, HTTPBearer

from app.db.mongo import get_collection
from app.schemas.auth import AdminLoginRequest, TokenResponse
from app.core.security import verify_password, hash_password, create_jwt, get_current_admin
from app.core.config import get_admin_bootstrap


router = APIRouter()


@router.post("/admin-login", response_model=TokenResponse)
async def admin_login(payload: AdminLoginRequest):
    accounts = get_collection("accounts")

    account = await accounts.find_one({"email": payload.email})

    if account is None:
        env_admin_email, env_admin_password = get_admin_bootstrap()
        if env_admin_email and env_admin_password and payload.email.lower() == env_admin_email.lower() and payload.password == env_admin_password:
            from app.models.entities import Account

            new_account = Account(email=payload.email, password_hash=hash_password(payload.password), role="admin")
            insert_res = await accounts.insert_one(new_account.dict(by_alias=True))
            account = await accounts.find_one({"_id": insert_res.inserted_id})
        else:
            raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")

    if account.get("role") != "admin":
        raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail="Admin account required")

    if not verify_password(payload.password, account.get("password_hash", "")):
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")

    token = create_jwt({"sub": str(account.get("_id")), "role": "admin"}, expires_minutes=60 * 24)

    admin_tokens = get_collection("admin_tokens")
    expires_at = datetime.utcnow() + timedelta(minutes=60 * 24)
    await admin_tokens.insert_one({"account_id": str(account.get("_id")), "token": token, "created_at": datetime.utcnow(), "expires_at": expires_at})

    return TokenResponse(access_token=token)


@router.post("/admin-logout")
async def admin_logout(
    admin=Depends(get_current_admin),
    credentials: HTTPAuthorizationCredentials = Depends(HTTPBearer()),
):
    admin_tokens = get_collection("admin_tokens")
    token = credentials.credentials
    res = await admin_tokens.delete_one({"token": token})
    if res.deleted_count == 0:
        return {"message": "Already logged out"}
    return {"message": "Logged out"}

# add check admin-token role
@router.get("/check-admin-token")
async def check_admin_token(
    admin=Depends(get_current_admin),
    credentials: HTTPAuthorizationCredentials = Depends(HTTPBearer()),
):
    admin_tokens = get_collection("admin_tokens")
    token = credentials.credentials
    token_data = await admin_tokens.find_one({"token": token})
    if not token_data:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid or expired token")
    return {"message": "Token is valid", "account_id": token_data.get("account_id")}


@router.post("/reset-admin-token")
async def reset_admin_token(
    admin=Depends(get_current_admin),
    credentials: HTTPAuthorizationCredentials = Depends(HTTPBearer()),
):
    """Reset admin token by invalidating current token and creating a new one"""
    admin_tokens = get_collection("admin_tokens")
    token = credentials.credentials
    
    # Delete current token
    await admin_tokens.delete_one({"token": token})
    
    # Create new token
    new_token = create_jwt({"sub": admin.get("sub"), "role": "admin"}, expires_minutes=60 * 24)
    expires_at = datetime.utcnow() + timedelta(minutes=60 * 24)
    await admin_tokens.insert_one({
        "account_id": admin.get("sub"), 
        "token": new_token, 
        "created_at": datetime.utcnow(), 
        "expires_at": expires_at
    })
    
    return TokenResponse(access_token=new_token)


