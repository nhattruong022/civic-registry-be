# Hệ Thống Quản Lý Dân Cư (Civic Registry System)

## Tổng Quan

Hệ thống quản lý dân cư là một ứng dụng web được thiết kế để quản lý thông tin dân cư, hộ khẩu, và các yêu cầu hành chính của công dân. Hệ thống hỗ trợ phân cấp quản lý từ cấp quốc gia xuống cấp xã/phường.

## Phân Cấp Quản Lý

Hệ thống được tổ chức theo cấu trúc hành chính:
- **Quốc gia** → **Tỉnh/Thành phố** → **Huyện/Quận** → **Xã/Phường** → **Thôn/Bản**

## Các Vai Trò (Roles) Trong Hệ Thống

### 1. SuperAdmin – Quản trị viên hệ thống

**Quyền hạn:**
- Toàn quyền quản lý hệ thống
- Quản lý tất cả các tài khoản cán bộ (ProvinceAdmin, DistrictAdmin, WardAdmin)
- Phân quyền địa bàn quản lý cho các cán bộ
- Xem thống kê toàn quốc
- Theo dõi nhật ký hệ thống (Audit Log)

**Flow nghiệp vụ:**
1. **Đăng nhập** vào hệ thống
2. **Dashboard toàn quốc**
   - Xem tổng quan thống kê toàn quốc
   - Theo dõi số liệu dân cư, hộ khẩu
   - Xem số lượng yêu cầu đang xử lý
3. **Quản lý tài khoản cán bộ**
   - Tạo tài khoản cho ProvinceAdmin (Cán bộ Tỉnh)
   - Tạo tài khoản cho DistrictAdmin (Cán bộ Huyện)
   - Tạo tài khoản cho WardAdmin (Cán bộ Xã)
   - Phân quyền địa bàn quản lý (gán ProvinceId, DistrictId, WardId)
   - Cập nhật thông tin tài khoản
   - Vô hiệu hóa tài khoản (soft delete)
   - Reset mật khẩu cho cán bộ
4. **Theo dõi Audit Log**
   - Xem nhật ký các thao tác trong hệ thống
   - Theo dõi thay đổi dữ liệu
   - Kiểm tra hoạt động của các cán bộ

**API sử dụng:**
- `POST /api/auth/login` - Đăng nhập
- `GET /api/users` - Xem danh sách tất cả cán bộ
- `POST /api/users` - Tạo tài khoản cán bộ
- `PUT /api/users/{id}` - Cập nhật thông tin cán bộ
- `DELETE /api/users/{id}` - Vô hiệu hóa tài khoản
- `GET /api/statistics/province` - Xem thống kê toàn quốc
- `GET /api/audit-logs` - Xem nhật ký hệ thống

---

### 2. ProvinceAdmin – Cán bộ Tỉnh/Thành phố

**Quyền hạn:**
- Quản lý tài khoản DistrictAdmin (Cán bộ Huyện) trong tỉnh
- Xem và duyệt hồ sơ do huyện gửi lên
- Xem thống kê tỉnh
- Duyệt/từ chối yêu cầu cấp tỉnh

**Flow nghiệp vụ:**
1. **Đăng nhập** vào hệ thống
2. **Dashboard tỉnh**
   - Xem thống kê tỉnh (số hộ khẩu, nhân khẩu, yêu cầu)
   - Theo dõi số liệu trong phạm vi tỉnh
3. **Quản lý DistrictAdmin**
   - Xem danh sách DistrictAdmin trong tỉnh
   - Tạo tài khoản DistrictAdmin mới
   - Cập nhật thông tin DistrictAdmin
   - Vô hiệu hóa tài khoản DistrictAdmin
   - Reset mật khẩu cho DistrictAdmin
4. **Xem hồ sơ huyện gửi lên**
   - Xem danh sách yêu cầu đang chờ xử lý
   - Xem chi tiết từng yêu cầu
5. **Duyệt hồ sơ cấp tỉnh**
   - Duyệt các yêu cầu phức tạp cần phê duyệt cấp tỉnh
   - Từ chối yêu cầu không hợp lệ
   - Ghi nhận người xử lý và thời gian xử lý
6. **Xem thống kê tỉnh**
   - Tổng số hộ khẩu trong tỉnh
   - Tổng số nhân khẩu trong tỉnh
   - Số yêu cầu đang chờ xử lý
   - Số yêu cầu đã duyệt/từ chối
   - Biến động dân cư trong tháng

**API sử dụng:**
- `POST /api/auth/login` - Đăng nhập
- `GET /api/users?role=DistrictAdmin` - Xem danh sách DistrictAdmin
- `POST /api/users` - Tạo tài khoản DistrictAdmin
- `PUT /api/users/{id}` - Cập nhật DistrictAdmin
- `DELETE /api/users/{id}` - Vô hiệu hóa DistrictAdmin
- `GET /api/requests/pending` - Xem yêu cầu đang chờ
- `PUT /api/requests/{id}/approve` - Duyệt yêu cầu
- `PUT /api/requests/{id}/reject` - Từ chối yêu cầu
- `GET /api/statistics/province` - Xem thống kê tỉnh

