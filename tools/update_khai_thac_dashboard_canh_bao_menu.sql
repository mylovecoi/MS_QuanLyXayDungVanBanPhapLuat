SET NOCOUNT ON;

DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @Now datetime2 = SYSUTCDATETIME();
DECLARE @VanBanQpplGroupId uniqueidentifier;
DECLARE @DashboardGroupId uniqueidentifier;
DECLARE @CanhBaoGroupId uniqueidentifier;

SELECT @VanBanQpplGroupId = [Id] FROM [qtht].[RoleActions] WHERE [Role] = N'VanBanQPPL';
IF @VanBanQpplGroupId IS NULL
    THROW 51000, N'Khong tim thay RoleAction cha VanBanQPPL.', 1;

SELECT @DashboardGroupId = [Id] FROM [qtht].[RoleActions] WHERE [Role] = N'VanBanQPPL.Dashboard';
IF @DashboardGroupId IS NULL
    SET @DashboardGroupId = '94000000-0000-0000-0000-000000000001';

SELECT @CanhBaoGroupId = [Id] FROM [qtht].[RoleActions] WHERE [Role] = N'VanBanQPPL.CanhBao';
IF @CanhBaoGroupId IS NULL
    SET @CanhBaoGroupId = '94000000-0000-0000-0000-000000000002';

DECLARE @Rows TABLE
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

INSERT INTO @Rows
VALUES
(@DashboardGroupId, 400, N'Group', 0, N'VanBanQPPL.Dashboard', @VanBanQpplGroupId,
 N'Dashboard điều hành', NULL, NULL, NULL, N'/dashboard/tong-quan', N'Dashboard', N'layout-dashboard'),
('94000000-0000-0000-0000-000000000101', 401, N'Detail', 1, N'VanBanQPPL.Dashboard.TongQuan', @DashboardGroupId,
 N'Dashboard tổng quan điều hành', N'DashboardTongQuan', N'Index', N'CanhBaoKhaiThacDuLieus', N'/dashboard/tong-quan', N'Tổng quan', N'layout-dashboard'),
('94000000-0000-0000-0000-000000000102', 402, N'Detail', 1, N'VanBanQPPL.Dashboard.CauHinh', @DashboardGroupId,
 N'Cấu hình Dashboard', N'DashboardTongQuan', N'Edit', N'DashboardWidgetCauHinhs', N'/dashboard/cau-hinh', N'Cấu hình Dashboard', N'sliders-horizontal'),
(@CanhBaoGroupId, 410, N'Group', 0, N'VanBanQPPL.CanhBao', @VanBanQpplGroupId,
 N'Cảnh báo thông minh', NULL, NULL, NULL, N'/canh-bao/thong-minh', N'Cảnh báo', N'bell-ring'),
('94000000-0000-0000-0000-000000000201', 411, N'Detail', 1, N'VanBanQPPL.CanhBao.ThongMinh', @CanhBaoGroupId,
 N'Cảnh báo thông minh', N'CanhBaoThongMinh', N'Index', N'CanhBaoKhaiThacDuLieus', N'/canh-bao/thong-minh', N'Cảnh báo thông minh', N'bell-ring');

INSERT INTO [qtht].[RoleActions]
(
    [Id], [STTSapXep], [PhanLoai], [Level], [Role], [RoleGroupId], [Title],
    [Controller], [Action], [Parameter], [Table], [Status], [UseGroup],
    [FrontendPath], [IsVisibleInMenu], [ClientApp], [MenuTitle], [MenuIcon], [Icon],
    [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]
)
SELECT r.[Id], r.[STTSapXep], r.[PhanLoai], r.[Level], r.[Role], r.[RoleGroupId],
       r.[Title], r.[Controller], r.[Action], NULL, r.[Table], N'Kích hoạt', NULL,
       r.[FrontendPath], 1, N'frontend', r.[MenuTitle], r.[MenuIcon], r.[MenuIcon],
       @SystemUserId, @Now, @SystemUserId, @Now
FROM @Rows r
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
    e.[Icon] = r.[MenuIcon],
    e.[UpdatedBy] = @SystemUserId,
    e.[UpdatedDate] = @Now
FROM [qtht].[RoleActions] e
JOIN @Rows r ON r.[Role] = e.[Role];

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20261011134000_SeedKhaiThacDuLieuDashboardCanhBaoRoleActions')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261011134000_SeedKhaiThacDuLieuDashboardCanhBaoRoleActions', N'10.0.12');
END;

SELECT [Role], [MenuTitle], [FrontendPath], [IsVisibleInMenu], [ClientApp], [Status]
FROM [qtht].[RoleActions]
WHERE [Role] LIKE N'VanBanQPPL.Dashboard%'
   OR [Role] LIKE N'VanBanQPPL.CanhBao%'
ORDER BY [STTSapXep];
