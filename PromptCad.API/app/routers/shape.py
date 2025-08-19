from datetime import datetime
from fastapi import APIRouter, Depends, HTTPException, status, File, UploadFile
from fastapi.responses import FileResponse
import pandas as pd
import io
import os

from app.api.deps import get_rag_service, get_gemini_service
from app.models.shape import ShapeRequest
from app.services.rag import RAGService
from app.services.gemini import GeminiService
from app.core.security import get_current_user, get_current_admin
from app.services.prompt import log_prompt, export_prompts_to_excel_tempfile, delete_all_prompts as svc_delete_all_prompts, get_all_prompts as svc_get_all_prompts
from app.services.shapes_file import extract_prompts_from_excel, update_shapes_from_prompts, read_shapes_content


router = APIRouter()


@router.post("/generate-entity")
async def generate_shape(
    req: ShapeRequest,
    rag_service: RAGService = Depends(get_rag_service),
    gemini_service: GeminiService = Depends(get_gemini_service),
    user=Depends(get_current_user),
):
    context = rag_service.retrieve_context(req.prompt)
    result = gemini_service.generate_shape_content(prompt=req.prompt, context=context)

    await log_prompt(text=req.prompt, api_key_id=user.get("api_key_id"), source="generate-entity")

    return result


@router.get("/export-prompts-to-excel")
async def export_prompts_to_excel(admin=Depends(get_current_admin)):
    """Export all prompts to Excel file"""
    try:
        temp_file_path = await export_prompts_to_excel_tempfile()
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=str(e))
    return FileResponse(
        temp_file_path,
        media_type="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        filename=f"prompts_export_{datetime.utcnow().strftime('%Y%m%d_%H%M%S')}.xlsx"
    )

@router.post("/import-prompts-from-excel")
async def import_prompts_from_excel(
    file: UploadFile = File(...),
    admin=Depends(get_current_admin)
):
    """Import prompts from Excel file and update shapes.txt"""
    try:
        file_content = await file.read()
        prompts = extract_prompts_from_excel(file_content)
        if not prompts:
            raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail="No valid prompts found in Excel file")
        imported_count = update_shapes_from_prompts(prompts)
        return {"message": f"Successfully imported {imported_count} prompts to shapes.txt", "imported_count": imported_count}
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail=str(e))
    except Exception as e:
        raise HTTPException(status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail=f"Error importing prompts: {str(e)}")

@router.get("/get-shapes-content")
async def get_shapes_content(admin=Depends(get_current_admin)):
    """Get current content of shapes.txt file"""
    try:
        content, lines = read_shapes_content()
        if lines == 0 and content == "":
            return {"content": content, "message": "shapes.txt file not found"}
        return {"content": content, "lines": lines}
    except Exception as e:
        raise HTTPException(status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail=f"Error reading shapes.txt: {str(e)}")

# delete all prompts for admin to garbage collection
@router.delete("/delete-all-prompts")
async def delete_all_prompts(admin=Depends(get_current_admin)):
    """Delete all prompts from the database"""
    deleted_count = await svc_delete_all_prompts()
    if deleted_count == 0:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="No prompts found to delete"
        )
    return {"message": f"Successfully deleted {deleted_count} prompts", "deleted_count": deleted_count}

# get all prompts for admin
@router.get("/get-all-prompts")
async def get_all_prompts(admin=Depends(get_current_admin)):
    """Get all prompts from the database"""
    prompts = await  svc_get_all_prompts()
    # if not prompts:
    #     raise HTTPException(
    #         status_code=status.HTTP_404_NOT_FOUND,
    #         detail="No prompts found"
    #     )
    return {"prompts": prompts, "count": len(prompts)}
