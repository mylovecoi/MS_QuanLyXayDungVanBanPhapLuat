# Seed dữ liệu mẫu hồ sơ quy trình

Script `tools/Seed-SampleHoSoQuyTrinh.ps1` chuẩn bị dữ liệu mẫu cho 5 quy trình nghiệp vụ đang được seed từ tài liệu QT:

- `DXDM_DKXD_QPPL`: Đề xuất danh mục / Đăng ký xây dựng QPPL
- `XD_QD_UBND_TINH`: Xây dựng Quyết định UBND tỉnh
- `XD_NQ_HDND_TINH`: Xây dựng Nghị quyết HĐND tỉnh
- `TC_THI_HANH_PHAP_LUAT`: Tổ chức thi hành pháp luật
- `KS_THI_HANH_PHAP_LUAT`: Khảo sát tình hình thi hành pháp luật

Mỗi quy trình có 5 hồ sơ/bộ hồ sơ hoàn chỉnh. Script tự tạo file đính kèm `.txt` đơn giản cho từng bước và sinh SQL idempotent tại:

```powershell
tools/seed_sample_ho_so_quy_trinh.generated.sql
```

Chạy khô, chỉ sinh file và SQL:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\Seed-SampleHoSoQuyTrinh.ps1
```

Nạp vào SQL Server khi đã kiểm tra đúng database:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\Seed-SampleHoSoQuyTrinh.ps1 -Execute -Password "<sa-password>"
```

Script mặc định dùng `Server=14.248.85.58`, `Database=CSDLXayDungVanBan_MS`, `Username=sa`; có thể override bằng tham số `-Server`, `-Database`, `-Username`.

Lưu ý: chạy trước các seed danh mục quy trình/trạng thái nếu môi trường DB chưa có:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\Seed-DanhMucTrangThaiByService.ps1 -Password "<sa-password>"
```

và SQL `BE/services/DanhMucService/Infrastructure/Persistence/Data/seed_quy_trinh_nghiep_vu_from_qt.sql`.
