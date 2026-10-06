# Đánh giá quy trình khảo sát và thi hành pháp luật

## Mục tiêu

Tài liệu này tổng hợp đánh giá hai quy trình nghiệp vụ trong `QT khao sat.docx` và `QT thi hanh PL.docx`, làm cơ sở thiết kế và phát triển ở giai đoạn sau.

## Định hướng kiến trúc

Hai quy trình cần được xây dựng thành hai phân hệ độc lập, không ghép vào `XayDungVanBanService`.

| Phân hệ | Phạm vi |
|---|---|
| `KhaoSatThiHanhPhapLuatService` | Thiết lập, phát hành, thu thập, rà soát và phân tích dữ liệu khảo sát. |
| `ThiHanhPhapLuatService` | Lập kế hoạch, phân công, cập nhật tiến độ, đánh giá và tổng hợp thi hành pháp luật. |

Hai phân hệ có thể liên kết qua khóa tùy chọn: một cuộc khảo sát có thể là đầu việc hoặc minh chứng của một kế hoạch thi hành pháp luật. Không dùng chung bảng nghiệp vụ.

`DanhMucService` quản lý đơn vị, lĩnh vực, văn bản, trạng thái và quy trình. `QuanTriHeThongService` quản lý RoleAction và Permission.

## Quy trình khảo sát tình hình thi hành pháp luật

### Luồng nghiệp vụ

1. Sở Tư pháp tạo cuộc khảo sát, xác định mục đích, phạm vi, lĩnh vực hoặc văn bản, thời gian và đơn vị chủ trì.
2. Xác định đối tượng khảo sát, biểu mẫu và phát hành khảo sát.
3. Đối tượng khảo sát nhập hoặc gửi dữ liệu; Sở Tư pháp theo dõi, đôn đốc và cảnh báo quá hạn.
4. Sở Tư pháp rà soát dữ liệu, yêu cầu bổ sung khi chưa đầy đủ hoặc không hợp lệ.
5. Tổng hợp, phân tích, đánh giá và xuất báo cáo kết quả.

### Nhóm dữ liệu cần có

- `CuocKhaoSat`: thông tin, phạm vi, thời hạn, đơn vị chủ trì, trạng thái.
- `DoiTuongKhaoSat`: đơn vị/cá nhân/nhóm đối tượng nhận khảo sát, hạn trả lời, trạng thái tham gia.
- `CauHoiKhaoSat`, `LuaChonTraLoi`: cấu trúc biểu mẫu và phiên bản biểu mẫu.
- `PhieuTraLoi`, `CauTraLoi`, `TepDinhKemKhaoSat`: dữ liệu phản hồi và minh chứng.
- `YeuCauBoSungKhaoSat`, `LichSuNhacViecKhaoSat`: yêu cầu cập nhật và lịch sử đôn đốc.
- `BaoCaoKhaoSat`: kết quả tổng hợp, nhận xét, kiến nghị và file báo cáo.

### Trạng thái đề xuất

Trạng thái cuộc khảo sát thuộc nhóm `KHAO_SAT_THI_HANH_PHAP_LUAT`:

```text
NHAP
PHAT_HANH
DANG_KHAO_SAT
CHO_RA_SOAT
YEU_CAU_BO_SUNG
HOAN_THANH
DONG
HUY
```

Trạng thái phiếu trả lời thuộc nhóm riêng `PHIEU_KHAO_SAT`:

```text
CHUA_TRA_LOI
DANG_NHAP
DA_GUI
CAN_BO_SUNG
DA_XAC_NHAN
```

### Điểm cần chốt trước khi xây dựng

- Chuẩn hóa mẫu import câu hỏi. Ưu tiên Excel có cấu trúc hoặc trình tạo câu hỏi trên phần mềm.
- Xác định phương thức tham gia: tài khoản nội bộ, đường dẫn có mã truy cập hoặc khảo sát ẩn danh.
- Xác định quyền sửa sau khi gửi, quy tắc mở lại phiếu và thời hạn đóng dữ liệu.
- Xác định loại câu hỏi cần hỗ trợ: một lựa chọn, nhiều lựa chọn, tự do, số liệu, bảng và file đính kèm.
- Xác định báo cáo bắt buộc, chỉ tiêu thống kê và định dạng xuất dữ liệu.

