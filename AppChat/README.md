# AppChat — Hướng dẫn Backend (Tiếng Việt)

## Mục đích
- Đây là backend của ứng dụng AppChat, triển khai API, hubs (SignalR), repository/service pattern và lưu trữ tệp tĩnh trong `wwwroot/uploads`.

## Yêu cầu trước
- .NET SDK (6.0/7.0 trở lên) — cài đặt từ https://dotnet.microsoft.com/
- (Tuỳ chọn) Visual Studio 2022/Visual Studio Code
- (Tuỳ chọn) Docker nếu muốn chạy trong container
- Thiết lập cơ sở dữ liệu: kiểm tra `appsettings.json` để cập nhật connection string (ví dụ SQL Server / PostgreSQL / SQLite)

## Cấu trúc chính
- `AppChat/` — project chính (chứa Program.cs, Controllers, Hubs, Models, Services, Repository)
- `wwwroot/uploads/` — nơi lưu file upload
- `Data/AppDbContext.cs` — DbContext nếu dùng Entity Framework
- `AppChat.sln` — solution

## Hướng dẫn build và chạy (CLI)
1. Mở terminal, chuyển tới thư mục gốc chứa `AppChat.sln` hoặc vào thư mục `AppChat/`:

```powershell
cd AppChat
```

2. Cài phụ thuộc (restore) và build:

```powershell
dotnet restore
dotnet build
```

3. Cấu hình `appsettings.json`:
- Mở `AppChat/appsettings.json` và chỉnh `ConnectionStrings` phù hợp với DB của bạn.
- Nếu cần, tạo database và chạy migration (nếu project dùng EF Core).

4. (Nếu dùng EF Core Migrations) tạo và áp dụng migration:

```powershell
dotnet tool install --global dotnet-ef  # nếu chưa cài
dotnet ef migrations add InitialCreate --project AppChat --startup-project AppChat
dotnet ef database update --project AppChat --startup-project AppChat
```

5. Chạy ứng dụng:

```powershell
dotnet run --project AppChat
```

- Sau khi chạy, API thường lắng nghe trên `http://localhost:5000` và `https://localhost:5001` (kiểm tra `Properties/launchSettings.json` để xác nhận).

## Chạy bằng Visual Studio
- Mở `AppChat.sln` trong Visual Studio.
- Đặt project `AppChat` làm startup project.
- Nhấn F5 để chạy (debug) hoặc Ctrl+F5 để chạy không debug.

## Chạy bằng Docker
- Project có `Dockerfile`, build image và chạy:

```powershell
# từ thư mục chứa Dockerfile
docker build -t appchat-backend .
docker run -p 5000:80 -e "ASPNETCORE_ENVIRONMENT=Production" appchat-backend
```

(Điều chỉnh biến môi trường và port theo nhu cầu.)

## Uploads và quyền
- Thư mục `wwwroot/uploads` cần có quyền ghi cho process chạy ứng dụng nếu bạn dùng upload file.

## Cách debug & kiểm tra
- Kiểm tra log console khi chạy để biết lỗi kết nối DB hoặc lỗi khởi tạo.
- Nếu API có Swagger, mở đường dẫn `http://localhost:5000/swagger` (hoặc cổng tương ứng) để thử endpoint.
- Kiểm tra `Hubs/ChatHub.cs` để hiểu các event SignalR.

## Gợi ý trang web và tài nguyên học C# (tiếng Việt & tiếng Anh)
- Microsoft Learn / .NET Docs: https://learn.microsoft.com/dotnet — Tài liệu chính thức, rất đầy đủ.
- C# Guide (Microsoft): https://learn.microsoft.com/dotnet/csharp
- freeCodeCamp: https://www.freecodecamp.org/ — có nhiều khóa và video miễn phí.
- Udemy / Coursera / Pluralsight — các khóa có hướng dẫn dự án thực tế (thường trả phí).
- YouTube channels: "IAmTimCorey", "Programming with Mosh" — tutorial C# thực tế, dễ hiểu.
- Học bằng tiếng Việt:
  - Viblo: https://viblo.asia — cộng đồng kỹ thuật Việt Nam, nhiều bài hướng dẫn .NET/C#.
  - Các blog và cộng đồng trên Facebook/YouTube của lập trình viên Việt Nam (tìm theo từ khoá "C#" hoặc ".NET Core").
- Học thực hành API & SignalR:
  - .NET Documentation (SignalR): https://learn.microsoft.com/aspnet/core/signalr
  - Các tutorial dạng dự án trên YouTube (tìm "ASP.NET Core SignalR tutorial").

## Mẹo & best practices
- Quản lý secrets: không commit thông tin nhạy cảm (connection strings, keys) vào git. Dùng `User Secrets` hoặc biến môi trường cho môi trường phát triển/triển khai.
- Logging: sử dụng logging built-in của .NET để dễ debug.
- Cấu hình CORS nếu frontend kết nối tới backend từ domain khác.

## Những bước tiếp theo (gợi ý)
- Thêm file README này vào repository để người khác dễ bắt đầu.
- Tạo script Docker Compose nếu muốn chạy DB + app cùng lúc.
- Viết hướng dẫn migration cụ thể nếu DB và migrations đã được thiết lập.

---
Nếu bạn muốn, tôi có thể:
- Thêm phần hướng dẫn cụ thể cho DB bạn đang dùng (SQL Server / PostgreSQL / SQLite).
- Tạo `docker-compose.yml` mẫu để chạy app + database.
