# Kế hoạch làm việc FE module xây dựng văn bản

Tài liệu này dùng làm căn cứ triển khai module FE `xay-dung-van-ban` theo tài liệu `QT xay dung.docx`. Module phục vụ chung cho hai quy trình:

- Quyết định UBND tỉnh
- Nghị quyết HĐND tỉnh

## 1. Mục tiêu

Xây dựng module FE quản lý toàn bộ quy trình xây dựng văn bản QPPL cấp tỉnh, từ tạo hồ sơ dự thảo, gửi thẩm định, thẩm định, trình UBND, lấy ý kiến, trình HĐND đối với nghị quyết và cập nhật kết quả ban hành.

Module FE được tổ chức theo đúng nguyên tắc mỗi service một thư mục:

```text
FE/src/features/xay-dung-van-ban/
  api/
  pages/
  components/
  routes.jsx
```

## 2. Giai đoạn 1: Nền tảng module

### Công việc

- Tạo cấu trúc thư mục `FE/src/features/xay-dung-van-ban`.
- Khai báo route trong `routes.jsx`.
- Kết nối route vào `appFeatureRoutes`.
- Tạo API client cơ bản:
  - Lấy danh sách hồ sơ.
  - Lấy chi tiết hồ sơ.
  - Tạo hồ sơ.
  - Cập nhật hồ sơ.
  - Xóa hồ sơ.
  - Chuyển bước xử lý.

### Kết quả cần đạt

Có khung module FE riêng, sẵn sàng mở rộng các màn nghiệp vụ của quy trình xây dựng văn bản.

## 3. Giai đoạn 2: Danh sách và hồ sơ gốc

### Màn hình

| STT | Màn hình | Route đề xuất |
| --- | --- | --- |
| 1 | Danh sách hồ sơ xây dựng văn bản | `/admin/xay-dung-van-ban/ho-so` |
| 2 | Tạo mới hồ sơ | `/admin/xay-dung-van-ban/ho-so/them-moi` |
| 3 | Chi tiết hồ sơ | `/admin/xay-dung-van-ban/ho-so/:id` |
| 4 | Cập nhật hồ sơ | `/admin/xay-dung-van-ban/ho-so/:id/chinh-sua` |

### Chức năng

- Xem danh sách hồ sơ.
- Tìm kiếm hồ sơ.
- Lọc theo loại văn bản, trạng thái, cơ quan chủ trì, khoảng thời gian.
- Phân trang.
- Xem chi tiết.
- Sửa hồ sơ.
- Xóa hồ sơ khi chưa phát sinh nghiệp vụ.
- Kết xuất danh sách.

### Kết quả cần đạt

Quản lý được hồ sơ gốc dùng chung cho cả Quyết định UBND tỉnh và Nghị quyết HĐND tỉnh.

## 4. Giai đoạn 3: Tổ chức soạn thảo và lấy ý kiến góp ý

### Route

- `/admin/xay-dung-van-ban/quyet-dinh/soan-thao/:id`
- `/admin/xay-dung-van-ban/nghi-quyet/soan-thao/:id`

### Chức năng

- Cập nhật thông tin dự thảo.
- Cập nhật cơ quan chủ trì, đơn vị phối hợp, người phụ trách.
- Cập nhật thời gian dự kiến xây dựng và thời gian hoàn thành.
- Đính kèm tài liệu:
  - Dự thảo văn bản.
  - Bản tổng hợp ý kiến tiếp thu, giải trình.
  - Báo cáo tổng kết việc thi hành pháp luật hoặc đánh giá thực trạng quan hệ xã hội.
  - Bản đánh giá thủ tục hành chính, phân cấp, nhiệm vụ, quyền hạn được phân cấp nếu có.
- Lưu ý kiến góp ý.
- Chuyển sang bước gửi thẩm định.

### Kết quả cần đạt

Hoàn thiện bước 1 của cả hai quy trình.

## 5. Giai đoạn 4: Gửi hồ sơ thẩm định

### Route

- `/admin/xay-dung-van-ban/quyet-dinh/trinh-tham-dinh/:id`
- `/admin/xay-dung-van-ban/nghi-quyet/trinh-tham-dinh/:id`

### Chức năng

- Kế thừa thông tin và tài liệu từ hồ sơ soạn thảo.
- Bổ sung tờ trình đề nghị thẩm định.
- Bổ sung bản so sánh, thuyết minh nội dung dự thảo.
- Đính kèm tài liệu trình thẩm định.
- Gửi hồ sơ đến Sở Tư pháp.
- Cập nhật, bổ sung và gửi lại nếu hồ sơ bị yêu cầu bổ sung.

