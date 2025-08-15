import os
from fastapi import Depends

from app.services.rag import RAGService
from app.services.gemini import GeminiService
from app.core.config import get_gemini_api_key


def get_rag_service() -> RAGService:
    return RAGService(api_key=get_gemini_api_key())


def get_gemini_service() -> GeminiService:
    return GeminiService(api_key=get_gemini_api_key())