## Quy trình tổ chức thi hành pháp luật

### Luồng nghiệp vụ

1. Cơ quan chủ trì lập kế hoạch căn cứ Luật, Nghị định và các văn bản liên quan.
2. Phân rã kế hoạch thành nội dung công việc, chỉ tiêu, hạn hoàn thành và phân công đơn vị thực hiện.
3. Đơn vị được giao cập nhật tiến độ, kết quả, khó khăn, kiến nghị và tài liệu minh chứng.
4. Sở Tư pháp theo dõi, cảnh báo, đánh giá hoặc yêu cầu bổ sung.
5. Sở Tư pháp tổng hợp và chốt báo cáo kết quả thực hiện kế hoạch.

### Nhóm dữ liệu cần có

- `KeHoachThiHanhPhapLuat`: căn cứ pháp lý, phạm vi, thời gian, cơ quan chủ trì và trạng thái.
- `NoiDungKeHoach`: mục tiêu, đầu việc, chỉ tiêu, hạn hoàn thành và thứ tự thực hiện.
- `PhanCongThiHanh`: đơn vị/cán bộ nhận việc, vai trò, hạn thực hiện và mức ưu tiên.
- `BaoCaoTienDoThiHanh`: kết quả, tỷ lệ hoàn thành, khó khăn, kiến nghị và tài liệu minh chứng.
- `DanhGiaThiHanh`: kết quả đánh giá, nhận xét, yêu cầu bổ sung và lịch sử.
- `BaoCaoTongHopThiHanh`: số liệu đã chốt theo kỳ và tệp báo cáo.

### Trạng thái đề xuất

Trạng thái kế hoạch thuộc nhóm `KE_HOACH_THI_HANH_PHAP_LUAT`:

```text
NHAP
CHO_PHE_DUYET
DA_PHE_DUYET
DANG_THUC_HIEN
DA_TONG_HOP
HOAN_THANH
HUY
```

Trạng thái đầu việc thuộc nhóm `NOI_DUNG_THI_HANH_PHAP_LUAT`:

```text
CHUA_THUC_HIEN
DANG_THUC_HIEN
CHO_DANH_GIA
YEU_CAU_BO_SUNG
DAT
KHONG_DAT
QUA_HAN
```

### Điểm cần chốt trước khi xây dựng

- Thẩm quyền tạo, phê duyệt, điều chỉnh và hủy kế hoạch.
- Cách xác định chỉ tiêu, tỷ lệ hoàn thành và điều kiện đạt/không đạt.
- Chu kỳ cập nhật và chốt báo cáo: tháng, quý, sáu tháng hoặc năm.
- Quy tắc khóa dữ liệu sau khi chốt tổng hợp và việc mở lại dữ liệu.
- Danh sách cảnh báo tự động: chưa thực hiện, chưa nhập liệu, chậm tiến độ, quá hạn.
- Hình thức và cấu trúc báo cáo tổng hợp bắt buộc.

## Khuyến nghị triển khai

1. Hoàn thiện danh mục trạng thái theo từng nhóm trước khi tạo bảng nghiệp vụ.
2. Xây dựng quản lý kế hoạch thi hành pháp luật trước, vì đây là nguồn dữ liệu đầu việc và tiến độ.
3. Xây dựng khảo sát như một phân hệ độc lập, có liên kết tùy chọn tới kế hoạch hoặc nội dung kế hoạch.
4. Thiết kế background job gửi cảnh báo và nhắc việc; không chỉ kiểm tra khi người dùng truy cập màn hình.
5. Thiết kế RoleAction riêng cho từng màn hình nghiệp vụ; controller chỉ gọi lại `QuanTriHeThongService` để kiểm tra quyền.
