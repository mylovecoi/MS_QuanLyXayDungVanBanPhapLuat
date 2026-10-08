/*
    Seed quy trình nghiệp vụ theo các tài liệu QT.
    Không seed QT khai thác dữ liệu / tổng hợp vì đây là nhóm dashboard, tra cứu, báo cáo.
*/

IF OBJECT_ID(N'[dm].[DanhMucTrangThaiNghiepVuQuyTrinhs]', N'U') IS NULL
BEGIN
    CREATE TABLE [dm].[DanhMucTrangThaiNghiepVuQuyTrinhs]
    (
        [Id] uniqueidentifier NOT NULL CONSTRAINT [PK_DanhMucTrangThaiNghiepVuQuyTrinhs] PRIMARY KEY,
        [QuyTrinhSoanThaoId] uniqueidentifier NOT NULL,
        [BuocQuyTrinhId] uniqueidentifier NULL,
        [MaTrangThai] nvarchar(100) NOT NULL,
        [TenTrangThai] nvarchar(255) NOT NULL,
        [MaMauHex] nvarchar(50) NOT NULL,
        [ThuTuSapXep] int NOT NULL CONSTRAINT [DF_DMTTNVQT_ThuTu] DEFAULT 1,
        [LaTrangThaiKhoiTao] bit NOT NULL CONSTRAINT [DF_DMTTNVQT_KhoiTao] DEFAULT 0,
        [LaTrangThaiKetThuc] bit NOT NULL CONSTRAINT [DF_DMTTNVQT_KetThuc] DEFAULT 0,
        [TrangThai] bit NOT NULL CONSTRAINT [DF_DMTTNVQT_TrangThai] DEFAULT 1,
        [MoTa] nvarchar(max) NULL,
        [GhiChu] nvarchar(max) NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedBy] uniqueidentifier NOT NULL,
        [UpdatedDate] datetime2 NOT NULL,
        CONSTRAINT [FK_DMTTNVQT_QuyTrinh] FOREIGN KEY ([QuyTrinhSoanThaoId])
            REFERENCES [dm].[DanhMucQuyTrinhSoanThaos]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_DMTTNVQT_Buoc] FOREIGN KEY ([BuocQuyTrinhId])
            REFERENCES [dm].[DanhMucBuocQuyTrinhs]([Id]) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX [IX_DMTTNVQT_QuyTrinh_MaTrangThai]
        ON [dm].[DanhMucTrangThaiNghiepVuQuyTrinhs]([QuyTrinhSoanThaoId], [MaTrangThai]);
END;
GO

DECLARE @Now datetime2 = SYSUTCDATETIME();
DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';

DECLARE @QuyTrinh TABLE
(
    Id uniqueidentifier NOT NULL,
    MaQuyTrinh nvarchar(250) NOT NULL,
    TenQuyTrinh nvarchar(500) NOT NULL,
    LoaiQuyTrinh nvarchar(250) NOT NULL,
    CapApDung nvarchar(250) NULL,
    PhienBan int NOT NULL,
    MoTa nvarchar(max) NULL
);

INSERT INTO @QuyTrinh VALUES
('33333333-3333-3333-3333-333333333301', N'DXDM_DKXD_QPPL', N'Đề xuất danh mục / Đăng ký xây dựng QPPL', N'DangKyXayDungQPPL', N'CapTinh', 1, N'Quy trình đăng ký/đề xuất danh mục xây dựng văn bản QPPL cấp tỉnh.'),
('44444444-4444-4444-4444-444444444101', N'XD_QD_UBND_TINH', N'Xây dựng văn bản QPPL - Quyết định UBND tỉnh', N'XayDungQuyetDinhUBND', N'CapTinh', 1, N'Quy trình xây dựng Quyết định UBND tỉnh từ soạn thảo đến cập nhật kết quả ban hành.'),
('44444444-4444-4444-4444-444444444201', N'XD_NQ_HDND_TINH', N'Xây dựng văn bản QPPL - Nghị quyết HĐND tỉnh', N'XayDungNghiQuyetHDND', N'CapTinh', 1, N'Quy trình xây dựng Nghị quyết HĐND tỉnh từ soạn thảo đến thông qua, ban hành.'),
('44444444-4444-4444-4444-444444444301', N'TC_THI_HANH_PHAP_LUAT', N'Tổ chức thi hành pháp luật', N'ThiHanhPhapLuat', N'CapTinh', 1, N'Quy trình lập kế hoạch, giao nhiệm vụ, cập nhật tiến độ, đánh giá và tổng kết thi hành pháp luật.'),
('44444444-4444-4444-4444-444444444401', N'KS_THI_HANH_PHAP_LUAT', N'Khảo sát tình hình thi hành pháp luật', N'KhaoSatThiHanhPhapLuat', N'CapTinh', 1, N'Quy trình tạo cuộc khảo sát, phát hành, theo dõi trả lời, đánh giá và lập báo cáo kết quả.');

MERGE [dm].[DanhMucQuyTrinhSoanThaos] AS target
USING @QuyTrinh AS source
ON target.[MaQuyTrinh] = source.[MaQuyTrinh]
WHEN MATCHED THEN UPDATE SET
    [TenQuyTrinh] = source.[TenQuyTrinh],
    [LoaiQuyTrinh] = source.[LoaiQuyTrinh],
    [CapApDung] = source.[CapApDung],
    [PhienBan] = source.[PhienBan],
    [TrangThai] = 1,
    [MoTa] = source.[MoTa],
    [UpdatedBy] = @SystemUserId,
    [UpdatedDate] = @Now
