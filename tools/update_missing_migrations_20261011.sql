SET NOCOUNT ON;

PRINT N'Applying missing database migrations for 2026-10-11...';

IF OBJECT_ID(N'dbo.DangKyXayDungVanBans', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DangKyXayDungVanBans_CreatedBy_CreatedAt' AND object_id = OBJECT_ID(N'dbo.DangKyXayDungVanBans'))
    BEGIN
        IF EXISTS (
            SELECT 1
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = N'dbo'
              AND TABLE_NAME = N'DangKyXayDungVanBans'
              AND COLUMN_NAME = N'CreatedBy'
              AND CHARACTER_MAXIMUM_LENGTH = -1
        )
            CREATE INDEX IX_DangKyXayDungVanBans_CreatedBy_CreatedAt ON dbo.DangKyXayDungVanBans (CreatedAt) INCLUDE (CreatedBy);
        ELSE
            CREATE INDEX IX_DangKyXayDungVanBans_CreatedBy_CreatedAt ON dbo.DangKyXayDungVanBans (CreatedBy, CreatedAt);
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DangKyXayDungVanBans_DonViPheDuyetId_TrangThaiHoSoId_CreatedAt' AND object_id = OBJECT_ID(N'dbo.DangKyXayDungVanBans'))
        CREATE INDEX IX_DangKyXayDungVanBans_DonViPheDuyetId_TrangThaiHoSoId_CreatedAt ON dbo.DangKyXayDungVanBans (DonViPheDuyetId, TrangThaiHoSoId, CreatedAt);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DangKyXayDungVanBans_DonViSoanThaoId_TrangThaiHoSoId_CreatedAt' AND object_id = OBJECT_ID(N'dbo.DangKyXayDungVanBans'))
        CREATE INDEX IX_DangKyXayDungVanBans_DonViSoanThaoId_TrangThaiHoSoId_CreatedAt ON dbo.DangKyXayDungVanBans (DonViSoanThaoId, TrangThaiHoSoId, CreatedAt);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DangKyXayDungVanBans_LoaiVanBanId' AND object_id = OBJECT_ID(N'dbo.DangKyXayDungVanBans'))
        CREATE INDEX IX_DangKyXayDungVanBans_LoaiVanBanId ON dbo.DangKyXayDungVanBans (LoaiVanBanId);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DangKyXayDungVanBans_NamDangKy' AND object_id = OBJECT_ID(N'dbo.DangKyXayDungVanBans'))
        CREATE INDEX IX_DangKyXayDungVanBans_NamDangKy ON dbo.DangKyXayDungVanBans (NamDangKy);
END;

IF OBJECT_ID(N'dm.DanhMucTrangThais', N'U') IS NOT NULL AND COL_LENGTH(N'dm.DanhMucTrangThais', N'NhomTrangThai') IS NULL
BEGIN
    ALTER TABLE dm.DanhMucTrangThais ADD NhomTrangThai nvarchar(max) NOT NULL CONSTRAINT DF_DanhMucTrangThais_NhomTrangThai DEFAULT N'';
END;

IF OBJECT_ID(N'dm.DanhMucTieuChiDiems', N'U') IS NOT NULL
BEGIN
    MERGE dm.DanhMucTieuChiDiems AS target
    USING (VALUES
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111101'), N'CHAT_LUONG_SOAN_THAO', N'Chất lượng soạn thảo', N'CHAT_LUONG_SOAN_THAO', N'SO_LAN_GUI_THAM_DINH', N'LAN', 1, CONVERT(decimal(18,2), 60), CONVERT(bit, 1), N'Chấm điểm theo số lần gửi hồ sơ thẩm định.', CAST(NULL AS nvarchar(max)), CONVERT(datetime2, '2026-10-06T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111102'), N'TIEN_DO_SOAN_THAO', N'Tiến độ soạn thảo', N'THOI_GIAN_SOAN_THAO', N'TY_LE_THOI_GIAN_THUC_TE', N'PERCENT', 2, CONVERT(decimal(18,2), 40), CONVERT(bit, 1), N'Chấm điểm theo tỷ lệ thời gian soạn thảo thực tế so với kế hoạch trên hồ sơ.', CAST(NULL AS nvarchar(max)), CONVERT(datetime2, '2026-10-06T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111103'), N'DAY_DU_TAI_LIEU', N'Đầy đủ thành phần tài liệu', N'THANH_PHAN_TAI_LIEU', N'SO_LUONG', N'FILE', 3, CONVERT(decimal(18,2), 20), CONVERT(bit, 1), N'Chấm điểm theo số tài liệu bắt buộc còn thiếu khi trình xử lý.', N'Áp dụng khi hệ thống đã cấu hình danh mục tài liệu bắt buộc.', CONVERT(datetime2, '2026-10-11T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111104'), N'TUAN_THU_QUY_TRINH', N'Tuân thủ quy trình xử lý', N'TUAN_THU_QUY_TRINH', N'SO_LAN', N'LAN', 4, CONVERT(decimal(18,2), 20), CONVERT(bit, 1), N'Chấm điểm theo số lần chuyển sai bước, thiếu điều kiện hoặc phải trả lại do lỗi nghiệp vụ.', CAST(NULL AS nvarchar(max)), CONVERT(datetime2, '2026-10-11T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111105'), N'KET_QUA_XU_LY', N'Kết quả xử lý cuối cùng', N'KET_QUA_XU_LY', N'DIEM_DANH_GIA', N'DIEM', 5, CONVERT(decimal(18,2), 20), CONVERT(bit, 1), N'Chấm điểm theo kết quả cuối cùng của hồ sơ sau khi hoàn thành quy trình.', CAST(NULL AS nvarchar(max)), CONVERT(datetime2, '2026-10-11T00:00:00'))
    ) AS source (Id, MaTieuChi, TenTieuChi, LoaiTieuChi, KieuGiaTri, DonViGiaTri, ThuTuSapXep, DiemToiDa, TrangThai, MoTa, GhiChu, SeedDate)
    ON target.Id = source.Id
    WHEN MATCHED THEN
        UPDATE SET
            MaTieuChi = source.MaTieuChi,
            TenTieuChi = source.TenTieuChi,
            LoaiTieuChi = source.LoaiTieuChi,
            KieuGiaTri = source.KieuGiaTri,
            DonViGiaTri = source.DonViGiaTri,
            ThuTuSapXep = source.ThuTuSapXep,
            DiemToiDa = source.DiemToiDa,
            TrangThai = source.TrangThai,
            MoTa = source.MoTa,
            GhiChu = source.GhiChu,
            UpdatedBy = CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'),
            UpdatedDate = source.SeedDate
    WHEN NOT MATCHED THEN
        INSERT (Id, MaTieuChi, TenTieuChi, LoaiTieuChi, KieuGiaTri, DonViGiaTri, ThuTuSapXep, DiemToiDa, TrangThai, MoTa, GhiChu, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
        VALUES (source.Id, source.MaTieuChi, source.TenTieuChi, source.LoaiTieuChi, source.KieuGiaTri, source.DonViGiaTri, source.ThuTuSapXep, source.DiemToiDa, source.TrangThai, source.MoTa, source.GhiChu, CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'), source.SeedDate, CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'), source.SeedDate);
END;

IF OBJECT_ID(N'dm.DanhMucTieuChiDiemMucs', N'U') IS NOT NULL
BEGIN
    MERGE dm.DanhMucTieuChiDiemMucs AS target
    USING (VALUES
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111201'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111101'), CONVERT(decimal(18,2), 1), CONVERT(decimal(18,2), 1), 1, 1, CONVERT(decimal(18,2), 60), N'Gửi thẩm định lần 1', 1, CONVERT(datetime2, '2026-10-06T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111202'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111101'), CONVERT(decimal(18,2), 2), CONVERT(decimal(18,2), 2), 1, 1, CONVERT(decimal(18,2), 45), N'Gửi thẩm định lần 2', 2, CONVERT(datetime2, '2026-10-06T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111203'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111101'), CONVERT(decimal(18,2), 3), CONVERT(decimal(18,2), 3), 1, 1, CONVERT(decimal(18,2), 30), N'Gửi thẩm định lần 3', 3, CONVERT(datetime2, '2026-10-06T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111204'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111101'), CONVERT(decimal(18,2), 4), CAST(NULL AS decimal(18,2)), 1, 1, CONVERT(decimal(18,2), 15), N'Gửi thẩm định từ lần 4', 4, CONVERT(datetime2, '2026-10-06T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111301'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111102'), CAST(NULL AS decimal(18,2)), CONVERT(decimal(18,2), 100), 1, 1, CONVERT(decimal(18,2), 40), N'Hoàn thành không quá 100% thời gian kế hoạch', 1, CONVERT(datetime2, '2026-10-06T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111302'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111102'), CONVERT(decimal(18,2), 100), CONVERT(decimal(18,2), 120), 0, 1, CONVERT(decimal(18,2), 30), N'Hoàn thành trên 100% đến 120% thời gian kế hoạch', 2, CONVERT(datetime2, '2026-10-06T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111303'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111102'), CONVERT(decimal(18,2), 120), CONVERT(decimal(18,2), 150), 0, 1, CONVERT(decimal(18,2), 20), N'Hoàn thành trên 120% đến 150% thời gian kế hoạch', 3, CONVERT(datetime2, '2026-10-06T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111304'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111102'), CONVERT(decimal(18,2), 150), CAST(NULL AS decimal(18,2)), 0, 1, CONVERT(decimal(18,2), 10), N'Hoàn thành trên 150% thời gian kế hoạch', 4, CONVERT(datetime2, '2026-10-06T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111401'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111103'), CONVERT(decimal(18,2), 0), CONVERT(decimal(18,2), 0), 1, 1, CONVERT(decimal(18,2), 20), N'Không thiếu tài liệu bắt buộc', 1, CONVERT(datetime2, '2026-10-11T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111402'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111103'), CONVERT(decimal(18,2), 1), CONVERT(decimal(18,2), 1), 1, 1, CONVERT(decimal(18,2), 15), N'Thiếu 1 tài liệu bắt buộc', 2, CONVERT(datetime2, '2026-10-11T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111403'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111103'), CONVERT(decimal(18,2), 2), CONVERT(decimal(18,2), 2), 1, 1, CONVERT(decimal(18,2), 10), N'Thiếu 2 tài liệu bắt buộc', 3, CONVERT(datetime2, '2026-10-11T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111404'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111103'), CONVERT(decimal(18,2), 3), CAST(NULL AS decimal(18,2)), 1, 1, CONVERT(decimal(18,2), 5), N'Thiếu từ 3 tài liệu bắt buộc', 4, CONVERT(datetime2, '2026-10-11T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111501'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111104'), CONVERT(decimal(18,2), 0), CONVERT(decimal(18,2), 0), 1, 1, CONVERT(decimal(18,2), 20), N'Không có lỗi quy trình', 1, CONVERT(datetime2, '2026-10-11T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111502'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111104'), CONVERT(decimal(18,2), 1), CONVERT(decimal(18,2), 1), 1, 1, CONVERT(decimal(18,2), 15), N'Có 1 lỗi quy trình', 2, CONVERT(datetime2, '2026-10-11T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111503'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111104'), CONVERT(decimal(18,2), 2), CONVERT(decimal(18,2), 2), 1, 1, CONVERT(decimal(18,2), 10), N'Có 2 lỗi quy trình', 3, CONVERT(datetime2, '2026-10-11T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111504'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111104'), CONVERT(decimal(18,2), 3), CAST(NULL AS decimal(18,2)), 1, 1, CONVERT(decimal(18,2), 5), N'Có từ 3 lỗi quy trình', 4, CONVERT(datetime2, '2026-10-11T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111601'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111105'), CONVERT(decimal(18,2), 100), CONVERT(decimal(18,2), 100), 1, 1, CONVERT(decimal(18,2), 20), N'Hoàn thành và ban hành đúng quy định', 1, CONVERT(datetime2, '2026-10-11T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111602'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111105'), CONVERT(decimal(18,2), 80), CONVERT(decimal(18,2), 99), 1, 1, CONVERT(decimal(18,2), 15), N'Hoàn thành nhưng có nội dung phải chỉnh sửa sau thẩm tra/thẩm định', 2, CONVERT(datetime2, '2026-10-11T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111603'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111105'), CONVERT(decimal(18,2), 50), CONVERT(decimal(18,2), 79), 1, 1, CONVERT(decimal(18,2), 10), N'Hoàn thành sau nhiều lần trả lại', 3, CONVERT(datetime2, '2026-10-11T00:00:00')),
        (CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111604'), CONVERT(uniqueidentifier, '11111111-1111-1111-1111-111111111105'), CONVERT(decimal(18,2), 0), CONVERT(decimal(18,2), 49), 1, 1, CONVERT(decimal(18,2), 5), N'Không hoàn thành hoặc không đủ điều kiện ban hành', 4, CONVERT(datetime2, '2026-10-11T00:00:00'))
    ) AS source (Id, DanhMucTieuChiDiemId, TuGiaTri, DenGiaTri, BaoGomTuGiaTri, BaoGomDenGiaTri, Diem, NhanHienThi, ThuTuSapXep, SeedDate)
    ON target.Id = source.Id
    WHEN MATCHED THEN
        UPDATE SET
            DanhMucTieuChiDiemId = source.DanhMucTieuChiDiemId,
            TuGiaTri = source.TuGiaTri,
            DenGiaTri = source.DenGiaTri,
            BaoGomTuGiaTri = source.BaoGomTuGiaTri,
            BaoGomDenGiaTri = source.BaoGomDenGiaTri,
            Diem = source.Diem,
            NhanHienThi = source.NhanHienThi,
            ThuTuSapXep = source.ThuTuSapXep,
            TrangThai = 1,
            UpdatedBy = CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'),
            UpdatedDate = source.SeedDate
    WHEN NOT MATCHED THEN
        INSERT (Id, DanhMucTieuChiDiemId, TuGiaTri, DenGiaTri, BaoGomTuGiaTri, BaoGomDenGiaTri, Diem, NhanHienThi, ThuTuSapXep, TrangThai, GhiChu, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
        VALUES (source.Id, source.DanhMucTieuChiDiemId, source.TuGiaTri, source.DenGiaTri, source.BaoGomTuGiaTri, source.BaoGomDenGiaTri, source.Diem, source.NhanHienThi, source.ThuTuSapXep, 1, NULL, CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'), source.SeedDate, CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'), source.SeedDate);
