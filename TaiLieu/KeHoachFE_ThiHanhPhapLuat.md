# Kế hoạch FE – Module Thi hành pháp luật

Ngày cập nhật: 09/10/2026

## 1. Mô hình nghiệp vụ và vai trò

```text
Đơn vị chủ trì
  Tạo kế hoạch → tạo đầu việc → phân công → bắt đầu thực hiện
                                                   ↓
Đơn vị được giao
  Xem việc được giao → cập nhật kết quả/minh chứng/khó khăn/kiến nghị → gửi báo cáo
                                                   ↓
Đơn vị chủ trì
  Theo dõi → đánh giá → yêu cầu bổ sung hoặc xác nhận → tổng hợp → chốt/hoàn thành
```

| Nhóm quyền | Phạm vi |
| --- | --- |
| Đơn vị chủ trì thi hành pháp luật | Quản lý kế hoạch, đầu việc, căn cứ pháp lý, phân công, đánh giá, tổng hợp và hoàn thành. Chỉ xem tiến độ do các đơn vị được giao gửi. |
| Đơn vị thực hiện thi hành pháp luật | Chỉ xem phân công thuộc đơn vị/cán bộ hiện tại; tạo, sửa, gửi báo cáo tiến độ và minh chứng. Không được sửa kế hoạch, đầu việc, phân công, đánh giá hoặc tổng hợp. |
| Quản trị hệ thống | Theo cơ chế SSA hiện có; có thể xem/quản trị toàn hệ thống theo quyền được gán. |

## 2. Hiện trạng BE và đánh giá quyền

- Controller/API đã có: `DanhSach`, `KeHoach`, `NoiDungKeHoach`, `PhanCong`, `TienDo`, `DanhGia`, `TongHop`.
- RoleAction cho 7 controller trên đã tồn tại trong database, đúng tên controller mà API kiểm tra.
- Chưa có RoleAction/API cho màn **Việc được giao của tôi**.
- Database hiện chưa có nhóm quyền và chưa có dòng `Permission` gán cho các RoleAction thi hành pháp luật. Vì vậy API sẽ trả `403` cho người dùng thông thường.
- API `TienDo` đã kiểm tra đơn vị/cán bộ được phân công khi tạo, sửa, gửi báo cáo. Tuy nhiên API danh sách kế hoạch/phân công hiện chỉ phù hợp cho đơn vị chủ trì, nên cần endpoint riêng cho đơn vị thực hiện.

## 3. Lộ trình thực hiện

### Pha 1 – Nền tảng quyền và API phân vai

1. Tạo hai nhóm quyền: `Đơn vị chủ trì THPL`, `Đơn vị thực hiện THPL`.
2. Gán Permission theo ma trận dưới đây; không tự gán nhóm vào tài khoản.
3. Thêm RoleAction và API `ViecDuocGiao` cho đơn vị thực hiện.
4. Bổ sung API hộp chờ đánh giá cho đơn vị chủ trì.
5. Bổ sung API file minh chứng: upload binary, danh sách, tải xuống, xóa theo quyền.

### Pha 2 – FE đơn vị chủ trì

1. Danh sách/tạo/sửa/chi tiết kế hoạch.
2. Tab căn cứ pháp lý, đầu việc, phân công, tiến độ, đánh giá, tổng hợp, lịch sử.
3. Thao tác bắt đầu thực hiện, tạo/chốt/mở lại báo cáo tổng hợp, hoàn thành kế hoạch.

### Pha 3 – FE đơn vị thực hiện

1. Màn `Việc được giao của tôi`.
2. Form cập nhật báo cáo tiến độ theo kỳ.
3. Tải minh chứng, sửa khi nháp/cần bổ sung, gửi báo cáo.
4. Hiển thị yêu cầu bổ sung và hạn bổ sung.

### Pha 4 – FE đánh giá và tổng hợp

1. Hộp báo cáo chờ đánh giá theo kế hoạch/đơn vị/kỳ.
2. Đánh giá đạt, không đạt, yêu cầu bổ sung.
3. Báo cáo tổng hợp, số liệu tổng quan và lịch sử chốt.