---

### 3. DistrictAdmin – Cán bộ Huyện/Quận

**Quyền hạn:**
- Quản lý tài khoản WardAdmin (Cán bộ Xã) trong huyện
- Xem và duyệt hồ sơ do xã gửi lên
- Xem thống kê huyện
- Duyệt/từ chối yêu cầu cấp huyện

**Flow nghiệp vụ:**
1. **Đăng nhập** vào hệ thống
2. **Dashboard huyện**
   - Xem thống kê huyện (số hộ khẩu, nhân khẩu, yêu cầu)
   - Theo dõi số liệu trong phạm vi huyện
3. **Quản lý WardAdmin**
   - Xem danh sách WardAdmin trong huyện
   - Tạo tài khoản WardAdmin mới
   - Cập nhật thông tin WardAdmin
   - Vô hiệu hóa tài khoản WardAdmin
   - Reset mật khẩu cho WardAdmin
4. **Nhận hồ sơ xã gửi lên**
   - Xem danh sách yêu cầu đang chờ xử lý từ các xã
   - Xem chi tiết từng yêu cầu
5. **Duyệt/Từ chối hồ sơ**
   - Duyệt các yêu cầu hợp lệ
   - Từ chối yêu cầu không đủ điều kiện
   - Chuyển tiếp yêu cầu phức tạp lên cấp tỉnh
   - Ghi nhận người xử lý và thời gian xử lý
6. **Xem thống kê huyện**
   - Tổng số hộ khẩu trong huyện
   - Tổng số nhân khẩu trong huyện
   - Số yêu cầu đang chờ xử lý
   - Số yêu cầu đã duyệt/từ chối
   - Biến động dân cư trong tháng

**API sử dụng:**
- `POST /api/auth/login` - Đăng nhập
- `GET /api/users?role=WardAdmin` - Xem danh sách WardAdmin
- `POST /api/users` - Tạo tài khoản WardAdmin
- `PUT /api/users/{id}` - Cập nhật WardAdmin
- `DELETE /api/users/{id}` - Vô hiệu hóa WardAdmin
- `GET /api/requests/pending` - Xem yêu cầu đang chờ
- `PUT /api/requests/{id}/approve` - Duyệt yêu cầu
- `PUT /api/requests/{id}/reject` - Từ chối yêu cầu
- `GET /api/statistics/district` - Xem thống kê huyện

---

### 4. WardAdmin – Cán bộ Xã/Phường

**Quyền hạn:**
- Quản lý hộ khẩu trong xã
- Quản lý nhân khẩu trong xã
- Nhận và xử lý yêu cầu từ công dân
- Tạo biến động dân cư
- Duyệt/từ chối yêu cầu cấp xã

**Flow nghiệp vụ:**
1. **Đăng nhập** vào hệ thống
2. **Dashboard xã**
   - Xem thống kê xã (số hộ khẩu, nhân khẩu, yêu cầu)
   - Theo dõi số liệu trong phạm vi xã
3. **Thêm hộ khẩu**
   - Tạo hộ khẩu mới
   - Cập nhật thông tin hộ khẩu
   - Quản lý trạng thái hộ khẩu (Active, Transferred, Closed)
4. **Thêm nhân khẩu**
   - Thêm nhân khẩu vào hộ khẩu
   - Cập nhật thông tin nhân khẩu
   - Quản lý trạng thái nhân khẩu (Living, Transferred, Dead)
5. **Nhận yêu cầu người dân**
   - Xem danh sách yêu cầu từ công dân
   - Xem chi tiết từng yêu cầu
6. **Duyệt/Từ chối yêu cầu**
   - Duyệt các yêu cầu hợp lệ và đơn giản
   - Từ chối yêu cầu không đủ điều kiện
   - Chuyển tiếp yêu cầu phức tạp lên cấp huyện
   - Ghi nhận người xử lý và thời gian xử lý
7. **Tạo biến động dân cư**
   - Ghi nhận các biến động: Sinh, Tử, Kết hôn, Ly hôn, Chuyển đến, Chuyển đi, Tạm trú, Tạm vắng
   - Cập nhật thông tin hộ khẩu và nhân khẩu theo biến động

**API sử dụng:**
- `POST /api/auth/login` - Đăng nhập
- `POST /api/households` - Tạo hộ khẩu
- `PUT /api/households/{id}` - Cập nhật hộ khẩu
- `POST /api/citizens` - Thêm nhân khẩu
- `PUT /api/citizens/{id}` - Cập nhật nhân khẩu
- `GET /api/requests/pending` - Xem yêu cầu đang chờ
- `PUT /api/requests/{id}/approve` - Duyệt yêu cầu
- `PUT /api/requests/{id}/reject` - Từ chối yêu cầu
- `POST /api/changes` - Tạo biến động dân cư
- `GET /api/statistics/ward` - Xem thống kê xã

