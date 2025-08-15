import os
import json
import google.generativeai as genai
from jinja2 import Environment, FileSystemLoader

class GeminiService:
    def __init__(self, api_key: str):
        self.api_key = api_key
        genai.configure(api_key=api_key)
        self.model = genai.GenerativeModel("models/gemini-2.5-pro")
        self.template_env = Environment(loader=FileSystemLoader("app/templates"))

    def generate_shape_content(self, prompt: str, context: str) -> dict:
        """Sinh nội dung về hình học sử dụng Gemini"""
        # Load template và render prompt
        template = self.template_env.get_template("shape_prompt.j2")
        prompt_text = template.render(
            prompt=prompt,
            context=context
        )

        # Gọi Gemini API
        response = self.model.generate_content(prompt_text)
        response_text = response.text
        
        # Remove markdown if present
        if response_text.startswith("```json"):
            response_text = response_text.replace("```json", "").replace("```", "").strip()
        
        try:
            # Parse JSON response
            result = json.loads(response_text)
            
            return {
                "prompt": prompt_text,
                "result": result["content"],
                "type_response": result["type"]
            }
        except json.JSONDecodeError:
            # Fallback nếu response không phải JSON
            return {
                "prompt": prompt_text,
                "result": response.text,
                "type_response": "text"
            }
        except KeyError:
            # Fallback nếu JSON không có đúng cấu trúc
            return {
                "prompt": prompt_text,
                "result": response.text,
                "type_response": "text"
            }