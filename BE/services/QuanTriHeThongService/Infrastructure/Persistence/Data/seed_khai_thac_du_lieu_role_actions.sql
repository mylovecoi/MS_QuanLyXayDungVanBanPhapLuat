/*
    Seed RoleAction cho KhaiThacDuLieuService.
    Quy tac PM: moi RoleAction Detail co mot Form va mot Controller rieng.
    Luu y: neu dau muc VanBanQPPL.TraCuu da ton tai thi dung lai, khong tao nhanh tra cuu trung lap.
*/
SET NOCOUNT ON;

DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @Now datetime2 = SYSUTCDATETIME();
DECLARE @VanBanQpplGroupId uniqueidentifier;
DECLARE @DashboardGroupId uniqueidentifier = '94000000-0000-0000-0000-000000000001';
DECLARE @CanhBaoGroupId uniqueidentifier = '94000000-0000-0000-0000-000000000002';
DECLARE @TraCuuGroupId uniqueidentifier;
DECLARE @BaoCaoGroupId uniqueidentifier = '94000000-0000-0000-0000-000000000004';

SELECT @VanBanQpplGroupId = [Id]
FROM [qtht].[RoleActions]
WHERE [Role] = N'VanBanQPPL';

IF @VanBanQpplGroupId IS NULL
    THROW 51000, N'Khong tim thay RoleAction cha VanBanQPPL. Hay seed nhom VanBanQPPL truoc.', 1;

SELECT @TraCuuGroupId = [Id]
FROM [qtht].[RoleActions]
WHERE [Role] = N'VanBanQPPL.TraCuu';

IF @TraCuuGroupId IS NULL
    SET @TraCuuGroupId = '94000000-0000-0000-0000-000000000003';

DECLARE @RoleActions TABLE
(
    [Id] uniqueidentifier NOT NULL,
    [STTSapXep] int NOT NULL,
    [PhanLoai] nvarchar(50) NOT NULL,
    [Level] int NOT NULL,
    [Role] nvarchar(250) NOT NULL,
    [RoleGroupId] uniqueidentifier NULL,
    [Title] nvarchar(500) NOT NULL,
    [Controller] nvarchar(250) NULL,
    [Action] nvarchar(100) NULL,
    [Table] nvarchar(250) NULL,
    [FrontendPath] nvarchar(500) NULL,
    [MenuTitle] nvarchar(500) NULL,
    [MenuIcon] nvarchar(100) NULL
);

INSERT INTO @RoleActions
VALUES
(@DashboardGroupId, 400, N'Group', 0, N'VanBanQPPL.Dashboard', @VanBanQpplGroupId,
 N'Dashboard điều hành', NULL, NULL, NULL, N'/dashboard', N'Dashboard', N'layout-dashboard'),
(@CanhBaoGroupId, 410, N'Group', 0, N'VanBanQPPL.CanhBao', @VanBanQpplGroupId,
 N'Cảnh báo thông minh', NULL, NULL, NULL, N'/canh-bao', N'Cảnh báo', N'bell-ring'),
(@TraCuuGroupId, 420, N'Group', 0, N'VanBanQPPL.TraCuu', @VanBanQpplGroupId,
 N'Tra cứu', NULL, NULL, NULL, N'/tra-cuu', N'Tra cứu', N'search'),
(@BaoCaoGroupId, 430, N'Group', 0, N'VanBanQPPL.BaoCaoThongKe', @VanBanQpplGroupId,
 N'Báo cáo thống kê', NULL, NULL, NULL, N'/bao-cao-thong-ke', N'Báo cáo thống kê', N'file-chart-column'),

(N'94000000-0000-0000-0000-000000000101', 401, N'Detail', 1, N'VanBanQPPL.Dashboard.TongQuan', @DashboardGroupId,
 N'Dashboard tổng quan điều hành', N'DashboardTongQuan', N'Index', N'CanhBaoKhaiThacDuLieus', N'/dashboard/tong-quan', N'Tổng quan', N'layout-dashboard'),

(N'94000000-0000-0000-0000-000000000201', 411, N'Detail', 1, N'VanBanQPPL.CanhBao.ThongMinh', @CanhBaoGroupId,
 N'Cảnh báo thông minh', N'CanhBaoThongMinh', N'Index', N'CanhBaoKhaiThacDuLieus', N'/canh-bao/thong-minh', N'Cảnh báo thông minh', N'bell-ring'),

