from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from dotenv import load_dotenv
from contextlib import asynccontextmanager
from app.routers import shape
from app.routers import auth, api_key
from app.db.mongo import get_collection

# Load .env
load_dotenv()

@asynccontextmanager
async def lifespan(app: FastAPI):
    # Startup logic
    try:
        # Accounts
        accounts = get_collection("accounts")
        await accounts.create_index("email", unique=True)

        # API keys
        api_keys = get_collection("api_keys")
        await api_keys.create_index("key", unique=True)
        # Auto delete API keys 3 days after their expires_at
        await api_keys.create_index("expires_at", expireAfterSeconds=3 * 24 * 3600)

        # Admin tokens should expire exactly at expires_at
        admin_tokens = get_collection("admin_tokens")
        await admin_tokens.create_index("expires_at", expireAfterSeconds=0)
    except Exception as e:
        # Allow app to start even if MongoDB is not reachable
        print(f"[startup] Skipping index creation: {e}")

    yield  # App runs here

    # Shutdown logic (nếu cần thì thêm tại đây)
    print("[shutdown] App is stopping...")

# Khởi tạo FastAPI app với lifespan
app = FastAPI(lifespan=lifespan)

"""Register routers and endpoints"""
app.include_router(shape.router, prefix="/api")
app.include_router(auth.router, prefix="/api")
app.include_router(api_key.router, prefix="/api")

# Enable Swagger authorization using Bearer token
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)
