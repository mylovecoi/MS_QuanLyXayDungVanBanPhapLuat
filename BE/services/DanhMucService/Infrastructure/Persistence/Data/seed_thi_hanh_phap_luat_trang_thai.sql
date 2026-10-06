/* Seed idempotent danh mục trạng thái cho ThiHanhPhapLuatService. */
SET NOCOUNT ON;
DECLARE @Now datetime2 = SYSUTCDATETIME();
DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';

DECLARE @Statuses TABLE (Nhom nvarchar(100), Ma nvarchar(100), Ten nvarchar(250), Mau nvarchar(20), ThuTu int);
INSERT INTO @Statuses VALUES
(N'KE_HOACH_THI_HANH_PHAP_LUAT', N'NHAP', N'Nháp', N'#6B7280', 1),
(N'KE_HOACH_THI_HANH_PHAP_LUAT', N'DANG_THUC_HIEN', N'Đang thực hiện', N'#2563EB', 2),
(N'KE_HOACH_THI_HANH_PHAP_LUAT', N'DA_TONG_HOP', N'Đã tổng hợp', N'#7C3AED', 3),
(N'KE_HOACH_THI_HANH_PHAP_LUAT', N'HOAN_THANH', N'Hoàn thành', N'#16A34A', 4),
(N'KE_HOACH_THI_HANH_PHAP_LUAT', N'HUY', N'Hủy', N'#DC2626', 5),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'CHUA_THUC_HIEN', N'Chưa thực hiện', N'#6B7280', 1),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'DANG_THUC_HIEN', N'Đang thực hiện', N'#2563EB', 2),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'CHO_DANH_GIA', N'Chờ đánh giá', N'#D97706', 3),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'YEU_CAU_BO_SUNG', N'Yêu cầu bổ sung', N'#EA580C', 4),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'DAT', N'Đạt', N'#16A34A', 5),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'KHONG_DAT', N'Không đạt', N'#DC2626', 6),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'QUA_HAN', N'Quá hạn', N'#B91C1C', 7),
(N'BAO_CAO_TIEN_DO_THI_HANH', N'NHAP', N'Nháp', N'#6B7280', 1),
(N'BAO_CAO_TIEN_DO_THI_HANH', N'DA_GUI', N'Đã gửi', N'#2563EB', 2),
(N'BAO_CAO_TIEN_DO_THI_HANH', N'CAN_BO_SUNG', N'Cần bổ sung', N'#EA580C', 3),
(N'BAO_CAO_TIEN_DO_THI_HANH', N'DA_XAC_NHAN', N'Đã xác nhận', N'#16A34A', 4),
(N'BAO_CAO_TONG_HOP_THI_HANH', N'NHAP', N'Nháp', N'#6B7280', 1),
(N'BAO_CAO_TONG_HOP_THI_HANH', N'DA_CHOT', N'Đã chốt', N'#16A34A', 2),
(N'BAO_CAO_TONG_HOP_THI_HANH', N'MO_LAI', N'Mở lại', N'#EA580C', 3),
(N'BAO_CAO_TONG_HOP_THI_HANH', N'HUY', N'Hủy', N'#DC2626', 4);

INSERT INTO [dm].[DanhMucTrangThais] ([Id], [NhomTrangThai], [MaTrangThai], [TenTrangThai], [MaMauHex], [ThuTuSapXep], [TrangThai], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
SELECT NEWID(), s.Nhom, s.Ma, s.Ten, s.Mau, s.ThuTu, 1, @SystemUserId, @Now, @SystemUserId, @Now
FROM @Statuses s
WHERE NOT EXISTS (SELECT 1 FROM [dm].[DanhMucTrangThais] d WHERE d.[NhomTrangThai] = s.Nhom AND d.[MaTrangThai] = s.Ma);

UPDATE d SET d.[TenTrangThai] = s.Ten, d.[MaMauHex] = s.Mau, d.[ThuTuSapXep] = s.ThuTu, d.[TrangThai] = 1, d.[UpdatedBy] = @SystemUserId, d.[UpdatedDate] = @Now
FROM [dm].[DanhMucTrangThais] d JOIN @Statuses s ON s.Nhom = d.[NhomTrangThai] AND s.Ma = d.[MaTrangThai];