(N'94000000-0000-0000-0000-000000000301', 421, N'Detail', 1, N'VanBanQPPL.TraCuu.TongHop', @TraCuuGroupId,
 N'Tra cứu tổng hợp', N'TraCuuTongHop', N'Index', NULL, N'/tra-cuu/tong-hop', N'Tổng hợp', N'search'),
(N'94000000-0000-0000-0000-000000000302', 422, N'Detail', 1, N'VanBanQPPL.TraCuu.DangKyXayDungVanBan', @TraCuuGroupId,
 N'Tra cứu đăng ký xây dựng văn bản', N'TraCuuDangKyXayDungVanBan', N'Index', NULL, N'/tra-cuu/dang-ky-xay-dung-van-ban', N'Đăng ký xây dựng văn bản', N'search'),
(N'94000000-0000-0000-0000-000000000303', 423, N'Detail', 1, N'VanBanQPPL.TraCuu.XayDungVanBan', @TraCuuGroupId,
 N'Tra cứu xây dựng văn bản', N'TraCuuXayDungVanBan', N'Index', NULL, N'/tra-cuu/xay-dung-van-ban', N'Xây dựng văn bản', N'search'),
(N'94000000-0000-0000-0000-000000000304', 424, N'Detail', 1, N'VanBanQPPL.TraCuu.ThiHanhPhapLuat', @TraCuuGroupId,
 N'Tra cứu thi hành pháp luật', N'TraCuuThiHanhPhapLuat', N'Index', NULL, N'/tra-cuu/thi-hanh-phap-luat', N'Thi hành pháp luật', N'search'),

(N'94000000-0000-0000-0000-000000000401', 431, N'Detail', 1, N'VanBanQPPL.BaoCaoThongKe.TongHop', @BaoCaoGroupId,
 N'Báo cáo thống kê tổng hợp', N'BaoCaoThongKe', N'Index', NULL, N'/bao-cao-thong-ke/tong-hop', N'Tổng hợp', N'file-chart-column');

INSERT INTO [qtht].[RoleActions]
(
    [Id], [STTSapXep], [PhanLoai], [Level], [Role], [RoleGroupId], [Title],
    [Controller], [Action], [Parameter], [Table], [Status], [UseGroup],
    [FrontendPath], [IsVisibleInMenu], [ClientApp], [MenuTitle], [MenuIcon], [Icon],
    [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]
)
SELECT r.[Id], r.[STTSapXep], r.[PhanLoai], r.[Level], r.[Role], r.[RoleGroupId],
       r.[Title], r.[Controller], r.[Action], NULL, r.[Table], N'Kích hoạt', NULL,
       r.[FrontendPath], 1, N'frontend', r.[MenuTitle], r.[MenuIcon], NULL,
       @SystemUserId, @Now, @SystemUserId, @Now
FROM @RoleActions r
WHERE NOT EXISTS (SELECT 1 FROM [qtht].[RoleActions] e WHERE e.[Role] = r.[Role]);

UPDATE e
SET e.[STTSapXep] = r.[STTSapXep],
    e.[PhanLoai] = r.[PhanLoai],
    e.[Level] = r.[Level],
    e.[RoleGroupId] = r.[RoleGroupId],
    e.[Title] = r.[Title],
    e.[Controller] = r.[Controller],
    e.[Action] = r.[Action],
    e.[Table] = r.[Table],
    e.[Status] = N'Kích hoạt',
    e.[FrontendPath] = r.[FrontendPath],
    e.[IsVisibleInMenu] = 1,
    e.[ClientApp] = N'frontend',
    e.[MenuTitle] = r.[MenuTitle],
    e.[MenuIcon] = r.[MenuIcon],
    e.[UpdatedBy] = @SystemUserId,
    e.[UpdatedDate] = @Now
FROM [qtht].[RoleActions] e
JOIN @RoleActions r ON r.[Role] = e.[Role];

SELECT [Role], [PhanLoai], [Controller], [Action], [FrontendPath], [Table], [Status]
FROM [qtht].[RoleActions]
WHERE [Role] IN (N'VanBanQPPL.Dashboard', N'VanBanQPPL.CanhBao', N'VanBanQPPL.TraCuu', N'VanBanQPPL.BaoCaoThongKe')
   OR [Role] LIKE N'VanBanQPPL.Dashboard.%'
   OR [Role] LIKE N'VanBanQPPL.CanhBao.%'
   OR [Role] LIKE N'VanBanQPPL.TraCuu.%'
   OR [Role] LIKE N'VanBanQPPL.BaoCaoThongKe.%'
ORDER BY [STTSapXep];
