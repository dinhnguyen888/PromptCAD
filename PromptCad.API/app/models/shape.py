from pydantic import BaseModel

class ShapeRequest(BaseModel):
    prompt: str