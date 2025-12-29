# Civic Registry API - .NET 8

Backend API cho hệ thống Civic Registry sử dụng ASP.NET Core 8.0 với Entity Framework Core và Swagger.

## Yêu cầu

- .NET 8 SDK
- SQL Server hoặc LocalDB

## Cài đặt và chạy

### 1. Restore packages
```bash
dotnet restore
```

### 2. Build project
```bash
dotnet build
```

### 3. Chạy project
```bash
dotnet run
```

Hoặc trong Cursor/VS Code, nhấn F5 hoặc chạy từ terminal.

## Truy cập

Sau khi chạy, truy cập:

- **Swagger UI:** `https://localhost:5001/swagger` hoặc `http://localhost:5000/swagger`
- **API Base URL:** `https://localhost:5001/api` hoặc `http://localhost:5000/api`

## Cấu trúc project

```
civic-registry/
├── Controllers/
│   └── Api/
│       └── BaseApiController.cs    # Base controller
├── Models/
│   ├── BaseModel.cs               # Base model với soft delete
│   └── ApplicationDbContext.cs    # EF Core DbContext
├── Program.cs                     # Entry point
├── appsettings.json               # Cấu hình
└── CivicRegistry.API.csproj       # Project file
```

## Packages sử dụng

- **Microsoft.EntityFrameworkCore.SqlServer** (8.0.0) - Entity Framework Core
- **Swashbuckle.AspNetCore** (6.5.0) - Swagger/OpenAPI

## Database

Connection string được cấu hình trong `appsettings.json`. Mặc định sử dụng LocalDB.

Để tạo database migration:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Phát triển tiếp

1. Tạo models mới trong `Models/` kế thừa từ `BaseModel`
2. Tạo controllers mới trong `Controllers/Api/` kế thừa từ `BaseApiController`
3. Thêm DbSet vào `ApplicationDbContext`

