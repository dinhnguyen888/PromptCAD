from pydantic import BaseModel, Field


class CreateApiKeyRequest(BaseModel):
    duration_months: int = Field(..., description="One of 3, 6, 12")
    user_name: str 
    phone_number: str


class UpdateApiKeyRequest(BaseModel):
    api_key: str
    duration_months: int


class DeleteApiKeyRequest(BaseModel):
    api_key: str
    cleanup_expired: bool = False


class AuthApiKeyRequest(BaseModel):
    api_key: str

class UpdateApiKeyInfoRequest(BaseModel):
    api_key: str
    user_name: str | None = None
    phone_number: str | None = None