### Kết quả cần đạt

Xử lý được bước trình thẩm định cho cả Quyết định và Nghị quyết.

## 6. Giai đoạn 5: Thẩm định văn bản

### Route

- `/admin/xay-dung-van-ban/quyet-dinh/tham-dinh/:id`
- `/admin/xay-dung-van-ban/nghi-quyet/tham-dinh/:id`

### Chức năng

- Sở Tư pháp tiếp nhận hồ sơ.
- Cập nhật hình thức thẩm định:
  - Tự thẩm định.
  - Hội đồng thẩm định.
  - Cuộc họp thẩm định.
  - Lấy ý kiến thẩm định bằng văn bản.
- Đính kèm báo cáo kết quả thẩm định.
- Gửi kết quả thẩm định cho cơ quan chủ trì.
- Yêu cầu bổ sung nếu hồ sơ chưa đạt.

### Kết quả cần đạt

Hoàn thiện nghiệp vụ thẩm định văn bản.

## 7. Giai đoạn 6: Trình UBND tỉnh

### Route

- `/admin/xay-dung-van-ban/quyet-dinh/trinh-ubnd/:id`
- `/admin/xay-dung-van-ban/nghi-quyet/trinh-ubnd/:id`

### Chức năng

- Lập hồ sơ trình UBND tỉnh.
- Đính kèm:
  - Tờ trình.
  - Dự thảo văn bản.
  - Báo cáo thẩm định.
  - Báo cáo tiếp thu, giải trình ý kiến thẩm định.
  - Các tài liệu liên quan.
- Cập nhật trạng thái đã trình UBND.
- Sở Tư pháp theo dõi tiến độ.

### Kết quả cần đạt

Lưu trữ được hồ sơ đã trình UBND tỉnh.

## 8. Giai đoạn 7: Ý kiến thành viên UBND tỉnh

### Route

- `/admin/xay-dung-van-ban/quyet-dinh/y-kien-ubnd/:id`
- `/admin/xay-dung-van-ban/nghi-quyet/y-kien-ubnd/:id`

### Chức năng

- Cập nhật tổng hợp ý kiến thành viên UBND tỉnh.
- Đính kèm file ý kiến.
- Ghi nhận kết quả:
  - Còn ý kiến khác nhau.
  - Không có ý kiến khác nhau.
- Nếu còn ý kiến khác nhau: cập nhật giải trình, bổ sung hồ sơ.
- Nếu đồng ý:
  - Quyết định: chuyển ban hành.
  - Nghị quyết: chuyển trình HĐND thẩm tra.

### Kết quả cần đạt

Xử lý được nhánh nghiệp vụ theo kết quả ý kiến thành viên UBND tỉnh.

## 9. Giai đoạn 8: Trình HĐND tỉnh thẩm tra dự thảo nghị quyết

### Route

- `/admin/xay-dung-van-ban/nghi-quyet/trinh-hdnd-tham-tra/:id`

### Chức năng

- Lập hồ sơ trình HĐND tỉnh thẩm tra.
- Đính kèm hồ sơ trình.
- Cập nhật báo cáo thẩm tra của HĐND.
- Cập nhật ý kiến thảo luận của HĐND.
- Cập nhật hồ sơ hoàn thiện sau thẩm tra.
- Chuyển bước thông qua và ban hành.

### Kết quả cần đạt

Hoàn thiện bước riêng của quy trình Nghị quyết HĐND tỉnh.

## 10. Giai đoạn 9: Ban hành văn bản

### Route

- `/admin/xay-dung-van-ban/quyet-dinh/ban-hanh/:id`
- `/admin/xay-dung-van-ban/nghi-quyet/ban-hanh/:id`

### Chức năng

- Cập nhật văn bản đã được ký ban hành.
- Nhập số, ký hiệu, ngày ban hành.
- Đính kèm file văn bản ban hành.
- Ghi nhận cơ quan ký ban hành.
- Hoàn thành quy trình.

### Kết quả cần đạt

Hồ sơ kết thúc đúng trạng thái và lưu đủ thông tin văn bản ban hành.

## 11. Giai đoạn 10: Thành phần dùng chung

### Component đề xuất

```text
HoSoSummary.jsx
FileDinhKemTable.jsx
TienDoQuyTrinh.jsx
LichSuXuLyTable.jsx
TrangThaiBadge.jsx
ChuyenBuocActions.jsx
VanBanWorkflowTabs.jsx
```

### Mục đích

