import io
import os
from typing import Tuple, List

import pandas as pd


SHAPES_FILE_PATH = "app/documents/shapes.txt"


def update_shapes_from_prompts(prompts: List[str]) -> int:
    with open(SHAPES_FILE_PATH, "w", encoding="utf-8") as f:
        for prompt in prompts:
            f.write(f"{prompt}\n")
    return len(prompts)


def read_shapes_content() -> Tuple[str, int]:
    if not os.path.exists(SHAPES_FILE_PATH):
        return "", 0
    with open(SHAPES_FILE_PATH, "r", encoding="utf-8") as f:
        content = f.read()
    return content, len(content.splitlines())


def extract_prompts_from_excel(file_bytes: bytes) -> List[str]:
    df = pd.read_excel(io.BytesIO(file_bytes))
    required_columns = ["prompt_text"]
    missing_columns = [col for col in required_columns if col not in df.columns]
    if missing_columns:
        raise ValueError(f"Missing required columns: {missing_columns}")
    prompts = df["prompt_text"].dropna().tolist()
    return prompts