### Pha 5 – Kiểm thử

1. Tài khoản đơn vị chủ trì lập và phân công kế hoạch.
2. Tài khoản đơn vị thực hiện chỉ thấy việc của mình và gửi báo cáo.
3. Chủ trì đánh giá/yêu cầu bổ sung/chốt tổng hợp.
4. Kiểm thử quyền API trực tiếp với cả hai nhóm quyền.

## 4. Ma trận Permission cần tạo

| RoleAction | Chủ trì | Thực hiện |
| --- | --- | --- |
| `ThiHanhPhapLuat` | Index | Index |
| `ThiHanhPhapLuat.DanhSach` | Index | Không dùng (thay bằng Việc được giao) |
| `ThiHanhPhapLuat.KeHoach` | Index, Create, Edit, Delete | Không có |
| `ThiHanhPhapLuat.NoiDungKeHoach` | Index, Create, Edit, Delete | Không có |
| `ThiHanhPhapLuat.PhanCong` | Index, Create, Edit, Delete | Không có |
| `ThiHanhPhapLuat.TienDo` | Index | Index, Create, Edit |
| `ThiHanhPhapLuat.DanhGia` | Index, Approve | Không có |
| `ThiHanhPhapLuat.TongHop` | Index, Create, Approve | Không có |
| `ThiHanhPhapLuat.ViecDuocGiao` | Index | Index |

## 5. Route FE mục tiêu

```text
/thi-hanh-phap-luat/ke-hoach
/thi-hanh-phap-luat/ke-hoach/them-moi
/thi-hanh-phap-luat/ke-hoach/:id
/thi-hanh-phap-luat/viec-duoc-giao
/thi-hanh-phap-luat/viec-duoc-giao/:phanCongId
/thi-hanh-phap-luat/danh-gia
/thi-hanh-phap-luat/tong-hop
```

## 6. Điều kiện vận hành

- Sau khi migration được cập nhật database, quản trị hệ thống phải gán một trong hai nhóm quyền cho tài khoản thử nghiệm.
- Mỗi tài khoản đơn vị thực hiện cần có `DonViId`; nếu phân công theo cá nhân thì cần đúng `CanBoDuocGiaoId`.
- Status danh mục cần có các mã mà BE đang kiểm tra: `NHAP`, `DANG_THUC_HIEN`, `CHUA_THUC_HIEN`, `CHO_DANH_GIA`, `YEU_CAU_BO_SUNG`, `DAT`, `KHONG_DAT`, `DA_GUI`, `CAN_BO_SUNG`, `DA_XAC_NHAN`, `DA_CHOT`, `MO_LAI`, `DA_TONG_HOP`, `HOAN_THANH`.

## 7. Nhật ký triển khai

- 09/10/2026: Đã bổ sung API `GET /api/thi-hanh-phap-luat/viec-duoc-giao`. Tài khoản thường chỉ nhận đầu việc của đơn vị/cán bộ đang đăng nhập; tài khoản SSA xem toàn bộ để kiểm thử.
- 09/10/2026: Đã thêm RoleAction `ThiHanhPhapLuat.ViecDuocGiao`, cấp quyền Index cho hai nhóm THPL bằng migration mới.
- 09/10/2026: Đã thêm route và màn FE `/thi-hanh-phap-luat/viec-duoc-giao` để hiển thị đầu việc, hạn thực hiện, tiến độ và báo cáo gần nhất.
- 09/10/2026: Đã thêm form báo cáo tiến độ theo kỳ. Đơn vị thực hiện có thể tạo nháp, cập nhật và gửi báo cáo; FE tự xác định trạng thái `NHAP`, `DA_GUI` và `CHO_DANH_GIA` từ danh mục.
- 09/10/2026: Đã thêm hộp báo cáo chờ đánh giá cho đơn vị chủ trì, kèm thao tác đạt, không đạt và yêu cầu bổ sung có hạn xử lý.
- 09/10/2026: Đã thêm màn lập/chốt báo cáo tổng hợp theo kế hoạch; chỉ chốt khi BE xác nhận mọi đầu việc đã được đánh giá.
