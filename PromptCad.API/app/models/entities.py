from __future__ import annotations

from typing import Optional, Literal
from pydantic import BaseModel, Field, EmailStr
from datetime import datetime


class Account(BaseModel):
    id: Optional[str] = Field(default=None, alias="_id")
    email: EmailStr
    password_hash: str
    role: Literal["admin", "user"] = "user"
    created_at: datetime = Field(default_factory=datetime.utcnow)


class APIKey(BaseModel):
    id: Optional[str] = Field(default=None, alias="_id")
    key: str
    expires_at: datetime
    created_at: datetime = Field(default_factory=datetime.utcnow)
    created_by_account_id: Optional[str] = None
    is_active: bool = True
    user_name: str
    phone_number: str


class AdminToken(BaseModel):
    id: Optional[str] = Field(default=None, alias="_id")
    account_id: str
    token: str
    created_at: datetime = Field(default_factory=datetime.utcnow)
    expires_at: Optional[datetime] = None


class Prompt(BaseModel):
    id: Optional[str] = Field(default=None, alias="_id")
    text: str
    account_id: Optional[str] = None
    api_key_id: Optional[str] = None
    created_at: datetime = Field(default_factory=datetime.utcnow)
    metadata: Optional[dict] = None


# Request/Response models
class AdminLoginRequest(BaseModel):
    email: EmailStr
    password: str