END;

IF OBJECT_ID(N'dm.DanhMucTrangThais', N'U') IS NOT NULL
BEGIN
    MERGE dm.DanhMucTrangThais AS target
    USING (VALUES
        (CONVERT(uniqueidentifier, '22222222-2222-2222-2222-222222222201'), N'CHAM_DIEM_HO_SO', N'NHAP', N'Nháp', N'#6B7280', 1, N'Bảng điểm đang được tạo hoặc tính lại.'),
        (CONVERT(uniqueidentifier, '22222222-2222-2222-2222-222222222202'), N'CHAM_DIEM_HO_SO', N'DA_CHOT', N'Đã chốt', N'#16A34A', 2, N'Kết quả chấm điểm chính thức.'),
        (CONVERT(uniqueidentifier, '22222222-2222-2222-2222-222222222203'), N'CHAM_DIEM_HO_SO', N'DA_HUY', N'Đã hủy', N'#DC2626', 3, N'Bảng điểm đã bị hủy sau khi chốt.')
    ) AS source (Id, NhomTrangThai, MaTrangThai, TenTrangThai, MaMauHex, ThuTuSapXep, MoTa)
    ON target.Id = source.Id
    WHEN MATCHED THEN
        UPDATE SET
            NhomTrangThai = source.NhomTrangThai,
            MaTrangThai = source.MaTrangThai,
            TenTrangThai = source.TenTrangThai,
            MaMauHex = source.MaMauHex,
            ThuTuSapXep = source.ThuTuSapXep,
            TrangThai = 1,
            MoTa = source.MoTa,
            UpdatedBy = CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'),
            UpdatedDate = CONVERT(datetime2, '2026-10-06T00:00:00')
    WHEN NOT MATCHED THEN
        INSERT (Id, NhomTrangThai, MaTrangThai, TenTrangThai, MaMauHex, ThuTuSapXep, TrangThai, MoTa, GhiChu, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
        VALUES (source.Id, source.NhomTrangThai, source.MaTrangThai, source.TenTrangThai, source.MaMauHex, source.ThuTuSapXep, 1, source.MoTa, NULL, CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'), CONVERT(datetime2, '2026-10-06T00:00:00'), CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000'), CONVERT(datetime2, '2026-10-06T00:00:00'));

    DELETE FROM dm.DanhMucTrangThais
    WHERE NhomTrangThai = N'CHAM_DIEM_HO_SO'
      AND MaTrangThai IN (N'NHAP', N'DA_CHOT', N'DA_HUY')
      AND Id NOT IN (
          CONVERT(uniqueidentifier, '22222222-2222-2222-2222-222222222201'),
          CONVERT(uniqueidentifier, '22222222-2222-2222-2222-222222222202'),
          CONVERT(uniqueidentifier, '22222222-2222-2222-2222-222222222203')
      );
