/* Seed idempotent danh mục trạng thái dùng chung theo từng service. */
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF COL_LENGTH('dm.DanhMucTrangThais', 'NhomTrangThai') IS NULL
BEGIN
    ALTER TABLE [dm].[DanhMucTrangThais]
    ADD [NhomTrangThai] nvarchar(max) NOT NULL
        CONSTRAINT [DF_DanhMucTrangThais_NhomTrangThai] DEFAULT N'';
END;
GO

DECLARE @Now datetime2 = SYSUTCDATETIME();
DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';

DECLARE @Statuses TABLE
(
    Nhom nvarchar(100) NOT NULL,
    Ma nvarchar(100) NOT NULL,
    Ten nvarchar(250) NOT NULL,
    Mau nvarchar(20) NOT NULL,
    ThuTu int NOT NULL,
    MoTa nvarchar(max) NULL
);

INSERT INTO @Statuses (Nhom, Ma, Ten, Mau, ThuTu, MoTa) VALUES
-- DangKyXayDungVanBanService
(N'DANG_KY_XAY_DUNG_VAN_BAN', N'MOI_TAO', N'Mới tạo', N'#6B7280', 1, N'Hồ sơ đăng ký vừa được tạo.'),
(N'DANG_KY_XAY_DUNG_VAN_BAN', N'DANG_SOAN_THAO', N'Đang soạn thảo', N'#2563EB', 2, N'Hồ sơ đăng ký đang được soạn thảo.'),
(N'DANG_KY_XAY_DUNG_VAN_BAN', N'DA_TRINH_PHE_DUYET', N'Đã trình phê duyệt', N'#D97706', 3, N'Hồ sơ đăng ký đã trình phê duyệt.'),
(N'DANG_KY_XAY_DUNG_VAN_BAN', N'DA_PHE_DUYET', N'Đã phê duyệt', N'#16A34A', 4, N'Hồ sơ đăng ký đã được phê duyệt.'),
(N'DANG_KY_XAY_DUNG_VAN_BAN', N'BI_TRA_LAI', N'Bị trả lại', N'#DC2626', 5, N'Hồ sơ đăng ký bị trả lại để chỉnh sửa.'),
(N'DANG_KY_XAY_DUNG_VAN_BAN', N'KHONG_PHE_DUYET', N'Không phê duyệt', N'#B91C1C', 6, N'Hồ sơ đăng ký không được phê duyệt.'),
(N'DANG_KY_XAY_DUNG_VAN_BAN', N'DA_CAP_NHAT_KET_QUA', N'Đã cập nhật kết quả', N'#0891B2', 7, N'Hồ sơ đăng ký đã cập nhật kết quả.'),
(N'DANG_KY_XAY_DUNG_VAN_BAN', N'HOAN_THANH', N'Hoàn thành', N'#16A34A', 8, N'Hồ sơ đăng ký hoàn thành.'),
(N'DANG_KY_XAY_DUNG_VAN_BAN', N'DA_CHUYEN_QUY_TRINH_XAY_DUNG', N'Đã chuyển quy trình xây dựng', N'#7C3AED', 9, N'Hồ sơ đã chuyển sang quy trình xây dựng văn bản.'),

-- XayDungVanBanService
(N'HO_SO_XAY_DUNG_VAN_BAN', N'DANG_XU_LY', N'Đang xử lý', N'#2563EB', 1, N'Hồ sơ xây dựng văn bản đang xử lý.'),
(N'HO_SO_XAY_DUNG_VAN_BAN', N'HOAN_THANH', N'Hoàn thành', N'#16A34A', 2, N'Hồ sơ xây dựng văn bản hoàn thành.'),
(N'HO_SO_XAY_DUNG_VAN_BAN', N'HOAN_THANH_DUNG_HAN', N'Hoàn thành đúng hạn', N'#15803D', 3, N'Hồ sơ hoàn thành đúng hạn.'),
(N'HO_SO_XAY_DUNG_VAN_BAN', N'HOAN_THANH_QUA_HAN', N'Hoàn thành quá hạn', N'#D97706', 4, N'Hồ sơ hoàn thành quá hạn.'),
(N'BO_HO_SO_NGHIEP_VU', N'NHAP', N'Nháp', N'#6B7280', 1, N'Bộ hồ sơ nghiệp vụ đang nhập.'),
(N'BO_HO_SO_NGHIEP_VU', N'DA_GUI', N'Đã gửi', N'#2563EB', 2, N'Bộ hồ sơ nghiệp vụ đã gửi.'),
(N'BO_HO_SO_NGHIEP_VU', N'DA_TIEP_NHAN', N'Đã tiếp nhận', N'#0891B2', 3, N'Bộ hồ sơ nghiệp vụ đã được tiếp nhận.'),
(N'BO_HO_SO_NGHIEP_VU', N'YEU_CAU_BO_SUNG', N'Yêu cầu bổ sung', N'#EA580C', 4, N'Bộ hồ sơ nghiệp vụ cần bổ sung.'),
(N'BO_HO_SO_NGHIEP_VU', N'DA_HOAN_THANH', N'Đã hoàn thành', N'#16A34A', 5, N'Bộ hồ sơ nghiệp vụ đã hoàn thành.'),
(N'BO_HO_SO_NGHIEP_VU', N'HUY', N'Hủy', N'#DC2626', 6, N'Bộ hồ sơ nghiệp vụ đã hủy.'),
(N'NHAC_TIEN_DO_XAY_DUNG_VAN_BAN', N'DA_GUI', N'Đã gửi', N'#2563EB', 1, N'Nhắc tiến độ đã gửi.'),
(N'NHAC_TIEN_DO_XAY_DUNG_VAN_BAN', N'DA_PHAN_HOI', N'Đã phản hồi', N'#D97706', 2, N'Nhắc tiến độ đã được phản hồi.'),
(N'NHAC_TIEN_DO_XAY_DUNG_VAN_BAN', N'DA_XU_LY', N'Đã xử lý', N'#16A34A', 3, N'Nhắc tiến độ đã xử lý.'),
(N'CHAM_DIEM_HO_SO', N'NHAP', N'Nháp', N'#6B7280', 1, N'Bảng điểm đang được tạo hoặc tính lại.'),
(N'CHAM_DIEM_HO_SO', N'DA_CHOT', N'Đã chốt', N'#16A34A', 2, N'Kết quả chấm điểm chính thức.'),
(N'CHAM_DIEM_HO_SO', N'DA_HUY', N'Đã hủy', N'#DC2626', 3, N'Bảng điểm đã bị hủy sau khi chốt.'),

