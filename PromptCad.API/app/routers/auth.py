from datetime import datetime, timedelta
from fastapi import APIRouter, HTTPException, status, Depends
from fastapi.security import HTTPAuthorizationCredentials, HTTPBearer

from app.db.mongo import get_collection
from app.schemas.auth import AdminLoginRequest, TokenResponse
from app.core.security import verify_password, hash_password, create_jwt, get_current_admin
from app.services.admin import get_or_create_admin_account, create_admin_session, invalidate_admin_token, find_admin_token


router = APIRouter()


@router.post("/admin-login", response_model=TokenResponse)
async def admin_login(payload: AdminLoginRequest):
    account = await get_or_create_admin_account(payload.email, payload.password)
    if account is None:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")

    if account.get("role") != "admin":
        raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail="Admin account required")

    if not verify_password(payload.password, account.get("password_hash", "")):
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")

    token = await create_admin_session(str(account.get("_id")))
    return TokenResponse(access_token=token)


@router.post("/admin-logout")
async def admin_logout(
    admin=Depends(get_current_admin),
    credentials: HTTPAuthorizationCredentials = Depends(HTTPBearer()),
):
    token = credentials.credentials
    deleted = await invalidate_admin_token(token)
    if not deleted:
        return {"message": "Already logged out"}
    return {"message": "Logged out"}

# add check admin-token role
@router.get("/check-admin-token")
async def check_admin_token(
    admin=Depends(get_current_admin),
    credentials: HTTPAuthorizationCredentials = Depends(HTTPBearer()),
):
    token = credentials.credentials
    token_data = await find_admin_token(token)
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


