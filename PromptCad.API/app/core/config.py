import os


def get_mongo_uri() -> str:
    return os.getenv("MONGODB_URI", "mongodb://localhost:27017")


def get_database_name() -> str:
    return os.getenv("MONGODB_DB", "promptcad_db")


def get_jwt_secret() -> str:
    return os.getenv("JWT_SECRET", "change-me-secret")


def get_jwt_algorithm() -> str:
    return os.getenv("JWT_ALGORITHM", "HS256")


def get_gemini_api_key() -> str | None:
    return os.getenv("GEMINI_API_KEY")


def get_admin_bootstrap() -> tuple[str | None, str | None]:
    return os.getenv("ADMIN_EMAIL"), os.getenv("ADMIN_PASSWORD")


