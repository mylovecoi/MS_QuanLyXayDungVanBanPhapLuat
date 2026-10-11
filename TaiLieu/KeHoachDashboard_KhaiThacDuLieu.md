# Kế hoạch Dashboard khai thác dữ liệu

Tài liệu này căn cứ theo nhóm chức năng Dashboard, cảnh báo thông minh, tra cứu tìm kiếm và báo cáo thống kê trong `QT khai thac DL.docx`.

## 1. Phạm vi

Dashboard khai thác dữ liệu phục vụ tổng quan điều hành, theo dõi tiến độ xây dựng văn bản, tình hình tổ chức thi hành pháp luật, cảnh báo thông minh và truy cập nhanh báo cáo thống kê.

## 2. Màn hình

| STT | Màn hình | Route |
| --- | --- | --- |
| 1 | Dashboard khai thác dữ liệu | `/khai-thac-du-lieu/dashboard` |
| 2 | Cấu hình Dashboard | `/khai-thac-du-lieu/dashboard/cau-hinh` |
| 3 | Danh mục báo cáo thống kê | `/khai-thac-du-lieu/bao-cao` |
| 4 | Kết quả báo cáo thống kê | `/khai-thac-du-lieu/bao-cao/:maBaoCao` |

## 3. Widget dashboard

| Mã widget | Nội dung |
| --- | --- |
| `TONG_QUAN_DIEU_HANH` | Tổng quan điều hành |
| `TIEN_DO_XAY_DUNG_VAN_BAN` | Tiến độ xây dựng văn bản QPPL |
| `CANH_BAO_THONG_MINH` | Cảnh báo thông minh |
| `TO_CHUC_THI_HANH_PHAP_LUAT` | Tổ chức thi hành pháp luật |
| `BAO_CAO_THONG_KE` | Báo cáo thống kê |
| `HO_SO_SAP_HAN` | Hồ sơ sắp hạn |
| `HO_SO_QUA_HAN` | Hồ sơ quá hạn |
| `NHIEM_VU_QUA_HAN` | Nhiệm vụ quá hạn |
| `TY_LE_BAN_HANH_THEO_DON_VI` | Tỷ lệ ban hành theo đơn vị |
| `TY_LE_HOAN_THANH_THEO_DON_VI` | Tỷ lệ hoàn thành theo đơn vị |

## 4. Cấu hình hiển thị

Mỗi widget có thể cấu hình:

- Bật hoặc tắt hiển thị.
- Sắp xếp thứ tự.
- Dạng hiển thị: thẻ số liệu, biểu đồ cột, biểu đồ đường, biểu đồ donut, bảng, danh sách.
- Kích thước: nhỏ, vừa, rộng, toàn hàng.
- Bộ lọc mặc định dạng JSON.
- Thời gian tự động làm mới theo phút.

## 5. RoleAction

Các RoleAction cần có:

| Role | Controller | Action | Route |
| --- | --- | --- | --- |
| `VanBanQPPL.Dashboard.TongQuan` | `DashboardTongQuan` | `Index` | `/khai-thac-du-lieu/dashboard` |
| `VanBanQPPL.Dashboard.CauHinh` | `DashboardTongQuan` | `Edit` | `/khai-thac-du-lieu/dashboard/cau-hinh` |

## 6. Đã xây dựng

- Bổ sung bảng `DashboardWidgets` và `DashboardWidgetCauHinhs`.
- Bổ sung API lấy layout dashboard và lưu cấu hình widget cá nhân.
- Bổ sung màn Dashboard khai thác dữ liệu.
- Bổ sung màn Cấu hình Dashboard.
- Bổ sung RoleAction cho Dashboard và Cấu hình Dashboard.
- Bổ sung nguồn dữ liệu thực tế từ hồ sơ xây dựng văn bản cho các widget:
  `TIEN_DO_XAY_DUNG_VAN_BAN`, `HO_SO_SAP_HAN`, `HO_SO_QUA_HAN`,
  `TY_LE_BAN_HANH_THEO_DON_VI`.
- Bổ sung hiển thị tên đơn vị từ danh mục cho biểu đồ theo đơn vị.
- Bổ sung nguồn dữ liệu thực tế từ thi hành pháp luật cho các widget:
  `TO_CHUC_THI_HANH_PHAP_LUAT`, `NHIEM_VU_QUA_HAN`,
  `TY_LE_HOAN_THANH_THEO_DON_VI`.
- Bổ sung bộ lọc chung trên Dashboard: năm, đơn vị, loại văn bản, quy trình,
  trạng thái, từ ngày và đến ngày.
- API `GET /api/khai-thac-du-lieu/dashboard/layout` đã nhận bộ lọc và áp dụng
  cho dữ liệu xây dựng văn bản, thi hành pháp luật.
- Bổ sung danh mục báo cáo thống kê theo quy trình khai thác dữ liệu.
- Bổ sung luồng kết xuất báo cáo: chọn báo cáo, nhập tham số trong modal,
  mở kết quả ở tab mới.
- Bổ sung màn kết quả báo cáo có thẻ tổng hợp, group theo đơn vị chủ trì,
  group theo trạng thái, bảng dữ liệu chi tiết, nút in và xuất dữ liệu CSV.
- Hoàn thiện màn kết quả báo cáo ưu tiên dữ liệu chi tiết: có chế độ xem tất cả,
  dải dữ liệu theo group đơn vị chủ trì, dải dữ liệu theo group trạng thái và
  xuất CSV theo đúng chế độ đang xem.
- Bổ sung dữ liệu thật cho các báo cáo nền: đăng ký ban hành văn bản QPPL,
  văn bản ban hành/theo giai đoạn, tổ chức thi hành pháp luật và mức độ hoàn
  thành theo đơn vị.
- Áp dụng phạm vi dữ liệu theo người dùng: tài khoản SSA xem toàn bộ, tài khoản đơn vị xem hồ sơ của đơn vị hoặc hồ sơ do mình tạo.

## 7. Việc làm tiếp

- Bổ sung tên loại văn bản, quy trình, trạng thái từ danh mục để thay nhãn mã định danh trong biểu đồ.
- Bổ sung dashboard theo loại văn bản, lĩnh vực và năm dưới dạng widget chuyên sâu.
- Bổ sung xuất báo cáo Excel/PDF/Word phía BE thay cho CSV phía trình duyệt khi người dùng yêu cầu.
