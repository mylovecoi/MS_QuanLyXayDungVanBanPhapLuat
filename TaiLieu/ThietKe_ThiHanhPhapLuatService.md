# Thiết kế ThiHanhPhapLuatService

## 1. Phạm vi

`ThiHanhPhapLuatService` là phân hệ độc lập quản lý tổ chức thi hành pháp luật. Phân hệ không dùng chung bảng nghiệp vụ với `XayDungVanBanService`; chỉ tham chiếu bằng ID đến danh mục, đơn vị, cán bộ và văn bản do service khác quản lý.

Luồng được triển khai theo quy trình nguồn:

```text
Tạo kế hoạch -> phân rã/phân công -> cập nhật tiến độ -> đánh giá -> tổng hợp/chốt báo cáo
```

Quy trình nguồn không có bước phê duyệt kế hoạch. Vì vậy không tạo trạng thái `CHO_PHE_DUYET`, `DA_PHE_DUYET`, RoleAction hay controller phê duyệt kế hoạch trong phạm vi này.

## 2. Trạng thái

Trạng thái được đọc theo mã từ `DanhMucService`, không hard-code ID.

| Nhóm | Mã |
|---|---|
| `KE_HOACH_THI_HANH_PHAP_LUAT` | `NHAP`, `DANG_THUC_HIEN`, `DA_TONG_HOP`, `HOAN_THANH`, `HUY` |
| `NOI_DUNG_THI_HANH_PHAP_LUAT` | `CHUA_THUC_HIEN`, `DANG_THUC_HIEN`, `CHO_DANH_GIA`, `YEU_CAU_BO_SUNG`, `DAT`, `KHONG_DAT`, `QUA_HAN` |
| `BAO_CAO_TIEN_DO_THI_HANH` | `NHAP`, `DA_GUI`, `CAN_BO_SUNG`, `DA_XAC_NHAN` |
| `BAO_CAO_TONG_HOP_THI_HANH` | `NHAP`, `DA_CHOT`, `MO_LAI`, `HUY` |

## 3. Mô hình dữ liệu

Schema SQL Server: `thpl`. Các bảng có audit fields `Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `IsDeleted`, trừ bảng lịch sử chỉ append.

| Bảng | Chức năng |
|---|---|
| `KeHoachThiHanhPhapLuats` | Kế hoạch, thời gian, đơn vị chủ trì, trạng thái |
| `KeHoachCanCuPhapLys` | Căn cứ pháp lý |
| `NoiDungKeHoachs` | Đầu việc, chỉ tiêu, hạn, trạng thái |
| `PhanCongThiHanhs` | Đơn vị/cán bộ nhận việc |
| `BaoCaoTienDoThiHanhs` | Lần cập nhật tiến độ |
| `TepDinhKemThiHanhs` | Minh chứng và phiên bản tệp |
| `DanhGiaThiHanhs` | Đánh giá của Sở Tư pháp |
| `YeuCauBoSungThiHanhs` | Nội dung/hạn yêu cầu bổ sung |
| `BaoCaoTongHopThiHanhs` | Báo cáo theo kỳ |
| `BaoCaoTongHopChiTiets` | Snapshot dùng khi chốt |
| `LichSuXuLyThiHanhs` | Timeline append-only |
| `LichSuNhacViecThiHanhs` | Nhật ký job cảnh báo |

Ràng buộc chính:

- `MaKeHoach` là duy nhất.
- `MaNoiDung` là duy nhất trong một kế hoạch.
- `(KeHoachId, KyBaoCao)` là duy nhất tại báo cáo tổng hợp.
- Chốt báo cáo tạo snapshot theo `LanChot`. Số liệu đã chốt không bị thay đổi bởi cập nhật tiến độ về sau; mở lại và chốt lần nữa tạo snapshot mới.

## 4. Phân quyền và controller

Mỗi RoleAction `Detail` tương ứng một form và controller riêng.

| RoleAction | Route frontend | Controller | Quyền |
|---|---|---|---|
| `VanBanQPPL.ThiHanhPhapLuat.DanhSach` | `/thi-hanh-phap-luat/danh-sach` | `ThiHanhPhapLuatDanhSachController` | `Index` |
| `VanBanQPPL.ThiHanhPhapLuat.KeHoach` | `/thi-hanh-phap-luat/ke-hoach` | `ThiHanhPhapLuatKeHoachController` | `Index`, `Create`, `Edit`, `Delete` |
| `VanBanQPPL.ThiHanhPhapLuat.NoiDungKeHoach` | `/thi-hanh-phap-luat/noi-dung-ke-hoach` | `ThiHanhPhapLuatNoiDungKeHoachController` | `Index`, `Create`, `Edit`, `Delete` |
| `VanBanQPPL.ThiHanhPhapLuat.PhanCong` | `/thi-hanh-phap-luat/phan-cong` | `ThiHanhPhapLuatPhanCongController` | `Index`, `Create`, `Edit`, `Delete` |
| `VanBanQPPL.ThiHanhPhapLuat.TienDo` | `/thi-hanh-phap-luat/tien-do` | `ThiHanhPhapLuatTienDoController` | `Index`, `Create`, `Edit`, `Delete` |
| `VanBanQPPL.ThiHanhPhapLuat.DanhGia` | `/thi-hanh-phap-luat/danh-gia` | `ThiHanhPhapLuatDanhGiaController` | `Index`, `Approve` |
| `VanBanQPPL.ThiHanhPhapLuat.TongHop` | `/thi-hanh-phap-luat/tong-hop` | `ThiHanhPhapLuatTongHopController` | `Index`, `Create`, `Approve`, `Public` |
| `VanBanQPPL.ThiHanhPhapLuat.Dashboard` | `/thi-hanh-phap-luat/dashboard` | `ThiHanhPhapLuatDashboardController` | `Index` |

Backend phải kiểm tra cả quyền chức năng qua `QuanTriHeThongService` và phạm vi dữ liệu (đơn vị chủ trì, đơn vị/cán bộ được giao, Sở Tư pháp, SSA/Admin).

## 5. API mục tiêu

- `GET /api/thi-hanh-phap-luat/danh-sach`, `GET /{id}`, `GET /{id}/timeline`.
- CRUD tại `/ke-hoach`, `/noi-dung-ke-hoach`, `/phan-cong`.
- Cập nhật/gửi tiến độ tại `/tien-do`.
- Đánh giá, yêu cầu bổ sung tại `/danh-gia`.
- Tạo, chốt, mở lại và xuất báo cáo tại `/tong-hop`.

Thao tác đổi trạng thái chạy trong transaction và thêm một dòng `LichSuXuLyThiHanhs` trong cùng transaction.

## 6. Lộ trình thực hiện

1. Tạo project, DbContext, entity, migration và seed trạng thái/RoleAction.
2. Làm danh sách, chi tiết và timeline cùng data-scope.
3. Làm kế hoạch, nội dung và phân công.
4. Làm tiến độ, minh chứng và đánh giá.
5. Làm tổng hợp, snapshot và chốt/mở lại báo cáo.
6. Thêm job nền cảnh báo, dashboard, xuất báo cáo và kiểm thử luồng.
