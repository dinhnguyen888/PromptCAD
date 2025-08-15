# Cấu hình Biến Môi Trường - PromptCad

## Backend API (.env file)

Tạo file `.env` trong thư mục `PromptCad.API/`:

```
MONGODB_URI=mongodb://localhost:27017
MONGODB_DB=promptcad_db
JWT_SECRET=your-secret-key
JWT_ALGORITHM=HS256
GEMINI_API_KEY=your-gemini-api-key
ADMIN_EMAIL=admin@example.com
ADMIN_PASSWORD=admin123
```

## AdminPanel Configuration

Cập nhật file `PromptCad.AdminPanel/Services/globalAPI.cs`:

```csharp
public static string ApiUrl { get; set; } = "http://127.0.0.1:8000/api";
public static string TokenFilePath { get; set; } = "C:\\Users\\Public\\Documents\\AdminPromptCad\\access_token.txt";
```

## Plugin Configuration

Cập nhật file `PromptCad.Plugin/Utility/globalAPI.cs`:

```csharp
public static string ApiUrl { get; set; } = "http://127.0.0.1:8000/api";
```

## Lưu ý

- Thay đổi các giá trị theo môi trường thực tế
- Không commit file .env vào git
- Backup cấu hình trước khi thay đổi

## Hướng dẫn chạy Backend (Python)

1. **Cài đặt Python**  
   Đảm bảo bạn đã cài đặt Python 3.8 trở lên.

2. **Cài đặt các thư viện cần thiết**  
   Mở terminal/cmd và chạy:
   ```
   pip install -r requirements.txt
   ```
   (Chạy lệnh này trong thư mục `PromptCad.API`)

3. **Tạo file cấu hình môi trường**  
   Tạo file `.env` trong thư mục `PromptCad.API` như hướng dẫn ở trên.

4. **Chạy server backend**  
   Trong thư mục `PromptCad.API`, chạy:
   ```
   .\env\Scripts\Activate.ps1
   ```
   ```
   uvicorn main:app --reload 
   ```
6. **Chạy AdminPanel và Plugin, mở visual studio, chọn PromptCad.Solutions và ấn run**
