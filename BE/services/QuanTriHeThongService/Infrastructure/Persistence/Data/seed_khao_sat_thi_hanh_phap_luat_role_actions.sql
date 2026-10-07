/*
    Seed RoleAction cho KhaoSatThiHanhPhapLuatService.
    Bám theo tài liệu QT khao sat.docx và treo dưới group ThiHanhPhapLuat hiện hữu trong dữ liệu import.
    Script idempotent: có thể chạy lại để bổ sung/cập nhật cấu hình.
*/
SET NOCOUNT ON;

DECLARE @Now datetime2 = SYSUTCDATETIME();
DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @ParentId uniqueidentifier;
DECLARE @GroupId uniqueidentifier = '55555555-5555-5555-5555-555555555500';

SELECT @ParentId = [Id]
FROM [qtht].[RoleActions]
WHERE [Id] = '20000000-0000-0000-0000-000000000031'
   OR [Role] = N'ThiHanhPhapLuat';

IF @ParentId IS NULL
    THROW 51000, N'Không tìm thấy RoleAction cha ThiHanhPhapLuat.', 1;

DECLARE @Rows TABLE
(
    [Id] uniqueidentifier NOT NULL,
    [STTSapXep] int NOT NULL,
    [PhanLoai] nvarchar(50) NOT NULL,
    [Level] int NOT NULL,
    [Role] nvarchar(250) NOT NULL,
    [Title] nvarchar(500) NOT NULL,
    [Controller] nvarchar(250) NULL,
    [Table] nvarchar(250) NULL,
    [FrontendPath] nvarchar(500) NULL,
    [IsVisibleInMenu] bit NOT NULL,
    [MenuTitle] nvarchar(500) NOT NULL,
    [MenuIcon] nvarchar(100) NOT NULL
);