-- ThiHanhPhapLuatService
(N'KE_HOACH_THI_HANH_PHAP_LUAT', N'NHAP', N'Nháp', N'#6B7280', 1, NULL),
(N'KE_HOACH_THI_HANH_PHAP_LUAT', N'DANG_THUC_HIEN', N'Đang thực hiện', N'#2563EB', 2, NULL),
(N'KE_HOACH_THI_HANH_PHAP_LUAT', N'DA_TONG_HOP', N'Đã tổng hợp', N'#7C3AED', 3, NULL),
(N'KE_HOACH_THI_HANH_PHAP_LUAT', N'HOAN_THANH', N'Hoàn thành', N'#16A34A', 4, NULL),
(N'KE_HOACH_THI_HANH_PHAP_LUAT', N'HUY', N'Hủy', N'#DC2626', 5, NULL),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'CHUA_THUC_HIEN', N'Chưa thực hiện', N'#6B7280', 1, NULL),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'DANG_THUC_HIEN', N'Đang thực hiện', N'#2563EB', 2, NULL),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'CHO_DANH_GIA', N'Chờ đánh giá', N'#D97706', 3, NULL),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'YEU_CAU_BO_SUNG', N'Yêu cầu bổ sung', N'#EA580C', 4, NULL),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'DAT', N'Đạt', N'#16A34A', 5, NULL),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'KHONG_DAT', N'Không đạt', N'#DC2626', 6, NULL),
(N'NOI_DUNG_THI_HANH_PHAP_LUAT', N'QUA_HAN', N'Quá hạn', N'#B91C1C', 7, NULL),
(N'BAO_CAO_TIEN_DO_THI_HANH', N'NHAP', N'Nháp', N'#6B7280', 1, NULL),
(N'BAO_CAO_TIEN_DO_THI_HANH', N'DA_GUI', N'Đã gửi', N'#2563EB', 2, NULL),
(N'BAO_CAO_TIEN_DO_THI_HANH', N'CAN_BO_SUNG', N'Cần bổ sung', N'#EA580C', 3, NULL),
(N'BAO_CAO_TIEN_DO_THI_HANH', N'DA_XAC_NHAN', N'Đã xác nhận', N'#16A34A', 4, NULL),
(N'BAO_CAO_TONG_HOP_THI_HANH', N'NHAP', N'Nháp', N'#6B7280', 1, NULL),
(N'BAO_CAO_TONG_HOP_THI_HANH', N'DA_CHOT', N'Đã chốt', N'#16A34A', 2, NULL),
(N'BAO_CAO_TONG_HOP_THI_HANH', N'MO_LAI', N'Mở lại', N'#EA580C', 3, NULL),
(N'BAO_CAO_TONG_HOP_THI_HANH', N'HUY', N'Hủy', N'#DC2626', 4, NULL),

