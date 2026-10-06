# Hạng mục cần hoàn thiện: Khảo sát thi hành pháp luật

## Phạm vi đã thực hiện

- Tạo `KhaoSatThiHanhPhapLuatService`, DbContext schema `kspl` và migration.
- Đọc mẫu Word `.docx` theo bảng chuẩn để sinh câu hỏi và đáp án.
- Hỗ trợ nhiều mẫu phiếu theo nhóm đối tượng trong một cuộc khảo sát.
- Tiếp nhận file kết quả Word, đọc số lượng/tổng số trả lời và lưu dữ liệu chuẩn hóa.
- Thống kê trực tiếp theo câu hỏi/đáp án.

## Quy ước file kết quả Word

File Word dùng bảng có các cột: `Mã câu hỏi`, `Nội dung`, `Loại`, `Mã đáp án`, `Đáp án`, `Số lượng`, `Tổng số trả lời`.

Tỷ lệ đáp án được tính bằng:

```text
Tổng số lượng đáp án / Tổng số trả lời của câu hỏi x 100
```

Khảo sát thực hiện bên ngoài hệ thống. Phần mềm chỉ quản lý mẫu, đọc file kết quả đính kèm, lưu số liệu và thống kê; không quản lý vòng đời phiếu của từng người trả lời.

## Hạng mục cần làm tiếp

### 1. Báo cáo khảo sát đã chốt

- Thêm `BaoCaoKhaoSat` và `BaoCaoKhaoSatChiTiet`.
- Tạo snapshot số liệu theo câu hỏi/đáp án/lần chốt.
- Lưu nhận xét, tồn tại, nguyên nhân và kiến nghị.
- Mở lại có lý do và chốt lại theo phiên, không ghi đè snapshot cũ.
- Xuất Word/Excel theo mẫu báo cáo được phê duyệt.

### 2. Hoàn thiện import Word

- Kiểm tra tổng số lượng đáp án của câu một lựa chọn phải bằng tổng số trả lời.
- Cho phép tổng số lượng lớn hơn tổng số trả lời đối với câu nhiều lựa chọn.
- Hỗ trợ câu tự do và số liệu; câu số phải lưu vào trường số.
- Ghi rõ lỗi theo dòng/câu hỏi/đáp án và hiển thị cho người tải file.
- Xác minh file phù hợp đúng mẫu Word đã phát hành.

### 3. Quản lý cuộc khảo sát

- Danh sách, lọc, phân trang, chi tiết và timeline.
- Sửa/xóa mềm khi khảo sát còn ở trạng thái được phép.
- Phát hành/đóng/hủy khảo sát theo trạng thái từ `DanhMucService`.
- Liên kết tùy chọn với kế hoạch hoặc nội dung kế hoạch thi hành pháp luật.

### 4. Tích hợp quản trị hệ thống

- Seed RoleAction riêng cho từng controller khảo sát.
- Khi `QuanTriHeThongService` hoàn thiện permission checker, thay `[Authorize]` bằng kiểm tra quyền chức năng theo RoleAction.
- Bổ sung `ICurrentUserContext` để lưu đúng người/đơn vị tạo, import, rà soát và chốt báo cáo.

### 5. Dashboard và cảnh báo

- Dashboard tổng số file kết quả, file import thành công/lỗi và tỷ lệ dữ liệu theo nhóm đối tượng.
- Chỉ xây job nền cảnh báo nếu nghiệp vụ sau này cần theo dõi hạn nhận file kết quả.

## Thứ tự khuyến nghị

1. Chốt cấu trúc và mẫu báo cáo đầu ra.
2. Xây báo cáo chốt/snapshot và xuất file.
3. Hoàn thiện kiểm tra import Word.
4. Hoàn thiện quản lý khảo sát và timeline.
5. Tích hợp phân quyền sau khi `QuanTriHeThongService` sẵn sàng.
6. Làm dashboard/cảnh báo nếu còn yêu cầu.