---

### 5. Citizen – Người dân (User Portal)

**Quyền hạn:**
- Xem thông tin cá nhân
- Gửi yêu cầu hành chính
- Theo dõi trạng thái hồ sơ

**Flow nghiệp vụ:**
1. **Đăng nhập** vào hệ thống
2. **Trang cá nhân**
   - Xem thông tin cá nhân (họ tên, ngày sinh, giới tính, CCCD, v.v.)
   - Xem thông tin hộ khẩu
3. **Gửi yêu cầu hành chính**
   - Tạo yêu cầu mới (cấp giấy khai sinh, khai tử, chuyển hộ khẩu, v.v.)
   - Điền nội dung yêu cầu
   - Gửi yêu cầu lên hệ thống
4. **Theo dõi trạng thái hồ sơ**
   - Xem danh sách các yêu cầu đã gửi
   - Xem trạng thái từng yêu cầu (Đang chờ, Đã duyệt, Bị từ chối)
   - Xem thông tin người xử lý và thời gian xử lý

**API sử dụng:**
- `POST /api/auth/login` - Đăng nhập
- `POST /api/auth/register` - Đăng ký tài khoản
- `GET /api/citizens/{id}` - Xem thông tin cá nhân
- `POST /api/requests` - Gửi yêu cầu hành chính
- `GET /api/requests/my` - Xem danh sách yêu cầu của mình

---

## Flow Duyệt Hồ Sơ Tổng Thể

Hệ thống hỗ trợ quy trình duyệt hồ sơ theo phân cấp:

```
Công dân gửi yêu cầu
        ↓
WardAdmin (Cán bộ Xã) xử lý
        ↓
    [Nếu đơn giản] → Duyệt/Từ chối ngay
        ↓
    [Nếu phức tạp] → Chuyển lên cấp trên
        ↓
DistrictAdmin (Cán bộ Huyện) xử lý
        ↓
    [Nếu đơn giản] → Duyệt/Từ chối ngay
        ↓
    [Nếu phức tạp] → Chuyển lên cấp trên
        ↓
ProvinceAdmin (Cán bộ Tỉnh) xử lý
        ↓
    Duyệt/Từ chối
```

**Quy tắc:**
- Yêu cầu đơn giản: WardAdmin có thể xử lý trực tiếp
- Yêu cầu phức tạp: Phải chuyển lên cấp trên để phê duyệt
- Mỗi yêu cầu chỉ có thể được xử lý một lần
- Hệ thống ghi nhận đầy đủ thông tin người xử lý và thời gian xử lý

---

## Các Loại Yêu Cầu Hành Chính

Hệ thống hỗ trợ các loại yêu cầu sau:
- Cấp giấy khai sinh
- Cấp giấy khai tử
- Chuyển hộ khẩu
- Tách hộ khẩu
- Nhập hộ khẩu
- Cấp lại giấy tờ
- Các yêu cầu hành chính khác

---

## Biến Động Dân Cư

Hệ thống quản lý các loại biến động:
- **Sinh** - Ghi nhận trẻ em mới sinh
- **Tử** - Ghi nhận người qua đời
- **Kết hôn** - Ghi nhận kết hôn
- **Ly hôn** - Ghi nhận ly hôn
- **Chuyển đến** - Người dân chuyển đến địa phương
- **Chuyển đi** - Người dân chuyển đi khỏi địa phương
- **Tạm trú** - Người dân tạm trú
- **Tạm vắng** - Người dân tạm vắng

Mỗi biến động được ghi nhận với:
- Thông tin công dân
- Loại biến động
- Hộ khẩu nguồn/đích (nếu có)
- Ngày biến động
- Lý do
- Người tạo
- Người phê duyệt
- Trạng thái (Pending, Approved, Rejected)

---

## Thống Kê và Báo Cáo

Hệ thống cung cấp thống kê theo từng cấp:

**Thống kê tỉnh:**
- Tổng số hộ khẩu
- Tổng số nhân khẩu
- Số yêu cầu đang chờ xử lý
- Số yêu cầu đã duyệt/từ chối
- Biến động dân cư trong tháng

**Thống kê huyện:**
- Tương tự thống kê tỉnh nhưng trong phạm vi huyện

**Thống kê xã:**
- Tương tự thống kê huyện nhưng trong phạm vi xã

---

## Bảo Mật và Phân Quyền

- Mỗi role chỉ có quyền truy cập các chức năng và dữ liệu trong phạm vi quản lý của mình
- SuperAdmin có quyền cao nhất, có thể quản lý toàn bộ hệ thống
- Các cán bộ chỉ có thể quản lý cấp dưới trực tiếp của mình
- Tất cả các thao tác đều được ghi nhận trong Audit Log để theo dõi và kiểm tra

---

## Liên Hệ

Để được hỗ trợ, vui lòng liên hệ:
- Email: support@civicregistry.com
- Hotline: 1900-xxxx