- Dùng lại giữa Quyết định và Nghị quyết.
- Giảm trùng lặp code.
- Dễ bảo trì khi quy trình thay đổi.
- Đồng bộ giao diện, trạng thái và thao tác chuyển bước.

## 12. Giai đoạn 11: Kiểm tra và hoàn thiện

### Kiểm tra luồng Quyết định UBND tỉnh

- Tạo hồ sơ.
- Soạn thảo và đính kèm tài liệu.
- Gửi thẩm định.
- Thẩm định.
- Trình UBND.
- Cập nhật ý kiến UBND.
- Ban hành Quyết định.

### Kiểm tra luồng Nghị quyết HĐND tỉnh

- Tạo hồ sơ.
- Soạn thảo và đính kèm tài liệu.
- Gửi thẩm định.
- Thẩm định.
- Trình UBND.
- Cập nhật ý kiến UBND.
- Trình HĐND thẩm tra.
- Ban hành Nghị quyết.

### Kiểm tra phân quyền

- Cơ quan chủ trì.
- Sở Tư pháp.
- UBND tỉnh.
- HĐND tỉnh.
- Quản trị hệ thống.

### Kiểm tra dữ liệu nghiệp vụ

- File đính kèm.
- Trạng thái hồ sơ.
- Lịch sử xử lý.
- Chuyển bước.
- Trả lại và yêu cầu bổ sung.
- Hoàn thành quy trình.

## 13. Thứ tự ưu tiên triển khai

1. Danh sách hồ sơ.
2. Tạo, sửa, chi tiết hồ sơ.
3. Soạn thảo và file đính kèm.
4. Gửi thẩm định.
5. Thẩm định.
6. Trình UBND.
7. Ý kiến UBND.
8. Ban hành Quyết định.
9. Trình HĐND thẩm tra.
10. Ban hành Nghị quyết.

## 14. Nguyên tắc thiết kế

- Module FE đặt trong `FE/src/features/xay-dung-van-ban`.
- Dùng chung dữ liệu hồ sơ cho Quyết định và Nghị quyết.
- Phân nhánh nghiệp vụ theo `QuyTrinhSoanThaoId`, `LoaiVanBan` hoặc mã quy trình.
- Mỗi bước nghiệp vụ có màn riêng để dễ bảo trì.
- Các phần file đính kèm, lịch sử xử lý, trạng thái, tiến độ và thao tác chuyển bước dùng component chung.
- Giao diện theo bố cục quản trị hiện có: danh sách, bộ lọc, phân trang, thao tác, modal/form chi tiết khi phù hợp.

## 15. Nhật ký triển khai cập nhật ngày 08/10/2026

### 15.1. Cấu trúc module FE

- Đã tạo module theo cấu trúc riêng tại `FE/src/features/xay-dung-van-ban`.
- Đã có các nhóm file chính:
  - `api/xayDungVanBanApi.js`
  - `pages/HoSoListPage.jsx`
  - `pages/HoSoFormPage.jsx`
  - `pages/HoSoDetailPage.jsx`
  - `pages/HoSoYKienDongGopPage.jsx`
  - `routes.jsx`
- Đã khai báo route để truy cập các màn nghiệp vụ hồ sơ xây dựng văn bản.

### 15.2. Màn danh sách hồ sơ

- Đã xây dựng màn `/admin/xay-dung-van-ban/ho-so` và `/xay-dung-van-ban/soan-thao`.
- Đã có tìm kiếm hồ sơ theo mã hồ sơ, tên hồ sơ, tên dự thảo.
- Đã có phân trang theo bố cục datatable của hệ thống.
- Đã có bộ lọc:
  - Loại văn bản.
  - Quy trình soạn thảo.
  - Năm xây dựng.
  - Đơn vị chủ trì soạn thảo.
- Đã bổ sung cột `Trạng thái hồ sơ`.
- Đã điều chỉnh cột thao tác theo trạng thái:
  - Hồ sơ đã chuyển bước chỉ còn `Xem chi tiết`.
  - Chỉ hồ sơ ở trạng thái nháp hoặc trả lại mới có thao tác `Nhập ý kiến`, `Chuyển bước`, `Sửa`, `Xóa`.
- Đã bổ sung nút `Xóa` hồ sơ và gọi API xóa hồ sơ soạn thảo.

### 15.3. Màn tạo mới, cập nhật và chi tiết hồ sơ

