from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from dotenv import load_dotenv
from contextlib import asynccontextmanager
from app.routers import shape
from app.routers import auth, api_key
from app.db.indexes import ensure_indexes
from app.core.config import get_gemini_api_key
from app.services.models import print_available_models

# Load .env
load_dotenv()

@asynccontextmanager
async def lifespan(app: FastAPI):
    # Startup logic
    try:
        await ensure_indexes()
    except Exception as e:
        # Allow app to start even if MongoDB is not reachable
        print(f"[startup] Skipping index creation: {e}")

    # Print model listing (best-effort)
    try:
        print_available_models(get_gemini_api_key())
    except Exception as e:
        print(f"[startup] Skipping model listing: {e}")

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