WHEN NOT MATCHED THEN INSERT
    ([Id], [MaQuyTrinh], [TenQuyTrinh], [LoaiQuyTrinh], [CapApDung], [PhienBan], [TrangThai], [MoTa], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
VALUES
    (source.[Id], source.[MaQuyTrinh], source.[TenQuyTrinh], source.[LoaiQuyTrinh], source.[CapApDung], source.[PhienBan], 1, source.[MoTa], @SystemUserId, @Now, @SystemUserId, @Now);

DECLARE @QuyTrinhIds TABLE (MaQuyTrinh nvarchar(250) NOT NULL PRIMARY KEY, Id uniqueidentifier NOT NULL);
INSERT INTO @QuyTrinhIds
SELECT [MaQuyTrinh], [Id]
FROM [dm].[DanhMucQuyTrinhSoanThaos]
WHERE [MaQuyTrinh] IN (N'DXDM_DKXD_QPPL', N'XD_QD_UBND_TINH', N'XD_NQ_HDND_TINH', N'TC_THI_HANH_PHAP_LUAT', N'KS_THI_HANH_PHAP_LUAT');

DECLARE @Buoc TABLE
(
    Id uniqueidentifier NOT NULL,
    MaQuyTrinh nvarchar(250) NOT NULL,
    MaBuoc nvarchar(250) NOT NULL,
    TenBuoc nvarchar(500) NOT NULL,
    ThuTuSapXep int NOT NULL,
    LoaiBuoc nvarchar(100) NOT NULL,
    YeuCauFileDinhKem bit NOT NULL,
    ChoPhepQuayLui bit NOT NULL,
    MoTa nvarchar(max) NULL
);

INSERT INTO @Buoc VALUES
-- Đăng ký xây dựng QPPL
('33333333-3333-3333-3333-333333333311', N'DXDM_DKXD_QPPL', N'LAP_HO_SO', N'Lập hồ sơ đề nghị/đăng ký', 1, N'NhapLieu', 0, 0, N'Cơ quan chủ trì soạn thảo lập hồ sơ đề xuất danh mục/đăng ký trên phần mềm.'),
('33333333-3333-3333-3333-333333333312', N'DXDM_DKXD_QPPL', N'TRINH_HO_SO', N'Trình hồ sơ/Chờ phê duyệt', 2, N'XuLy', 0, 1, N'Hồ sơ đã trình sang cơ quan có thẩm quyền phê duyệt.'),
('33333333-3333-3333-3333-333333333313', N'DXDM_DKXD_QPPL', N'PHE_DUYET', N'Phê duyệt hồ sơ', 3, N'PheDuyet', 0, 1, N'Cơ quan thẩm quyền phê duyệt phối hợp Sở Tư pháp xem xét hồ sơ.'),
('33333333-3333-3333-3333-333333333314', N'DXDM_DKXD_QPPL', N'CAP_NHAT_KET_QUA', N'Cập nhật kết quả phê duyệt', 4, N'NhapLieu', 1, 0, N'Cơ quan chủ trì cập nhật kết quả phê duyệt lên phần mềm.'),
('33333333-3333-3333-3333-333333333315', N'DXDM_DKXD_QPPL', N'HOAN_THANH', N'Hoàn thành', 5, N'KetThuc', 0, 0, N'Hoàn thành quy trình đăng ký xây dựng văn bản.'),

-- Xây dựng Quyết định UBND
('44444444-4444-4444-4444-444444444111', N'XD_QD_UBND_TINH', N'SOAN_THAO_LAY_Y_KIEN', N'Tổ chức soạn thảo, lấy ý kiến góp ý', 1, N'NhapLieu', 1, 0, N'Tạo hồ sơ xây dựng dự thảo Quyết định và đính kèm tài liệu dự thảo, tổng hợp ý kiến.'),
('44444444-4444-4444-4444-444444444112', N'XD_QD_UBND_TINH', N'GUI_THAM_DINH', N'Gửi hồ sơ thẩm định', 2, N'Forward', 1, 1, N'Tạo hồ sơ dự thảo, bổ sung tờ trình và gửi Sở Tư pháp thẩm định.'),
('44444444-4444-4444-4444-444444444113', N'XD_QD_UBND_TINH', N'THAM_DINH', N'Thẩm định văn bản', 3, N'ThamDinh', 1, 1, N'Sở Tư pháp tiếp nhận, thẩm định và gửi báo cáo kết quả thẩm định.'),
('44444444-4444-4444-4444-444444444114', N'XD_QD_UBND_TINH', N'TRINH_UBND', N'Trình UBND tỉnh hồ sơ dự thảo', 4, N'Forward', 1, 1, N'Lập và lưu trữ hồ sơ trình UBND tỉnh xem xét, thông qua dự thảo Quyết định.'),
('44444444-4444-4444-4444-444444444115', N'XD_QD_UBND_TINH', N'LAY_Y_KIEN_UBND', N'Lấy ý kiến thành viên UBND tỉnh', 5, N'LayYKien', 1, 1, N'Cập nhật tổng hợp ý kiến thành viên UBND tỉnh và giải trình bổ sung nếu có ý kiến khác nhau.'),
('44444444-4444-4444-4444-444444444116', N'XD_QD_UBND_TINH', N'BAN_HANH', N'Quyết định ban hành', 6, N'KetThuc', 1, 0, N'Cập nhật Quyết định được phê duyệt/ban hành.'),

-- Xây dựng Nghị quyết HĐND
('44444444-4444-4444-4444-444444444211', N'XD_NQ_HDND_TINH', N'SOAN_THAO_LAY_Y_KIEN', N'Tổ chức soạn thảo, lấy ý kiến góp ý', 1, N'NhapLieu', 1, 0, N'Tạo hồ sơ xây dựng dự thảo Nghị quyết và đính kèm tài liệu liên quan.'),
('44444444-4444-4444-4444-444444444212', N'XD_NQ_HDND_TINH', N'GUI_THAM_DINH', N'Gửi hồ sơ thẩm định', 2, N'Forward', 1, 1, N'Tạo hồ sơ dự thảo, bổ sung tờ trình và gửi Sở Tư pháp thẩm định.'),
('44444444-4444-4444-4444-444444444213', N'XD_NQ_HDND_TINH', N'THAM_DINH', N'Thẩm định văn bản', 3, N'ThamDinh', 1, 1, N'Sở Tư pháp tiếp nhận, thẩm định và gửi báo cáo kết quả thẩm định.'),
('44444444-4444-4444-4444-444444444214', N'XD_NQ_HDND_TINH', N'TRINH_UBND_CHO_Y_KIEN', N'Trình UBND tỉnh cho ý kiến', 4, N'Forward', 1, 1, N'Lập và lưu trữ hồ sơ trình UBND tỉnh xem xét, cho ý kiến dự thảo Nghị quyết.'),
('44444444-4444-4444-4444-444444444215', N'XD_NQ_HDND_TINH', N'LAY_Y_KIEN_UBND', N'Lấy ý kiến thành viên UBND tỉnh', 5, N'LayYKien', 1, 1, N'Cập nhật ý kiến thành viên UBND tỉnh và hoàn thiện hồ sơ trình HĐND.'),
('44444444-4444-4444-4444-444444444216', N'XD_NQ_HDND_TINH', N'TRINH_HDND_THAM_TRA', N'Trình HĐND tỉnh thẩm tra', 6, N'ThamTra', 1, 1, N'Cập nhật hồ sơ trình HĐND tỉnh thẩm tra và báo cáo thẩm tra.'),
('44444444-4444-4444-4444-444444444217', N'XD_NQ_HDND_TINH', N'THONG_QUA_BAN_HANH', N'Thông qua và ban hành Nghị quyết', 7, N'KetThuc', 1, 0, N'Cập nhật Nghị quyết được HĐND tỉnh thông qua/ban hành.'),

-- Tổ chức thi hành pháp luật
('44444444-4444-4444-4444-444444444311', N'TC_THI_HANH_PHAP_LUAT', N'TAO_KE_HOACH', N'Tạo kế hoạch', 1, N'NhapLieu', 0, 0, N'Cơ quan chủ trì tạo kế hoạch và phân chia nội dung thi hành pháp luật cho các đơn vị.'),
('44444444-4444-4444-4444-444444444312', N'TC_THI_HANH_PHAP_LUAT', N'THUC_HIEN_NOI_DUNG', N'Thực hiện các nội dung được giao', 2, N'XuLy', 1, 1, N'Các đơn vị được giao cập nhật nội dung, dữ liệu và trạng thái thực hiện.'),
('44444444-4444-4444-4444-444444444313', N'TC_THI_HANH_PHAP_LUAT', N'DANH_GIA_KET_QUA', N'Đánh giá kết quả', 3, N'DanhGia', 1, 1, N'Sở Tư pháp đánh giá kết quả thực hiện; yêu cầu bổ sung nếu chưa đạt.'),
('44444444-4444-4444-4444-444444444314', N'TC_THI_HANH_PHAP_LUAT', N'TONG_HOP_KET_QUA', N'Tổng hợp kết quả thực hiện', 4, N'KetThuc', 1, 0, N'Tổng kết các nội dung thực hiện và lập báo cáo tổng hợp kết quả kế hoạch.'),

-- Khảo sát thi hành pháp luật
('44444444-4444-4444-4444-444444444411', N'KS_THI_HANH_PHAP_LUAT', N'TAO_CUOC_KHAO_SAT', N'Tạo cuộc khảo sát', 1, N'NhapLieu', 1, 0, N'Sở Tư pháp tạo và thiết lập cuộc khảo sát, đối tượng khảo sát và biểu mẫu.'),
('44444444-4444-4444-4444-444444444412', N'KS_THI_HANH_PHAP_LUAT', N'THUC_HIEN_KHAO_SAT', N'Thực hiện khảo sát', 2, N'XuLy', 1, 1, N'Đối tượng khảo sát trả lời biểu mẫu; Sở Tư pháp theo dõi, đôn đốc tiến độ.'),
('44444444-4444-4444-4444-444444444413', N'KS_THI_HANH_PHAP_LUAT', N'DANH_GIA_KET_QUA', N'Đánh giá kết quả khảo sát', 3, N'DanhGia', 1, 1, N'Sở Tư pháp rà soát tính đầy đủ, hợp lệ của dữ liệu và yêu cầu bổ sung nếu cần.'),
('44444444-4444-4444-4444-444444444414', N'KS_THI_HANH_PHAP_LUAT', N'TONG_HOP_BAO_CAO', N'Tổng hợp và báo cáo kết quả khảo sát', 4, N'KetThuc', 1, 0, N'Tổng hợp, phân tích và kết xuất báo cáo kết quả khảo sát.');

MERGE [dm].[DanhMucBuocQuyTrinhs] AS target
USING (
    SELECT b.*, q.Id AS QuyTrinhId
    FROM @Buoc b
    JOIN @QuyTrinhIds q ON q.MaQuyTrinh = b.MaQuyTrinh
) AS source
ON target.[QuyTrinhSoanThaoId] = source.[QuyTrinhId] AND target.[MaBuoc] = source.[MaBuoc]
WHEN MATCHED THEN UPDATE SET
    [TenBuoc] = source.[TenBuoc],
    [ThuTuSapXep] = source.[ThuTuSapXep],
    [LoaiBuoc] = source.[LoaiBuoc],
    [BatBuoc] = 1,
    [ChoPhepBoQua] = 0,
    [ChoPhepQuayLui] = source.[ChoPhepQuayLui],
    [YeuCauFileDinhKem] = source.[YeuCauFileDinhKem],
    [SoLanTraLaiToiDa] = CASE WHEN source.[ChoPhepQuayLui] = 1 THEN 3 ELSE 0 END,
    [MoTa] = source.[MoTa],
    [UpdatedBy] = @SystemUserId,
    [UpdatedDate] = @Now
WHEN NOT MATCHED THEN INSERT
    ([Id], [QuyTrinhSoanThaoId], [MaBuoc], [TenBuoc], [ThuTuSapXep], [LoaiBuoc], [BatBuoc], [ChoPhepBoQua], [ChoPhepQuayLui], [YeuCauFileDinhKem], [SoLanTraLaiToiDa], [MoTa], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
VALUES
    (source.[Id], source.[QuyTrinhId], source.[MaBuoc], source.[TenBuoc], source.[ThuTuSapXep], source.[LoaiBuoc], 1, 0, source.[ChoPhepQuayLui], source.[YeuCauFileDinhKem], CASE WHEN source.[ChoPhepQuayLui] = 1 THEN 3 ELSE 0 END, source.[MoTa], @SystemUserId, @Now, @SystemUserId, @Now);

DECLARE @BuocIds TABLE (MaQuyTrinh nvarchar(250) NOT NULL, MaBuoc nvarchar(250) NOT NULL, Id uniqueidentifier NOT NULL, PRIMARY KEY (MaQuyTrinh, MaBuoc));
INSERT INTO @BuocIds
SELECT q.[MaQuyTrinh], b.[MaBuoc], b.[Id]
FROM [dm].[DanhMucBuocQuyTrinhs] b
JOIN @QuyTrinhIds q ON q.Id = b.[QuyTrinhSoanThaoId];

DECLARE @TrangThai TABLE
(
    Id uniqueidentifier NOT NULL,
    MaQuyTrinh nvarchar(250) NOT NULL,
    MaBuoc nvarchar(250) NULL,
    MaTrangThai nvarchar(100) NOT NULL,
    TenTrangThai nvarchar(255) NOT NULL,
    MaMauHex nvarchar(50) NOT NULL,
    ThuTuSapXep int NOT NULL,
    LaTrangThaiKhoiTao bit NOT NULL,
    LaTrangThaiKetThuc bit NOT NULL,
    MoTa nvarchar(max) NULL
);

INSERT INTO @TrangThai VALUES
('55555555-5555-5555-5555-555555555101', N'DXDM_DKXD_QPPL', N'LAP_HO_SO', N'DANG_SOAN_THAO', N'Đang soạn thảo', N'#2563EB', 1, 1, 0, N'Hồ sơ đang được lập/cập nhật.'),
('55555555-5555-5555-5555-555555555102', N'DXDM_DKXD_QPPL', N'TRINH_HO_SO', N'DA_TRINH_PHE_DUYET', N'Đã trình phê duyệt', N'#F59E0B', 2, 0, 0, N'Hồ sơ đã trình cơ quan thẩm quyền.'),
('55555555-5555-5555-5555-555555555103', N'DXDM_DKXD_QPPL', N'LAP_HO_SO', N'BI_TRA_LAI', N'Bị trả lại', N'#EF4444', 3, 0, 0, N'Hồ sơ bị trả lại để bổ sung.'),
('55555555-5555-5555-5555-555555555104', N'DXDM_DKXD_QPPL', N'CAP_NHAT_KET_QUA', N'DA_PHE_DUYET', N'Đã phê duyệt', N'#10B981', 4, 0, 0, N'Hồ sơ đăng ký được phê duyệt.'),
('55555555-5555-5555-5555-555555555105', N'DXDM_DKXD_QPPL', N'HOAN_THANH', N'HOAN_THANH', N'Hoàn thành', N'#16A34A', 5, 0, 1, N'Hoàn thành quy trình đăng ký.'),
('55555555-5555-5555-5555-555555555106', N'DXDM_DKXD_QPPL', N'HOAN_THANH', N'KHONG_PHE_DUYET', N'Không phê duyệt', N'#DC2626', 6, 0, 1, N'Hồ sơ không được phê duyệt.'),

('55555555-5555-5555-5555-555555555201', N'XD_QD_UBND_TINH', N'SOAN_THAO_LAY_Y_KIEN', N'DANG_SOAN_THAO', N'Đang soạn thảo', N'#2563EB', 1, 1, 0, N'Hồ sơ dự thảo Quyết định đang được soạn thảo, lấy ý kiến.'),
('55555555-5555-5555-5555-555555555202', N'XD_QD_UBND_TINH', N'GUI_THAM_DINH', N'CHO_THAM_DINH', N'Chờ thẩm định', N'#F59E0B', 2, 0, 0, N'Hồ sơ đã gửi Sở Tư pháp thẩm định.'),
('55555555-5555-5555-5555-555555555203', N'XD_QD_UBND_TINH', N'THAM_DINH', N'DANG_THAM_DINH', N'Đang thẩm định', N'#7C3AED', 3, 0, 0, N'Sở Tư pháp đang thẩm định hồ sơ.'),
('55555555-5555-5555-5555-555555555204', N'XD_QD_UBND_TINH', N'SOAN_THAO_LAY_Y_KIEN', N'YEU_CAU_BO_SUNG', N'Yêu cầu bổ sung', N'#EF4444', 4, 0, 0, N'Hồ sơ cần bổ sung/cập nhật lại.'),
('55555555-5555-5555-5555-555555555205', N'XD_QD_UBND_TINH', N'TRINH_UBND', N'DA_THAM_DINH', N'Đã thẩm định', N'#0EA5E9', 5, 0, 0, N'Đã có báo cáo kết quả thẩm định.'),
('55555555-5555-5555-5555-555555555206', N'XD_QD_UBND_TINH', N'LAY_Y_KIEN_UBND', N'CHO_Y_KIEN_UBND', N'Chờ ý kiến UBND', N'#F97316', 6, 0, 0, N'Hồ sơ đang lấy ý kiến thành viên UBND tỉnh.'),
('55555555-5555-5555-5555-555555555207', N'XD_QD_UBND_TINH', N'BAN_HANH', N'DA_BAN_HANH', N'Đã ban hành', N'#16A34A', 7, 0, 1, N'Quyết định đã được ban hành.'),

('55555555-5555-5555-5555-555555555301', N'XD_NQ_HDND_TINH', N'SOAN_THAO_LAY_Y_KIEN', N'DANG_SOAN_THAO', N'Đang soạn thảo', N'#2563EB', 1, 1, 0, N'Hồ sơ dự thảo Nghị quyết đang được soạn thảo, lấy ý kiến.'),
('55555555-5555-5555-5555-555555555302', N'XD_NQ_HDND_TINH', N'GUI_THAM_DINH', N'CHO_THAM_DINH', N'Chờ thẩm định', N'#F59E0B', 2, 0, 0, N'Hồ sơ đã gửi Sở Tư pháp thẩm định.'),
('55555555-5555-5555-5555-555555555303', N'XD_NQ_HDND_TINH', N'THAM_DINH', N'DANG_THAM_DINH', N'Đang thẩm định', N'#7C3AED', 3, 0, 0, N'Sở Tư pháp đang thẩm định hồ sơ.'),
('55555555-5555-5555-5555-555555555304', N'XD_NQ_HDND_TINH', N'SOAN_THAO_LAY_Y_KIEN', N'YEU_CAU_BO_SUNG', N'Yêu cầu bổ sung', N'#EF4444', 4, 0, 0, N'Hồ sơ cần bổ sung/cập nhật lại.'),
('55555555-5555-5555-5555-555555555305', N'XD_NQ_HDND_TINH', N'TRINH_UBND_CHO_Y_KIEN', N'DA_THAM_DINH', N'Đã thẩm định', N'#0EA5E9', 5, 0, 0, N'Đã có báo cáo kết quả thẩm định.'),
('55555555-5555-5555-5555-555555555306', N'XD_NQ_HDND_TINH', N'LAY_Y_KIEN_UBND', N'CHO_Y_KIEN_UBND', N'Chờ ý kiến UBND', N'#F97316', 6, 0, 0, N'Hồ sơ đang lấy ý kiến thành viên UBND tỉnh.'),
('55555555-5555-5555-5555-555555555307', N'XD_NQ_HDND_TINH', N'TRINH_HDND_THAM_TRA', N'CHO_THAM_TRA_HDND', N'Chờ thẩm tra HĐND', N'#8B5CF6', 7, 0, 0, N'Hồ sơ đã trình HĐND tỉnh thẩm tra.'),
('55555555-5555-5555-5555-555555555308', N'XD_NQ_HDND_TINH', N'THONG_QUA_BAN_HANH', N'DA_THONG_QUA_BAN_HANH', N'Đã thông qua, ban hành', N'#16A34A', 8, 0, 1, N'Nghị quyết đã được thông qua/ban hành.'),

('55555555-5555-5555-5555-555555555401', N'TC_THI_HANH_PHAP_LUAT', N'TAO_KE_HOACH', N'NHAP', N'Nhập', N'#6B7280', 1, 1, 0, N'Kế hoạch đang được tạo/cập nhật.'),
('55555555-5555-5555-5555-555555555402', N'TC_THI_HANH_PHAP_LUAT', N'THUC_HIEN_NOI_DUNG', N'DANG_THUC_HIEN', N'Đang thực hiện', N'#2563EB', 2, 0, 0, N'Các đơn vị đang thực hiện nội dung được giao.'),
('55555555-5555-5555-5555-555555555403', N'TC_THI_HANH_PHAP_LUAT', N'THUC_HIEN_NOI_DUNG', N'YEU_CAU_BO_SUNG', N'Yêu cầu bổ sung', N'#EF4444', 3, 0, 0, N'Nội dung cần bổ sung dữ liệu thực hiện.'),
('55555555-5555-5555-5555-555555555404', N'TC_THI_HANH_PHAP_LUAT', N'DANH_GIA_KET_QUA', N'CHO_DANH_GIA', N'Chờ đánh giá', N'#F59E0B', 4, 0, 0, N'Nội dung đã gửi và chờ Sở Tư pháp đánh giá.'),
('55555555-5555-5555-5555-555555555405', N'TC_THI_HANH_PHAP_LUAT', N'TONG_HOP_KET_QUA', N'HOAN_THANH', N'Hoàn thành', N'#16A34A', 5, 0, 1, N'Hoàn thành tổng hợp kết quả thi hành pháp luật.'),

('55555555-5555-5555-5555-555555555501', N'KS_THI_HANH_PHAP_LUAT', N'TAO_CUOC_KHAO_SAT', N'NHAP', N'Nhập', N'#6B7280', 1, 1, 0, N'Cuộc khảo sát đang được tạo/cấu hình.'),
('55555555-5555-5555-5555-555555555502', N'KS_THI_HANH_PHAP_LUAT', N'THUC_HIEN_KHAO_SAT', N'DA_PHAT_HANH', N'Đã phát hành', N'#2563EB', 2, 0, 0, N'Cuộc khảo sát đã phát hành đến đối tượng khảo sát.'),
('55555555-5555-5555-5555-555555555503', N'KS_THI_HANH_PHAP_LUAT', N'THUC_HIEN_KHAO_SAT', N'DANG_TRA_LOI', N'Đang trả lời', N'#0EA5E9', 3, 0, 0, N'Đối tượng khảo sát đang cập nhật câu trả lời.'),
('55555555-5555-5555-5555-555555555504', N'KS_THI_HANH_PHAP_LUAT', N'DANH_GIA_KET_QUA', N'YEU_CAU_BO_SUNG', N'Yêu cầu bổ sung', N'#EF4444', 4, 0, 0, N'Dữ liệu khảo sát chưa đầy đủ/hợp lệ, cần bổ sung.'),
('55555555-5555-5555-5555-555555555505', N'KS_THI_HANH_PHAP_LUAT', N'DANH_GIA_KET_QUA', N'DA_DANH_GIA', N'Đã đánh giá', N'#7C3AED', 5, 0, 0, N'Dữ liệu khảo sát đã được rà soát, đánh giá.'),
('55555555-5555-5555-5555-555555555506', N'KS_THI_HANH_PHAP_LUAT', N'TONG_HOP_BAO_CAO', N'HOAN_THANH', N'Hoàn thành', N'#16A34A', 6, 0, 1, N'Hoàn thành báo cáo kết quả khảo sát.');

MERGE [dm].[DanhMucTrangThaiNghiepVuQuyTrinhs] AS target
USING (
    SELECT t.*, q.Id AS QuyTrinhId, b.Id AS BuocId
    FROM @TrangThai t
    JOIN @QuyTrinhIds q ON q.MaQuyTrinh = t.MaQuyTrinh
    LEFT JOIN @BuocIds b ON b.MaQuyTrinh = t.MaQuyTrinh AND b.MaBuoc = t.MaBuoc
) AS source
ON target.[QuyTrinhSoanThaoId] = source.[QuyTrinhId] AND target.[MaTrangThai] = source.[MaTrangThai]
WHEN MATCHED THEN UPDATE SET
    [BuocQuyTrinhId] = source.[BuocId],
    [TenTrangThai] = source.[TenTrangThai],
    [MaMauHex] = source.[MaMauHex],
    [ThuTuSapXep] = source.[ThuTuSapXep],
    [LaTrangThaiKhoiTao] = source.[LaTrangThaiKhoiTao],
    [LaTrangThaiKetThuc] = source.[LaTrangThaiKetThuc],
    [TrangThai] = 1,
    [MoTa] = source.[MoTa],
    [UpdatedBy] = @SystemUserId,
    [UpdatedDate] = @Now
WHEN NOT MATCHED THEN INSERT
    ([Id], [QuyTrinhSoanThaoId], [BuocQuyTrinhId], [MaTrangThai], [TenTrangThai], [MaMauHex], [ThuTuSapXep], [LaTrangThaiKhoiTao], [LaTrangThaiKetThuc], [TrangThai], [MoTa], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
VALUES
    (source.[Id], source.[QuyTrinhId], source.[BuocId], source.[MaTrangThai], source.[TenTrangThai], source.[MaMauHex], source.[ThuTuSapXep], source.[LaTrangThaiKhoiTao], source.[LaTrangThaiKetThuc], 1, source.[MoTa], @SystemUserId, @Now, @SystemUserId, @Now);

DECLARE @ChuyenBuoc TABLE
(
    Id uniqueidentifier NOT NULL,
    MaQuyTrinh nvarchar(250) NOT NULL,
    TuBuoc nvarchar(250) NOT NULL,
    DenBuoc nvarchar(250) NOT NULL,
    DieuKienKetQua nvarchar(250) NOT NULL,
    LoaiChuyenBuoc nvarchar(100) NOT NULL,
    LaNhanhMacDinh bit NOT NULL,
    YeuCauNhapLyDo bit NOT NULL,
    IsKetThuc bit NOT NULL,
    MoTa nvarchar(max) NULL
);

INSERT INTO @ChuyenBuoc VALUES
-- Đăng ký xây dựng QPPL
('33333333-3333-3333-3333-333333333321', N'DXDM_DKXD_QPPL', N'LAP_HO_SO', N'TRINH_HO_SO', N'TRINH_PHE_DUYET', N'Forward', 1, 0, 0, N'Trình hồ sơ đăng ký đến cơ quan có thẩm quyền.'),
('33333333-3333-3333-3333-333333333322', N'DXDM_DKXD_QPPL', N'TRINH_HO_SO', N'PHE_DUYET', N'TIEP_NHAN_PHE_DUYET', N'Forward', 1, 0, 0, N'Tiếp nhận hồ sơ để xem xét, phê duyệt.'),
('33333333-3333-3333-3333-333333333323', N'DXDM_DKXD_QPPL', N'PHE_DUYET', N'CAP_NHAT_KET_QUA', N'PHE_DUYET', N'Approve', 1, 0, 0, N'Hồ sơ được phê duyệt, chuyển cập nhật kết quả.'),
('33333333-3333-3333-3333-333333333324', N'DXDM_DKXD_QPPL', N'PHE_DUYET', N'LAP_HO_SO', N'TRA_LAI', N'Return', 0, 1, 0, N'Trả lại hồ sơ để bổ sung.'),
('33333333-3333-3333-3333-333333333325', N'DXDM_DKXD_QPPL', N'PHE_DUYET', N'HOAN_THANH', N'KHONG_PHE_DUYET', N'Reject', 0, 1, 1, N'Không phê duyệt hồ sơ đăng ký.'),
('33333333-3333-3333-3333-333333333326', N'DXDM_DKXD_QPPL', N'CAP_NHAT_KET_QUA', N'HOAN_THANH', N'HOAN_THANH', N'Forward', 1, 0, 1, N'Hoàn thành quy trình đăng ký.'),

-- Xây dựng Quyết định UBND
('44444444-4444-4444-4444-444444444131', N'XD_QD_UBND_TINH', N'SOAN_THAO_LAY_Y_KIEN', N'GUI_THAM_DINH', N'GUI_THAM_DINH', N'Forward', 1, 0, 0, N'Gửi hồ sơ dự thảo Quyết định đến Sở Tư pháp thẩm định.'),
('44444444-4444-4444-4444-444444444132', N'XD_QD_UBND_TINH', N'GUI_THAM_DINH', N'THAM_DINH', N'TIEP_NHAN_THAM_DINH', N'Forward', 1, 0, 0, N'Sở Tư pháp tiếp nhận hồ sơ thẩm định.'),
('44444444-4444-4444-4444-444444444133', N'XD_QD_UBND_TINH', N'THAM_DINH', N'SOAN_THAO_LAY_Y_KIEN', N'YEU_CAU_BO_SUNG', N'Return', 0, 1, 0, N'Yêu cầu cơ quan chủ trì bổ sung hồ sơ.'),
('44444444-4444-4444-4444-444444444134', N'XD_QD_UBND_TINH', N'THAM_DINH', N'TRINH_UBND', N'DA_THAM_DINH', N'Forward', 1, 0, 0, N'Hồ sơ đã có báo cáo thẩm định, chuyển trình UBND.'),
('44444444-4444-4444-4444-444444444135', N'XD_QD_UBND_TINH', N'TRINH_UBND', N'LAY_Y_KIEN_UBND', N'LAY_Y_KIEN_UBND', N'Forward', 1, 0, 0, N'Cập nhật hồ sơ lấy ý kiến thành viên UBND tỉnh.'),
('44444444-4444-4444-4444-444444444136', N'XD_QD_UBND_TINH', N'LAY_Y_KIEN_UBND', N'SOAN_THAO_LAY_Y_KIEN', N'CO_Y_KIEN_KHAC_NHAU', N'Return', 0, 1, 0, N'Có ý kiến khác nhau, cần giải trình/bổ sung.'),
('44444444-4444-4444-4444-444444444137', N'XD_QD_UBND_TINH', N'LAY_Y_KIEN_UBND', N'BAN_HANH', N'DONG_Y_BAN_HANH', N'Approve', 1, 0, 1, N'Không có ý kiến khác nhau, cập nhật Quyết định ban hành.'),

-- Xây dựng Nghị quyết HĐND
('44444444-4444-4444-4444-444444444231', N'XD_NQ_HDND_TINH', N'SOAN_THAO_LAY_Y_KIEN', N'GUI_THAM_DINH', N'GUI_THAM_DINH', N'Forward', 1, 0, 0, N'Gửi hồ sơ dự thảo Nghị quyết đến Sở Tư pháp thẩm định.'),
('44444444-4444-4444-4444-444444444232', N'XD_NQ_HDND_TINH', N'GUI_THAM_DINH', N'THAM_DINH', N'TIEP_NHAN_THAM_DINH', N'Forward', 1, 0, 0, N'Sở Tư pháp tiếp nhận hồ sơ thẩm định.'),
('44444444-4444-4444-4444-444444444233', N'XD_NQ_HDND_TINH', N'THAM_DINH', N'SOAN_THAO_LAY_Y_KIEN', N'YEU_CAU_BO_SUNG', N'Return', 0, 1, 0, N'Yêu cầu cơ quan chủ trì bổ sung hồ sơ.'),
('44444444-4444-4444-4444-444444444234', N'XD_NQ_HDND_TINH', N'THAM_DINH', N'TRINH_UBND_CHO_Y_KIEN', N'DA_THAM_DINH', N'Forward', 1, 0, 0, N'Hồ sơ đã có báo cáo thẩm định, chuyển trình UBND cho ý kiến.'),
('44444444-4444-4444-4444-444444444235', N'XD_NQ_HDND_TINH', N'TRINH_UBND_CHO_Y_KIEN', N'LAY_Y_KIEN_UBND', N'LAY_Y_KIEN_UBND', N'Forward', 1, 0, 0, N'Cập nhật hồ sơ lấy ý kiến thành viên UBND tỉnh.'),
('44444444-4444-4444-4444-444444444236', N'XD_NQ_HDND_TINH', N'LAY_Y_KIEN_UBND', N'SOAN_THAO_LAY_Y_KIEN', N'CO_Y_KIEN_KHAC_NHAU', N'Return', 0, 1, 0, N'Có ý kiến khác nhau, cần giải trình/bổ sung.'),
('44444444-4444-4444-4444-444444444237', N'XD_NQ_HDND_TINH', N'LAY_Y_KIEN_UBND', N'TRINH_HDND_THAM_TRA', N'DONG_Y_TRINH_HDND', N'Forward', 1, 0, 0, N'Hoàn thiện hồ sơ trình HĐND tỉnh thẩm tra.'),
('44444444-4444-4444-4444-444444444238', N'XD_NQ_HDND_TINH', N'TRINH_HDND_THAM_TRA', N'SOAN_THAO_LAY_Y_KIEN', N'YEU_CAU_HOAN_THIEN', N'Return', 0, 1, 0, N'HĐND yêu cầu hoàn thiện hồ sơ sau thẩm tra/thảo luận.'),
('44444444-4444-4444-4444-444444444239', N'XD_NQ_HDND_TINH', N'TRINH_HDND_THAM_TRA', N'THONG_QUA_BAN_HANH', N'THONG_QUA_BAN_HANH', N'Approve', 1, 0, 1, N'HĐND thông qua, cập nhật Nghị quyết ban hành.'),

-- Tổ chức thi hành pháp luật
('44444444-4444-4444-4444-444444444331', N'TC_THI_HANH_PHAP_LUAT', N'TAO_KE_HOACH', N'THUC_HIEN_NOI_DUNG', N'PHAN_CONG_THUC_HIEN', N'Forward', 1, 0, 0, N'Phân công nội dung thi hành pháp luật cho các đơn vị.'),
('44444444-4444-4444-4444-444444444332', N'TC_THI_HANH_PHAP_LUAT', N'THUC_HIEN_NOI_DUNG', N'DANH_GIA_KET_QUA', N'GUI_DANH_GIA', N'Forward', 1, 0, 0, N'Đơn vị gửi kết quả thực hiện để Sở Tư pháp đánh giá.'),
('44444444-4444-4444-4444-444444444333', N'TC_THI_HANH_PHAP_LUAT', N'DANH_GIA_KET_QUA', N'THUC_HIEN_NOI_DUNG', N'YEU_CAU_BO_SUNG', N'Return', 0, 1, 0, N'Kết quả chưa đạt, yêu cầu đơn vị bổ sung/cập nhật.'),
('44444444-4444-4444-4444-444444444334', N'TC_THI_HANH_PHAP_LUAT', N'DANH_GIA_KET_QUA', N'TONG_HOP_KET_QUA', N'DAT', N'Approve', 1, 0, 1, N'Kết quả đạt yêu cầu, chuyển tổng hợp kết quả kế hoạch.'),

-- Khảo sát thi hành pháp luật
('44444444-4444-4444-4444-444444444431', N'KS_THI_HANH_PHAP_LUAT', N'TAO_CUOC_KHAO_SAT', N'THUC_HIEN_KHAO_SAT', N'PHAT_HANH_KHAO_SAT', N'Forward', 1, 0, 0, N'Phát hành cuộc khảo sát đến đối tượng khảo sát.'),
('44444444-4444-4444-4444-444444444432', N'KS_THI_HANH_PHAP_LUAT', N'THUC_HIEN_KHAO_SAT', N'DANH_GIA_KET_QUA', N'GUI_PHIEU_KHAO_SAT', N'Forward', 1, 0, 0, N'Đối tượng khảo sát gửi phiếu trả lời.'),
('44444444-4444-4444-4444-444444444433', N'KS_THI_HANH_PHAP_LUAT', N'DANH_GIA_KET_QUA', N'THUC_HIEN_KHAO_SAT', N'YEU_CAU_BO_SUNG', N'Return', 0, 1, 0, N'Dữ liệu chưa đầy đủ/hợp lệ, yêu cầu bổ sung.'),
('44444444-4444-4444-4444-444444444434', N'KS_THI_HANH_PHAP_LUAT', N'DANH_GIA_KET_QUA', N'TONG_HOP_BAO_CAO', N'DA_DANH_GIA', N'Approve', 1, 0, 1, N'Dữ liệu đã được đánh giá, chuyển tổng hợp báo cáo.');

MERGE [dm].[DanhMucChuyenBuocQuyTrinhs] AS target
USING (
    SELECT c.*, q.Id AS QuyTrinhId, tu.Id AS TuBuocId, den.Id AS DenBuocId
    FROM @ChuyenBuoc c
    JOIN @QuyTrinhIds q ON q.MaQuyTrinh = c.MaQuyTrinh
    JOIN @BuocIds tu ON tu.MaQuyTrinh = c.MaQuyTrinh AND tu.MaBuoc = c.TuBuoc
    JOIN @BuocIds den ON den.MaQuyTrinh = c.MaQuyTrinh AND den.MaBuoc = c.DenBuoc
) AS source
ON target.[Id] = source.[Id]
WHEN MATCHED THEN UPDATE SET
    [QuyTrinhSoanThaoId] = source.[QuyTrinhId],
    [TuBuocId] = source.[TuBuocId],
    [DenBuocId] = source.[DenBuocId],
    [DieuKienKetQua] = source.[DieuKienKetQua],
    [LoaiChuyenBuoc] = source.[LoaiChuyenBuoc],
    [LaNhanhMacDinh] = source.[LaNhanhMacDinh],
    [YeuCauNhapLyDo] = source.[YeuCauNhapLyDo],
    [IsKetThuc] = source.[IsKetThuc],
    [MoTa] = source.[MoTa],
    [UpdatedBy] = @SystemUserId,
    [UpdatedDate] = @Now
WHEN NOT MATCHED THEN INSERT
    ([Id], [QuyTrinhSoanThaoId], [TuBuocId], [DenBuocId], [DieuKienKetQua], [LoaiChuyenBuoc], [LaNhanhMacDinh], [YeuCauNhapLyDo], [IsKetThuc], [MoTa], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
VALUES
    (source.[Id], source.[QuyTrinhId], source.[TuBuocId], source.[DenBuocId], source.[DieuKienKetQua], source.[LoaiChuyenBuoc], source.[LaNhanhMacDinh], source.[YeuCauNhapLyDo], source.[IsKetThuc], source.[MoTa], @SystemUserId, @Now, @SystemUserId, @Now);

SELECT q.[MaQuyTrinh], q.[TenQuyTrinh],
       COUNT(DISTINCT b.[Id]) AS [SoBuoc],
       COUNT(DISTINCT tt.[Id]) AS [SoTrangThaiNghiepVu],
       COUNT(DISTINCT cb.[Id]) AS [SoChuyenBuoc]
FROM @QuyTrinhIds qi
JOIN [dm].[DanhMucQuyTrinhSoanThaos] q ON q.[Id] = qi.[Id]
LEFT JOIN [dm].[DanhMucBuocQuyTrinhs] b ON b.[QuyTrinhSoanThaoId] = q.[Id]
LEFT JOIN [dm].[DanhMucTrangThaiNghiepVuQuyTrinhs] tt ON tt.[QuyTrinhSoanThaoId] = q.[Id]
LEFT JOIN [dm].[DanhMucChuyenBuocQuyTrinhs] cb ON cb.[QuyTrinhSoanThaoId] = q.[Id]
GROUP BY q.[MaQuyTrinh], q.[TenQuyTrinh]
ORDER BY q.[MaQuyTrinh];
