import io
from datetime import datetime
from typing import List

import pandas as pd

from app.db.mongo import get_collection


async def log_prompt(text: str, api_key_id: str | None, source: str | None = None, metadata: dict | None = None) -> None:
    prompts = get_collection("prompts")
    await prompts.insert_one(
        {
            "text": text,
            "created_at": datetime.utcnow(),
            "api_key_id": api_key_id,
            "metadata": metadata or {"source": source} if source else metadata,
        }
    )


async def export_prompts_to_excel_tempfile() -> str:
    """Export all prompts to a temporary xlsx file and return its path."""
    prompts = get_collection("prompts")
    all_prompts: List[dict] = []
    async for doc in prompts.find():
        all_prompts.append(
            {
                "prompt_text": doc.get("text", ""),
                "created_at": doc.get("created_at").isoformat() if doc.get("created_at") else "",
                "api_key_id": doc.get("api_key_id", ""),
                "metadata": str(doc.get("metadata", {})),
            }
        )

    if not all_prompts:
        raise ValueError("No prompts found")

    df = pd.DataFrame(all_prompts)
    output = io.BytesIO()
    with pd.ExcelWriter(output, engine="openpyxl") as writer:
        df.to_excel(writer, sheet_name="Prompts", index=False)

    output.seek(0)
    temp_file_path = f"temp_prompts_export_{datetime.utcnow().strftime('%Y%m%d_%H%M%S')}.xlsx"
    with open(temp_file_path, "wb") as f:
        f.write(output.getvalue())

    return temp_file_path


async def delete_all_prompts() -> int:
    prompts = get_collection("prompts")
    result = await prompts.delete_many({})
    return int(result.deleted_count)

# Get all prompts
async def get_all_prompts() -> List[dict]:
    prompts = get_collection("prompts")
    all_prompts: List[dict] = []
    async for doc in prompts.find():
        all_prompts.append(
            {
                "prompt_text": doc.get("text", ""),
                "created_at": doc.get("created_at").isoformat() if doc.get("created_at") else "",
                "metadata": str(doc.get("metadata", {})),
            }
        )
    return all_prompts

