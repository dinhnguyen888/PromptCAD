import datetime as dt
import jwt
from passlib.context import CryptContext
from fastapi import HTTPException, status, Depends, Security
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials

from app.core.config import get_jwt_secret, get_jwt_algorithm
from app.db.mongo import get_collection


pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")
security_scheme = HTTPBearer(auto_error=False, scheme_name="SessionToken")


def hash_password(password: str) -> str:
    return pwd_context.hash(password)


def verify_password(plain_password: str, hashed_password: str) -> bool:
    return pwd_context.verify(plain_password, hashed_password)


def create_jwt(payload: dict, expires_minutes: int = 60) -> str:
    to_encode = payload.copy()
    expire = dt.datetime.utcnow() + dt.timedelta(minutes=expires_minutes)
    to_encode.update({"exp": expire, "iat": dt.datetime.utcnow()})
    token = jwt.encode(to_encode, get_jwt_secret(), algorithm=get_jwt_algorithm())
    return token


def decode_jwt(token: str) -> dict:
    try:
        decoded = jwt.decode(token, get_jwt_secret(), algorithms=[get_jwt_algorithm()])
        return decoded
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Token expired")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid token")


async def get_current_admin(credentials: HTTPAuthorizationCredentials = Security(security_scheme)) -> dict:
    if credentials is None or not credentials.scheme.lower() == "bearer":
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Missing credentials")
    token = credentials.credentials
    payload = decode_jwt(token)
    if payload.get("role") != "admin":
        raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail="Admin role required")

    admin_tokens = get_collection("admin_tokens")
    stored = await admin_tokens.find_one({"token": token})
    if stored is None:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Session invalidated")

    return payload


async def get_current_user(credentials: HTTPAuthorizationCredentials = Security(security_scheme)) -> dict:
    if credentials is None or not credentials.scheme.lower() == "bearer":
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Missing credentials")
    token = credentials.credentials
    payload = decode_jwt(token)
    role = payload.get("role")
    if role not in ("user", "admin"):
        raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail="Invalid role")
    return payload