END;

IF OBJECT_ID(N'dbo.DashboardWidgets', N'U') IS NOT NULL
BEGIN
    MERGE dbo.DashboardWidgets AS target
    USING (VALUES
        (N'TONG_QUAN_DIEU_HANH', N'Tổng quan điều hành', N'TONG_QUAN', N'metric', N'Tổng cảnh báo, hồ sơ và nhiệm vụ cần theo dõi.'),
        (N'TIEN_DO_XAY_DUNG_VAN_BAN', N'Tiến độ xây dựng văn bản', N'XAY_DUNG_VAN_BAN', N'bar', N'Theo dõi văn bản đúng hạn, sắp hạn và quá hạn.'),
        (N'CANH_BAO_THONG_MINH', N'Cảnh báo thông minh', N'CANH_BAO', N'donut', N'Tổng hợp cảnh báo theo trạng thái và mức độ.'),
        (N'TO_CHUC_THI_HANH_PHAP_LUAT', N'Tổ chức thi hành pháp luật', N'THI_HANH_PHAP_LUAT', N'bar', N'Theo dõi nhiệm vụ thi hành pháp luật theo tình trạng xử lý.'),
        (N'BAO_CAO_THONG_KE', N'Báo cáo thống kê', N'BAO_CAO', N'table', N'Truy cập nhanh số liệu báo cáo thống kê.'),
        (N'HO_SO_SAP_HAN', N'Hồ sơ sắp hạn', N'CANH_BAO', N'list', N'Danh sách hồ sơ hoặc nhiệm vụ sắp đến hạn.'),
        (N'HO_SO_QUA_HAN', N'Hồ sơ quá hạn', N'CANH_BAO', N'list', N'Danh sách hồ sơ hoặc nhiệm vụ đã quá hạn.'),
        (N'NHIEM_VU_QUA_HAN', N'Nhiệm vụ quá hạn', N'THI_HANH_PHAP_LUAT', N'list', N'Nhiệm vụ thi hành pháp luật chưa hoàn thành đúng hạn.'),
        (N'TY_LE_BAN_HANH_THEO_DON_VI', N'Tỷ lệ ban hành theo đơn vị', N'XAY_DUNG_VAN_BAN', N'bar', N'So sánh tỷ lệ hoàn thành ban hành văn bản theo đơn vị.'),
        (N'TY_LE_HOAN_THANH_THEO_DON_VI', N'Tỷ lệ hoàn thành theo đơn vị', N'THI_HANH_PHAP_LUAT', N'bar', N'So sánh mức độ hoàn thành nhiệm vụ theo đơn vị.')
    ) AS source (MaWidget, TenWidget, NhomWidget, LoaiWidget, MoTa)
    ON target.MaWidget = source.MaWidget
    WHEN MATCHED THEN
        UPDATE SET
            TenWidget = source.TenWidget,
            NhomWidget = source.NhomWidget,
            LoaiWidget = source.LoaiWidget,
            MoTa = source.MoTa,
            TrangThai = 1,
            CauHinhMacDinhJson = COALESCE(target.CauHinhMacDinhJson, N'{}'),
            UpdatedAt = SYSUTCDATETIME(),
            UpdatedBy = N'SYSTEM',
            IsDeleted = 0
    WHEN NOT MATCHED THEN
        INSERT (Id, MaWidget, TenWidget, NhomWidget, LoaiWidget, MoTa, TrangThai, CauHinhMacDinhJson, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
        VALUES (NEWID(), source.MaWidget, source.TenWidget, source.NhomWidget, source.LoaiWidget, source.MoTa, 1, N'{}', SYSUTCDATETIME(), N'SYSTEM', NULL, NULL, 0);
END;

IF OBJECT_ID(N'dbo.__EFMigrationsHistory', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'20261005040000_AddDangKyListIndexes')
        INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES (N'20261005040000_AddDangKyListIndexes', N'10.0.12');

    IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'20261006071033_SeedXayDungVanBanScoringCriteria')
        INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES (N'20261006071033_SeedXayDungVanBanScoringCriteria', N'10.0.12');

    IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'20261006072402_AddStatusGroupForScoring')
        INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES (N'20261006072402_AddStatusGroupForScoring', N'10.0.12');

    IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'20261011102000_SeedExpandedXayDungVanBanScoringCriteria')
        INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES (N'20261011102000_SeedExpandedXayDungVanBanScoringCriteria', N'10.0.12');