INSERT INTO @Rows VALUES
(@GroupId, 400, N'Group', 1, N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat', N'Khảo sát tình hình thi hành pháp luật', NULL, NULL, N'/khao-sat-thi-hanh-phap-luat', 1, N'Khảo sát THPL', N'clipboard-list'),
('55555555-5555-5555-5555-555555555501', 401, N'Detail', 2, N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat.CuocKhaoSat', N'Quản lý cuộc khảo sát', N'KhaoSatThiHanhPhapLuatCuocKhaoSat', N'CuocKhaoSats', N'/khao-sat-thi-hanh-phap-luat/cuoc-khao-sat', 1, N'Cuộc khảo sát', N'clipboard'),
('55555555-5555-5555-5555-555555555502', 402, N'Detail', 2, N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat.MauPhieu', N'Mẫu phiếu khảo sát theo cuộc khảo sát và nhóm đối tượng', N'KhaoSatThiHanhPhapLuatMauPhieu', N'MauPhieuKhaoSats', N'/khao-sat-thi-hanh-phap-luat/mau-phieu', 0, N'Mẫu phiếu', N'file-text'),
('55555555-5555-5555-5555-555555555503', 403, N'Detail', 2, N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat.PhatHanh', N'Thiết lập đối tượng khảo sát', N'KhaoSatThiHanhPhapLuatPhatHanh', N'DoiTuongKhaoSats', N'/khao-sat-thi-hanh-phap-luat/phat-hanh', 0, N'Đối tượng khảo sát', N'send'),
('55555555-5555-5555-5555-555555555504', 404, N'Detail', 2, N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat.NopPhieu', N'Cập nhật kết quả khảo sát đã thực hiện bên ngoài', N'KhaoSatThiHanhPhapLuatNopPhieu', N'PhieuNopKhaoSats', N'/khao-sat-thi-hanh-phap-luat/nhap-ket-qua', 1, N'Cập nhật kết quả', N'upload'),
('55555555-5555-5555-5555-555555555505', 405, N'Detail', 2, N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat.RaSoat', N'Đánh giá kết quả khảo sát', N'KhaoSatThiHanhPhapLuatRaSoat', N'PhieuNopKhaoSats', N'/khao-sat-thi-hanh-phap-luat/ra-soat', 1, N'Rà soát', N'search-check'),
('55555555-5555-5555-5555-555555555506', 406, N'Detail', 2, N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat.BaoCao', N'Tổng hợp và báo cáo kết quả khảo sát', N'KhaoSatThiHanhPhapLuatBaoCao', N'BaoCaoKhaoSats', N'/khao-sat-thi-hanh-phap-luat/bao-cao', 1, N'Báo cáo', N'chart-bar'),
('55555555-5555-5555-5555-555555555507', 407, N'Detail', 2, N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat.Dashboard', N'Dashboard khảo sát', N'KhaoSatThiHanhPhapLuatDashboard', N'CuocKhaoSats', N'/khao-sat-thi-hanh-phap-luat/dashboard', 1, N'Dashboard', N'layout-dashboard'),
('55555555-5555-5555-5555-555555555508', 408, N'Detail', 2, N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat.DocMauPhieu', N'Đọc mẫu phiếu khảo sát', N'KhaoSatThiHanhPhapLuatDocMauPhieu', N'PhienDocMauPhieuKhaoSats', N'/khao-sat-thi-hanh-phap-luat/doc-mau-phieu', 0, N'Đọc mẫu phiếu', N'file-input'),
('55555555-5555-5555-5555-555555555509', 409, N'Detail', 2, N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat.BaoCaoSnapshot', N'Chốt và xuất báo cáo khảo sát', N'KhaoSatThiHanhPhapLuatBaoCaoSnapshot', N'BaoCaoKhaoSats', N'/khao-sat-thi-hanh-phap-luat/bao-cao', 0, N'Chốt báo cáo', N'file-check');

INSERT INTO [qtht].[RoleActions]
(
    [Id], [STTSapXep], [PhanLoai], [Level], [Role], [RoleGroupId], [Title],
    [Controller], [Action], [Parameter], [Table], [Status], [UseGroup],
    [FrontendPath], [IsVisibleInMenu], [ClientApp], [MenuTitle], [MenuIcon], [Icon],
    [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]
)
SELECT r.[Id], r.[STTSapXep], r.[PhanLoai], r.[Level], r.[Role],
       CASE WHEN r.[Role] = N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat' THEN @ParentId ELSE @GroupId END,
       r.[Title], r.[Controller], CASE WHEN r.[PhanLoai] = N'Detail' THEN N'Index' END,
       NULL, r.[Table], N'Kích hoạt', NULL,
       r.[FrontendPath], r.[IsVisibleInMenu], N'frontend', r.[MenuTitle], r.[MenuIcon], r.[MenuIcon],
       @SystemUserId, @Now, @SystemUserId, @Now
FROM @Rows r
WHERE NOT EXISTS (SELECT 1 FROM [qtht].[RoleActions] e WHERE e.[Role] = r.[Role]);

UPDATE e
SET e.[STTSapXep] = r.[STTSapXep],
    e.[PhanLoai] = r.[PhanLoai],
    e.[Level] = r.[Level],
    e.[RoleGroupId] = CASE WHEN r.[Role] = N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat' THEN @ParentId ELSE @GroupId END,
    e.[Title] = r.[Title],
    e.[Controller] = r.[Controller],
    e.[Action] = CASE WHEN r.[PhanLoai] = N'Detail' THEN N'Index' END,
    e.[Table] = r.[Table],
    e.[Status] = N'Kích hoạt',
    e.[FrontendPath] = r.[FrontendPath],
    e.[IsVisibleInMenu] = r.[IsVisibleInMenu],
    e.[ClientApp] = N'frontend',
    e.[MenuTitle] = r.[MenuTitle],
    e.[MenuIcon] = r.[MenuIcon],
    e.[Icon] = r.[MenuIcon],
    e.[UpdatedBy] = @SystemUserId,
    e.[UpdatedDate] = @Now
FROM [qtht].[RoleActions] e
JOIN @Rows r ON r.[Role] = e.[Role];

SELECT [Role], [PhanLoai], [Controller], [Action], [FrontendPath], [Table], [Status], [IsVisibleInMenu]
FROM [qtht].[RoleActions]
WHERE [Role] = N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat'
   OR [Role] LIKE N'ThiHanhPhapLuat.KhaoSatThiHanhPhapLuat.%'
ORDER BY [STTSapXep];
