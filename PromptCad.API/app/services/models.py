from typing import List


def print_available_models(api_key: str | None) -> None:
    """Print available text-generation models to console if API key is provided.

    This is best-effort and will not raise; failures are printed and ignored.
    """
    if not api_key:
        print("[models] GEMINI_API_KEY not set; skipping model listing")
        return

    try:
        import google.generativeai as genai

        genai.configure(api_key=api_key)
        models: List = list(genai.list_models())
        printable = []
        for m in models:
            try:
                methods = getattr(m, "supported_generation_methods", []) or []
                if "generateContent" in methods:
                    name = getattr(m, "name", "<unknown>")
                    input_types = getattr(m, "input_token_limit", None)
                    output_types = getattr(m, "output_token_limit", None)
                    printable.append((name, input_types, output_types))
            except Exception:
                continue

        if not printable:
            print("[models] No text-generation models available or listing returned empty")
            return

        print("[models] Available Gemini models (name | input_tokens | output_tokens):")
        for name, in_lim, out_lim in printable:
            print(f" - {name} | in={in_lim} | out={out_lim}")

    except Exception as e:
        print(f"[models] Failed to list models: {e}")