END;

SELECT N'DashboardWidgets' AS Item, COUNT(*) AS CountValue FROM dbo.DashboardWidgets
UNION ALL
SELECT N'DashboardWidgetCauHinhs', COUNT(*) FROM dbo.DashboardWidgetCauHinhs
UNION ALL
SELECT N'DanhMucTieuChiDiems', COUNT(*) FROM dm.DanhMucTieuChiDiems WHERE Id IN (
    '11111111-1111-1111-1111-111111111101',
    '11111111-1111-1111-1111-111111111102',
    '11111111-1111-1111-1111-111111111103',
    '11111111-1111-1111-1111-111111111104',
    '11111111-1111-1111-1111-111111111105')
UNION ALL
SELECT N'DanhMucTieuChiDiemMucs', COUNT(*) FROM dm.DanhMucTieuChiDiemMucs WHERE DanhMucTieuChiDiemId IN (
    '11111111-1111-1111-1111-111111111101',
    '11111111-1111-1111-1111-111111111102',
    '11111111-1111-1111-1111-111111111103',
    '11111111-1111-1111-1111-111111111104',
    '11111111-1111-1111-1111-111111111105')
UNION ALL
SELECT N'TrangThaiChamDiem', COUNT(*) FROM dm.DanhMucTrangThais WHERE NhomTrangThai = N'CHAM_DIEM_HO_SO';

SELECT MigrationId
FROM dbo.__EFMigrationsHistory
WHERE MigrationId IN (
    N'20261005040000_AddDangKyListIndexes',
    N'20261006071033_SeedXayDungVanBanScoringCriteria',
    N'20261006072402_AddStatusGroupForScoring',
    N'20261011102000_SeedExpandedXayDungVanBanScoringCriteria',
    N'20261011123000_AddDashboardWidgetConfiguration',
    N'20261011124000_SeedKhaiThacDuLieuDashboardRoleActions',
    N'20261011133000_SeedKhaiThacDuLieuBaoCaoRoleActions',
    N'20261011134000_SeedKhaiThacDuLieuDashboardCanhBaoRoleActions')
ORDER BY MigrationId;

PRINT N'Database update completed.';
