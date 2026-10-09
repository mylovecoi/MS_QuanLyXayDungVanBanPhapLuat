# Kế hoạch FE/BE – Khảo sát tình hình thi hành pháp luật

## 1. Mục tiêu và nguyên tắc nghiệp vụ

Phần mềm quản lý hồ sơ, file và dữ liệu đã đọc từ file. Hoạt động soạn phiếu và thực hiện khảo sát diễn ra bên ngoài phần mềm.

1. Đơn vị chủ trì lập cuộc khảo sát, quản lý nhóm đối tượng và phân công đơn vị.
2. Đơn vị soạn phiếu bên ngoài tải file Word mẫu phiếu lên theo từng cuộc khảo sát. Hệ thống lưu file, đọc và lưu câu hỏi/lựa chọn để đối chiếu/tổng hợp; không xây dựng kho mẫu và không phát hành mẫu trên PM.
3. Đơn vị được khảo sát thực hiện khảo sát bên ngoài, sau đó tải file kết quả và khai báo phần mềm/nguồn dữ liệu đã sử dụng.
4. Hệ thống đọc file kết quả, lưu câu trả lời và lỗi dữ liệu. Đơn vị chủ trì rà soát, xác nhận hoặc yêu cầu bổ sung.
5. Chỉ dữ liệu đã xác nhận mới được đưa vào dashboard, snapshot, chốt và xuất báo cáo Word.

## 2. Vai trò

| Vai trò | Phạm vi chức năng |
| --- | --- |
| Đơn vị chủ trì | Tạo cuộc khảo sát, phân công, rà soát, dashboard, tổng hợp/chốt báo cáo. |
| Đơn vị soạn phiếu | Chỉ xem nhiệm vụ được giao, tải và thay thế file mẫu phiếu theo cuộc khảo sát; xem kết quả đọc file. |
| Đơn vị thực hiện khảo sát | Chỉ xem nhiệm vụ được giao, tải file kết quả, khai báo phần mềm/nguồn dữ liệu, xem lỗi và tải lại. |
| SSA | Toàn quyền để cấu hình và kiểm thử dev. |

## 3. Màn hình FE và route

| Giai đoạn | Màn hình | Route |
| --- | --- | --- |
| 1 | Danh sách cuộc khảo sát | `/khao-sat-thi-hanh-phap-luat/cuoc-khao-sat` |
| 1 | Chi tiết cuộc khảo sát: Thông tin / Mẫu phiếu / Đơn vị khảo sát | `/khao-sat-thi-hanh-phap-luat/cuoc-khao-sat/:id` |
| 2 | Tải mẫu phiếu và xem dữ liệu đã đọc | `/khao-sat-thi-hanh-phap-luat/cuoc-khao-sat/:id/mau-phieu` |
| 3 | Nhập kết quả, metadata phần mềm, lỗi import | `/khao-sat-thi-hanh-phap-luat/nhap-ket-qua` |
| 4 | Rà soát/xác nhận/yêu cầu bổ sung | `/khao-sat-thi-hanh-phap-luat/ra-soat` |
| 5 | Dashboard tiến độ và báo cáo | `/khao-sat-thi-hanh-phap-luat/dashboard`, `/khao-sat-thi-hanh-phap-luat/bao-cao` (chọn cuộc khảo sát) |

## 4. Dữ liệu đọc từ file

- File mẫu `.docx`: lưu `MauPhieuKhaoSats`, `CauHoiMauPhieus`, `LuaChonTraLois`, `CauHoiThongKes`.
- File kết quả `.docx`: lưu `PhieuNopKhaoSats`, `CauTraLoiKhaoSats`, `LoiImportKhaoSats`.
- FE bắt buộc hiển thị số dòng/câu hỏi đọc được, xem trước dữ liệu và lỗi import trước khi xác nhận.

## 5. Bổ sung BE cần thiết

1. Bổ sung metadata nguồn kết quả: tên phần mềm, phiên bản, đường dẫn hệ thống nguồn và ghi chú.
2. Bổ sung API chi tiết cuộc khảo sát cho ba tab và các API danh sách mẫu/đơn vị được giao.
3. Bổ sung kiểm soát dữ liệu theo đơn vị chủ trì/đơn vị được giao; không chỉ dừng ở RoleAction.
4. Bổ sung API tải file gốc mẫu phiếu/kết quả và lịch sử phiên bản.

## 6. Trình tự triển khai

- [~] P1: Danh sách và chi tiết cuộc khảo sát (ba tab), API dữ liệu nền.
- [~] P2: Upload/đọc mẫu phiếu theo cuộc khảo sát và xem dữ liệu đã lưu; không phát hành hoặc dùng kho mẫu.
- [~] P3: Upload/đọc file kết quả, metadata phần mềm, lỗi import và tải lại.
- [~] P4: Rà soát, xác nhận/yêu cầu bổ sung.
- [~] P5: Dashboard, snapshot, chốt và xuất báo cáo Word.
- [ ] P6: Áp dụng migration, kiểm thử với SSA và các tài khoản đơn vị.
