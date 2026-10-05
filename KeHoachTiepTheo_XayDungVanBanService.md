# Kế hoạch tiếp theo XayDungVanBanService

## 1. Mục tiêu

Hoàn thiện `XayDungVanBanService` để quản lý hai luồng xây dựng văn bản QPPL cấp tỉnh:

- Quyết định UBND tỉnh.
- Nghị quyết HĐND tỉnh.

Service dùng chung một mô hình dữ liệu và phân nhánh bằng `DanhMucVanBanId` cùng `QuyTrinhSoanThaoId`. Không tách service hoặc CSDL riêng theo loại văn bản.

## 2. Trạng thái đã hoàn thành

- Đã tạo `XayDungVanBanService` và thêm vào solution.
- Đã tạo migration `InitialXayDungVanBan` và áp dụng vào CSDL đích.
- Đã có mô hình hồ sơ gốc, bộ hồ sơ theo bước/lần xử lý, tài liệu kế thừa, timeline và bảng chi tiết từng nghiệp vụ.
- Đã tạo `ICurrentUserContext` và tám controller tách riêng theo RoleAction Detail.
- Đã tạo script seed RoleAction tại `BE/services/QuanTriHeThongService/Infrastructure/Persistence/Data/seed_xay_dung_van_ban_role_actions.sql`.

## 3. Nguyên tắc bắt buộc của PM

- Mỗi `RoleAction` có `PhanLoai = Detail` phải có đúng một Form và một Controller/API riêng.
- Không gom nghiệp vụ Detail khác nhau vào cùng controller nếu cần cấu hình quyền độc lập.
- Backend kiểm tra đồng thời quyền chức năng và quyền dữ liệu trước mỗi thao tác.
- Frontend route/form phải khớp với RoleAction; controller/action backend phải khớp dữ liệu RoleAction.
- Không hard-code ID loại văn bản, quy trình hoặc bước quy trình.
- Timeline là append-only: không sửa/xóa lịch sử; mọi tạo, kế thừa, thay file, chuyển bước và trả lại đều ghi lịch sử.

## 4. Bước chuẩn bị trên máy mới

1. Clone repository từ GitHub và checkout branch cần làm việc.
2. Cài .NET SDK 10, Node.js và package manager dùng cho frontend.
3. Khai báo `ConnectionStrings__DefaultConnection` bằng biến môi trường hoặc secret store; không commit mật khẩu CSDL vào Git.
4. Restore và build:

```powershell
dotnet restore .\MS_QuanLyXayDungVanBanPhapLuat.slnx --configfile .\NuGet.Config
dotnet build .\MS_QuanLyXayDungVanBanPhapLuat.slnx --no-restore
```

5. Kiểm tra migration đã có trên CSDL đích:

```powershell
cd .\BE\services\XayDungVanBanService
dotnet ef migrations list --context XayDungVanBanDbContext
```

6. Chạy script seed RoleAction, sau đó cấp quyền cho các nhóm người dùng qua `QuanTriHeThongService`.

## 5. RoleAction và controller bắt buộc

| RoleAction | Controller | Form/route frontend |
|---|---|---|
| `VanBanQPPL.XayDungVanBan.DanhSach` | `XayDungVanBanDanhSachController` | `/xay-dung-van-ban/danh-sach` |
| `VanBanQPPL.XayDungVanBan.SoanThao` | `XayDungVanBanSoanThaoController` | `/xay-dung-van-ban/soan-thao` |
| `VanBanQPPL.XayDungVanBan.TrinhThamDinh` | `XayDungVanBanTrinhThamDinhController` | `/xay-dung-van-ban/trinh-tham-dinh` |
| `VanBanQPPL.XayDungVanBan.ThamDinh` | `XayDungVanBanThamDinhController` | `/xay-dung-van-ban/tham-dinh` |
| `VanBanQPPL.XayDungVanBan.TrinhPheDuyet` | `XayDungVanBanTrinhPheDuyetController` | `/xay-dung-van-ban/trinh-phe-duyet` |
| `VanBanQPPL.XayDungVanBan.YKienUbnd` | `XayDungVanBanYKienUbndController` | `/xay-dung-van-ban/y-kien-ubnd` |
| `VanBanQPPL.XayDungVanBan.ThamTraHdnd` | `XayDungVanBanThamTraHdndController` | `/xay-dung-van-ban/tham-tra-hdnd` |
| `VanBanQPPL.XayDungVanBan.BanHanh` | `XayDungVanBanBanHanhController` | `/xay-dung-van-ban/ban-hanh` |