-- KhaoSatThiHanhPhapLuatService
(N'CUOC_KHAO_SAT_THI_HANH_PHAP_LUAT', N'NHAP', N'Nháp', N'#6B7280', 1, NULL),
(N'CUOC_KHAO_SAT_THI_HANH_PHAP_LUAT', N'DANG_THUC_HIEN', N'Đang thực hiện', N'#2563EB', 2, NULL),
(N'CUOC_KHAO_SAT_THI_HANH_PHAP_LUAT', N'DA_KET_THUC', N'Đã kết thúc', N'#16A34A', 3, NULL),
(N'CUOC_KHAO_SAT_THI_HANH_PHAP_LUAT', N'HUY', N'Hủy', N'#DC2626', 4, NULL),
(N'DOI_TUONG_KHAO_SAT', N'CHUA_NOP', N'Chưa nộp', N'#6B7280', 1, NULL),
(N'DOI_TUONG_KHAO_SAT', N'DA_NOP', N'Đã nộp', N'#2563EB', 2, NULL),
(N'DOI_TUONG_KHAO_SAT', N'CAN_BO_SUNG', N'Cần bổ sung', N'#EA580C', 3, NULL),
(N'DOI_TUONG_KHAO_SAT', N'DA_XAC_NHAN', N'Đã xác nhận', N'#16A34A', 4, NULL),
(N'PHIEU_NOP_KHAO_SAT', N'PHIEU_HOP_LE', N'Phiếu hợp lệ', N'#16A34A', 1, NULL),
(N'PHIEU_NOP_KHAO_SAT', N'THANH_CONG', N'Thành công', N'#16A34A', 2, NULL),
(N'PHIEU_NOP_KHAO_SAT', N'LOI', N'Lỗi', N'#DC2626', 3, NULL),
(N'PHIEU_NOP_KHAO_SAT', N'CAN_BO_SUNG', N'Cần bổ sung', N'#EA580C', 4, NULL),
(N'MAU_PHIEU_KHAO_SAT', N'NHAP', N'Nháp', N'#6B7280', 1, NULL),
(N'MAU_PHIEU_KHAO_SAT', N'DANG_SU_DUNG', N'Đang sử dụng', N'#16A34A', 2, NULL),
(N'MAU_PHIEU_KHAO_SAT', N'HET_HIEU_LUC', N'Hết hiệu lực', N'#DC2626', 3, NULL),
(N'DOC_MAU_PHIEU_KHAO_SAT', N'CHO_RA_SOAT', N'Chờ rà soát', N'#D97706', 1, NULL),
(N'DOC_MAU_PHIEU_KHAO_SAT', N'DA_XAC_NHAN', N'Đã xác nhận', N'#16A34A', 2, NULL),
(N'DOC_MAU_PHIEU_KHAO_SAT', N'LOI_CHUYEN_DOI', N'Lỗi chuyển đổi', N'#DC2626', 3, NULL),
(N'BAO_CAO_KHAO_SAT', N'NHAP', N'Nháp', N'#6B7280', 1, NULL),
(N'BAO_CAO_KHAO_SAT', N'DA_CHOT', N'Đã chốt', N'#16A34A', 2, NULL),

-- KhaiThacDuLieuService
(N'CANH_BAO_KHAI_THAC_DU_LIEU', N'MOI', N'Mới', N'#6B7280', 1, NULL),
(N'CANH_BAO_KHAI_THAC_DU_LIEU', N'DANG_XU_LY', N'Đang xử lý', N'#2563EB', 2, NULL),
(N'CANH_BAO_KHAI_THAC_DU_LIEU', N'DA_XU_LY', N'Đã xử lý', N'#16A34A', 3, NULL),
(N'DONG_BO_KHAI_THAC_DU_LIEU', N'DANG_CHAY', N'Đang chạy', N'#2563EB', 1, NULL),
(N'DONG_BO_KHAI_THAC_DU_LIEU', N'THANH_CONG', N'Thành công', N'#16A34A', 2, NULL),
(N'DONG_BO_KHAI_THAC_DU_LIEU', N'THAT_BAI', N'Thất bại', N'#DC2626', 3, NULL);

BEGIN TRANSACTION;

INSERT INTO [dm].[DanhMucTrangThais]
(
    [Id],
    [NhomTrangThai],
    [MaTrangThai],
    [TenTrangThai],
    [MaMauHex],
    [ThuTuSapXep],
    [TrangThai],
    [MoTa],
    [GhiChu],
    [CreatedBy],
    [CreatedDate],
    [UpdatedBy],
    [UpdatedDate]
)
SELECT
    NEWID(),
    s.Nhom,
    s.Ma,
    s.Ten,
    s.Mau,
    s.ThuTu,
    1,
    s.MoTa,
    NULL,
    @SystemUserId,
    @Now,
    @SystemUserId,
    @Now
FROM @Statuses s
WHERE NOT EXISTS
(
    SELECT 1
    FROM [dm].[DanhMucTrangThais] d
    WHERE d.[NhomTrangThai] = s.Nhom
      AND d.[MaTrangThai] = s.Ma
);

UPDATE d
SET
    d.[TenTrangThai] = s.Ten,
    d.[MaMauHex] = s.Mau,
    d.[ThuTuSapXep] = s.ThuTu,
    d.[TrangThai] = 1,
    d.[MoTa] = COALESCE(s.MoTa, d.[MoTa]),
    d.[UpdatedBy] = @SystemUserId,
    d.[UpdatedDate] = @Now
FROM [dm].[DanhMucTrangThais] d
JOIN @Statuses s
    ON s.Nhom = d.[NhomTrangThai]
   AND s.Ma = d.[MaTrangThai];

COMMIT TRANSACTION;

SELECT
    [NhomTrangThai],
    COUNT(1) AS [SoLuong]
FROM [dm].[DanhMucTrangThais]
GROUP BY [NhomTrangThai]
ORDER BY [NhomTrangThai];
