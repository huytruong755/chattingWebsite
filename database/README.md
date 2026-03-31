# AppChat Database Setup

Thư mục này chứa các SQL scripts để khởi tạo và seed dữ liệu cho database AppChat.

## Cấu trúc files

- **01-schema.sql** - Tạo cấu trúc (schema) database
  - Bảng Users (Người dùng)
  - Bảng Chats (Cuộc trò chuyện)
  - Bảng Messages (Tin nhắn)
  - Bảng Contacts (Danh bạ)
  - Foreign keys và Indexes

- **02-data.sql** - Seed dữ liệu test
  - 9 người dùng mẫu
  - 10 cuộc trò chuyện
  - 90 tin nhắn mẫu
  - 24 quan hệ bạn bè

## Cách sử dụng

### Với Docker Compose (Tự động)

```bash
docker-compose up -d
```

PostgreSQL sẽ tự động:
1. Tạo database `appchatdb`
2. Chạy `01-schema.sql` để tạo bảng
3. Chạy `02-data.sql` để insert dữ liệu test

### Chạy lại migrations

Nếu bạn muốn reset database:

```bash
# Xóa volume để xóa dữ liệu
docker-compose down -v

# Khởi động lại (sẽ chạy SQL scripts từ đầu)
docker-compose up -d
```

### Kết nối trực tiếp đến database

```bash
# Từ bạn cùng máy:
psql -h localhost -U appuser -d appchatdb

# Password: StrongPassword123

# Hoặc từ container:
docker exec -it <container_name> psql -U appuser -d appchatdb
```

## Thông tin kết nối

| Thuộc tính | Giá trị |
|-----------|--------|
| Host | localhost (hoặc `db` từ trong Docker) |
| Port | 5432 |
| Database | appchatdb |
| Username | appuser |
| Password | StrongPassword123 |

## Dữ liệu Test

Database đã có sẵn:
- **9 Users**: Khoa, Phuc, Huy, Thao, Bao, Tuan, Han, Nam, Phuc
- **10 Conversations**: Giữa các users khác nhau
- **90 Messages**: Trong các cuộc trò chuyện
- **24 Contacts**: Quan hệ bạn bè hai chiều

Mọi user có password: `123`

## Troubleshooting

### Database không được khởi tạo

Kiểm tra:
1. Thư mục `database/` có chứa SQL files không?
2. Xóa volume cũ: `docker-compose down -v`
3. Logs: `docker logs <container_name>`

### Lỗi duplicate key

Files SQL được viết với `ON CONFLICT DO NOTHING`, an toàn chạy nhiều lần.

### Port 5432 đã bị chiếm

Đổi port trong `docker-compose.yml`:
```yaml
ports:
  - "5433:5432"  # Dùng 5433 thay vì 5432
```

## Notes

- Schema được tạo với SERIAL IDs (auto-increment)
- Tất cả tables đều có foreign keys và indexes
- Seed data sử dụng UTF-8 hoàn toàn, hỗ trợ emoji
- Timestamps sử dụng UTC format