- Đã xây dựng màn tạo mới hồ sơ `/admin/xay-dung-van-ban/ho-so/them-moi`.
- Đã xây dựng màn cập nhật hồ sơ theo dạng form tương tự tạo mới.
- Đã thêm chức năng tải file đính kèm cho hồ sơ.
- Khi lưu hồ sơ thành công, FE quay lại màn danh sách hồ sơ.
- Đã xử lý lỗi thiếu thông tin đơn vị xử lý khi lưu bằng cách đồng bộ lại thông tin người dùng và đơn vị từ BE.
- Đã xây dựng màn xem chi tiết hồ sơ riêng tại `/admin/xay-dung-van-ban/ho-so/chi-tiet/:id`.
- Màn chi tiết hiển thị thông tin hồ sơ và timeline xử lý.

### 15.4. Timeline quy trình

- Đã thiết kế timeline trong màn chi tiết hồ sơ.
- Timeline hiển thị các bước theo đúng thứ tự quy trình.
- Đã phân biệt trạng thái từng bước:
  - Đã hoàn thành.
  - Đang thực hiện.
  - Chưa thực hiện.
- Màn chi tiết được định hướng dùng chung cho tất cả các bước trong quy trình.

### 15.5. Ý kiến đóng góp

- Đã xây dựng màn nghiệp vụ nhập ý kiến đóng góp tại `/admin/xay-dung-van-ban/ho-so/:id/y-kien-dong-gop`.
- Đã thiết kế lại bảng `Danh sách ý kiến đóng góp`.
- Đã có nút `Thêm mới` mở modal nhập ý kiến.
- Đã hỗ trợ lưu ý kiến mới và tải lại danh sách ý kiến sau khi thêm/sửa.
- Đã bổ sung phần file đính kèm để upload bảng tổng hợp ý kiến đã làm từ bên ngoài.
- Đã bỏ phần chuyển bước trong màn hình ý kiến đóng góp; thao tác chuyển bước chỉ thực hiện ở màn danh sách hồ sơ.

### 15.6. Chuyển bước hồ sơ

- Đã thiết kế modal `Chuyển hồ sơ sang bước tiếp theo` tại màn danh sách hồ sơ.
- Modal tự lấy:
  - Bước tiếp theo theo cấu hình quy trình.
  - Trạng thái sau chuyển.
  - Đơn vị tiếp nhận mặc định theo bước.
  - Ngày chuyển mặc định là ngày hiện tại.
  - Số ngày xử lý theo bước quy trình; nếu không có thì mặc định 5 ngày.
  - Thời hạn xử lý = ngày chuyển + số ngày xử lý.
- Người dùng có thể chọn lại ngày chuyển, số ngày xử lý, thời hạn xử lý và đơn vị tiếp nhận.
- Đơn vị tiếp nhận là trường bắt buộc khi chuyển bước.
- Đã cập nhật điều kiện chuyển bước chỉ còn kiểm tra nội dung: `Chưa cập nhật nội dung tổng hợp, tiếp thu và giải trình ý kiến.`

### 15.7. Backend/API đã bổ sung để hỗ trợ FE

- Đã bổ sung API nghiệp vụ cho `XayDungVanBanService` phục vụ:
  - Danh sách hồ sơ.
  - Chi tiết hồ sơ.
  - Timeline hồ sơ.
  - Tạo, sửa, xóa hồ sơ soạn thảo.
  - Upload tài liệu hồ sơ.
  - Quản lý ý kiến đơn vị.
  - Upload file tổng hợp ý kiến.
  - Kiểm tra điều kiện trước khi chuyển bước.
  - Chuyển hồ sơ sang bước tiếp theo.
- Đã bổ sung dữ liệu ngày chuyển và hạn đề nghị trả kết quả trong request chuyển bước.
- Đã cấu hình các service BE không tự mở `swagger/index.html` khi chạy/build.

### 15.8. Kiểm tra build

- Đã chạy `npm run build` cho FE và build thành công.
- Đã build `XayDungVanBanService` sau các thay đổi backend và build thành công bằng output tạm để tránh xung đột file đang chạy.

### 15.9. Phần còn tiếp tục

- Hoàn thiện các màn nghiệp vụ riêng sau bước soạn thảo:
  - Trình thẩm định.
  - Thẩm định.
  - Trình UBND/phê duyệt.
  - Ý kiến UBND.
  - Thẩm tra HĐND.
  - Ban hành văn bản.
- Tách component dùng chung cho file đính kèm, timeline, trạng thái và thao tác chuyển bước khi các màn nghiệp vụ tiếp theo ổn định.
- Hoàn thiện kiểm soát quyền thao tác theo API hành động khả dụng thay vì chỉ dựa trên trạng thái ở client.
