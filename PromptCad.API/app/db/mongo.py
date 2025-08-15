import os
from motor.motor_asyncio import AsyncIOMotorClient, AsyncIOMotorDatabase

_client: AsyncIOMotorClient | None = None
_db: AsyncIOMotorDatabase | None = None


def get_mongo_uri() -> str:
    # Standardize env var name; fallback to previous MONGO_URL for compatibility
    mongo_uri = os.getenv("MONGODB_URI") or os.getenv("MONGO_URL") or "mongodb://localhost:27017"
    return mongo_uri


def get_database_name() -> str:
    return os.getenv("MONGODB_DB", "promptcad_db")


def get_db() -> AsyncIOMotorDatabase:
    global _client, _db
    if _db is None:
        _client = AsyncIOMotorClient(get_mongo_uri())
        _db = _client[get_database_name()]
    return _db


def get_collection(collection_name: str):
    return get_db()[collection_name]