## 6. Thứ tự triển khai backend

### 6.1. Phân quyền và danh mục

1. Tích hợp permission checker từ `QuanTriHeThongService`.
2. Áp dụng quyền `Index`, `Create`, `Edit`, `Delete`, `Approve`, `Public` cho từng Detail.
3. Áp dụng data-scope theo đơn vị chủ trì, người phụ trách, Sở Tư pháp, UBND/HĐND và SSA/Admin.
4. Tích hợp client đọc `DanhMucVanBan`, `DanhMucQuyTrinhSoanThao`, bước và chuyển bước từ `DanhMucService`.
5. Validate `DanhMucVanBanId` thuộc quy trình, quy trình đang hoạt động và đúng loại nghiệp vụ.

### 6.2. API nền

1. Danh sách, lọc, phân trang và chi tiết hồ sơ.
2. Timeline toàn hồ sơ và theo từng bộ hồ sơ.
3. Danh sách hành động khả dụng theo quyền, bước và trạng thái.
4. Tạo/sửa/xóa mềm hồ sơ ở giai đoạn được phép.
5. Upload, quản lý phiên bản và liên kết tài liệu.

### 6.3. API nghiệp vụ theo bước

1. Soạn thảo: tạo hồ sơ, file, phối hợp, góp ý.
2. Trình thẩm định: kế thừa tài liệu, bổ sung tờ trình, gửi/gửi lại.
3. Thẩm định: tiếp nhận, báo cáo thẩm định, đạt/chưa đạt, yêu cầu bổ sung.
4. Trình phê duyệt: lưu hồ sơ trình UBND/HĐND và tiến độ nhận từ ngoài hệ thống.
5. Ý kiến UBND: tổng hợp ý kiến, giải trình, bổ sung hồ sơ.
6. Thẩm tra HĐND: chỉ bật khi quy trình có bước này.
7. Ban hành: số văn bản, ngày ban hành, người ký, file kết quả và hoàn thành.

Mọi thao tác chuyển bước phải chạy trong transaction, kiểm tra trạng thái hiện tại và ghi timeline trong cùng transaction.

## 7. Frontend

1. Tạo tám route/form đúng bảng RoleAction.
2. Làm màn danh sách, chi tiết và timeline trước.
3. Hiển thị nút thao tác theo API hành động khả dụng; không suy đoán quyền ở client.
4. Chỉ hiển thị phần thẩm tra HĐND khi quy trình hồ sơ có bước thẩm tra.
5. Mỗi form tải và hiển thị đúng tài liệu kế thừa cùng phiên bản tài liệu được sử dụng tại bước đó.

## 8. Kiểm thử nghiệm thu

- Quyết định UBND đi đủ sáu bước và không thể vào bước thẩm tra HĐND.
- Nghị quyết HĐND đi đủ bảy bước, bao gồm thẩm tra HĐND.
- Hồ sơ bị yêu cầu bổ sung có thể tạo lần xử lý mới, kế thừa tài liệu và giữ được lịch sử phiên bản cũ.
- Người không thuộc phạm vi dữ liệu hoặc không có quyền chức năng không thể xem/sửa/xử lý hồ sơ.
- SSA/Admin có quyền theo chính sách hệ thống.
- Timeline hiển thị theo các bước của quy trình danh mục và có đầy đủ sự kiện thực tế.
- Build backend, build frontend và kiểm tra API tích hợp đều thành công trước khi merge/push.

## 9. Thứ tự commit khuyến nghị

1. `feat(xay-dung-van-ban): add permission and catalogue integration`
2. `feat(xay-dung-van-ban): add core dossier and timeline APIs`
3. `feat(xay-dung-van-ban): add drafting and appraisal workflow`
4. `feat(xay-dung-van-ban): add ubnd hdnd and promulgation workflow`
5. `feat(xay-dung-van-ban): add role based frontend forms`
6. `test(xay-dung-van-ban): add end to end workflow coverage`
