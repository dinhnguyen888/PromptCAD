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
from app.db.mongo import get_collection
from app.core.security import get_current_user, get_current_admin


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

    prompts = get_collection("prompts")
    await prompts.insert_one({
        "text": req.prompt,
        "created_at": datetime.utcnow(),
        "api_key_id": user.get("api_key_id"),
        "metadata": {"source": "generate-entity"},
    })

    return result


@router.get("/export-prompts-to-excel")
async def export_prompts_to_excel(admin=Depends(get_current_admin)):
    """Export all prompts to Excel file"""
    prompts = get_collection("prompts")
    
    # Get all prompts
    all_prompts = []
    async for doc in prompts.find():
        all_prompts.append({
            "prompt_text": doc.get("text", ""),
            "created_at": doc.get("created_at").isoformat() if doc.get("created_at") else "",
            "api_key_id": doc.get("api_key_id", ""),
            "metadata": str(doc.get("metadata", {}))
        })
    
    if not all_prompts:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="No prompts found")
    
    # Create DataFrame
    df = pd.DataFrame(all_prompts)
    
    # Create Excel file in memory
    output = io.BytesIO()
    with pd.ExcelWriter(output, engine='openpyxl') as writer:
        df.to_excel(writer, sheet_name='Prompts', index=False)
    
    output.seek(0)
    
    # Save to temporary file
    temp_file_path = f"temp_prompts_export_{datetime.utcnow().strftime('%Y%m%d_%H%M%S')}.xlsx"
    with open(temp_file_path, "wb") as f:
        f.write(output.getvalue())
    
    # Return file
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
        # Read Excel file
        file_content = await file.read()
        df = pd.read_excel(io.BytesIO(file_content))
        
        # Validate required columns
        required_columns = ["prompt_text"]
        missing_columns = [col for col in required_columns if col not in df.columns]
        if missing_columns:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST, 
                detail=f"Missing required columns: {missing_columns}"
            )
        
        # Extract prompts
        prompts = df["prompt_text"].dropna().tolist()
        
        if not prompts:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST, 
                detail="No valid prompts found in Excel file"
            )
        
        # Update shapes.txt file
        shapes_file_path = "app/documents/shapes.txt"
        with open(shapes_file_path, "w", encoding="utf-8") as f:
            for prompt in prompts:
                f.write(f"{prompt}\n")
        
        return {
            "message": f"Successfully imported {len(prompts)} prompts to shapes.txt",
            "imported_count": len(prompts)
        }
        
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error importing prompts: {str(e)}"
        )

@router.get("/get-shapes-content")
async def get_shapes_content(admin=Depends(get_current_admin)):
    """Get current content of shapes.txt file"""
    try:
        shapes_file_path = "app/documents/shapes.txt"
        if not os.path.exists(shapes_file_path):
            return {"content": "", "message": "shapes.txt file not found"}
        
        with open(shapes_file_path, "r", encoding="utf-8") as f:
            content = f.read()
        
        return {"content": content, "lines": len(content.splitlines())}
        
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error reading shapes.txt: {str(e)}"
        )

# delete all prompts for admin to garbage collection
@router.delete("/delete-all-prompts")
async def delete_all_prompts(admin=Depends(get_current_admin)):
    """Delete all prompts from the database"""
    prompts = get_collection("prompts")
    result = await prompts.delete_many({})
    
    if result.deleted_count == 0:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="No prompts found to delete"
        )
    
    return {
        "message": f"Successfully deleted {result.deleted_count} prompts",
        "deleted_count": result.deleted_count
    }


