SET NOCOUNT ON;

IF OBJECT_ID(N'[DashboardWidgets]', N'U') IS NULL
BEGIN
    CREATE TABLE [DashboardWidgets] (
        [Id] uniqueidentifier NOT NULL,
        [MaWidget] nvarchar(100) NOT NULL,
        [TenWidget] nvarchar(250) NOT NULL,
        [NhomWidget] nvarchar(100) NOT NULL,
        [LoaiWidget] nvarchar(50) NOT NULL,
        [MoTa] nvarchar(1000) NULL,
        [TrangThai] bit NOT NULL,
        [CauHinhMacDinhJson] nvarchar(4000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DashboardWidgets] PRIMARY KEY ([Id])
    );
END;

IF OBJECT_ID(N'[DashboardWidgetCauHinhs]', N'U') IS NULL
BEGIN
    CREATE TABLE [DashboardWidgetCauHinhs] (
        [Id] uniqueidentifier NOT NULL,
        [DashboardWidgetId] uniqueidentifier NOT NULL,
        [NguoiDungId] uniqueidentifier NULL,
        [NhomQuyenId] uniqueidentifier NULL,
        [DonViId] uniqueidentifier NULL,
        [ThuTu] int NOT NULL,
        [HienThi] bit NOT NULL,
        [DangHienThi] nvarchar(50) NOT NULL,
        [KichThuoc] nvarchar(50) NOT NULL,
        [CauHinhBoLocJson] nvarchar(4000) NULL,
        [TuDongLamMoiPhut] int NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DashboardWidgetCauHinhs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_DashboardWidgetCauHinhs_DashboardWidgets_DashboardWidgetId'
)
BEGIN
    ALTER TABLE [DashboardWidgetCauHinhs]
    ADD CONSTRAINT [FK_DashboardWidgetCauHinhs_DashboardWidgets_DashboardWidgetId]
    FOREIGN KEY ([DashboardWidgetId]) REFERENCES [DashboardWidgets] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_DashboardWidgets_MaWidget' AND [object_id] = OBJECT_ID(N'[DashboardWidgets]'))
    CREATE UNIQUE INDEX [IX_DashboardWidgets_MaWidget] ON [DashboardWidgets] ([MaWidget]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_DashboardWidgets_NhomWidget_TrangThai' AND [object_id] = OBJECT_ID(N'[DashboardWidgets]'))
    CREATE INDEX [IX_DashboardWidgets_NhomWidget_TrangThai] ON [DashboardWidgets] ([NhomWidget], [TrangThai]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_DashboardWidgetCauHinhs_DashboardWidgetId_NguoiDungId_NhomQuyenId_DonViId' AND [object_id] = OBJECT_ID(N'[DashboardWidgetCauHinhs]'))
    CREATE INDEX [IX_DashboardWidgetCauHinhs_DashboardWidgetId_NguoiDungId_NhomQuyenId_DonViId] ON [DashboardWidgetCauHinhs] ([DashboardWidgetId], [NguoiDungId], [NhomQuyenId], [DonViId]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_DashboardWidgetCauHinhs_DonViId_ThuTu' AND [object_id] = OBJECT_ID(N'[DashboardWidgetCauHinhs]'))
    CREATE INDEX [IX_DashboardWidgetCauHinhs_DonViId_ThuTu] ON [DashboardWidgetCauHinhs] ([DonViId], [ThuTu]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_DashboardWidgetCauHinhs_NguoiDungId_ThuTu' AND [object_id] = OBJECT_ID(N'[DashboardWidgetCauHinhs]'))
    CREATE INDEX [IX_DashboardWidgetCauHinhs_NguoiDungId_ThuTu] ON [DashboardWidgetCauHinhs] ([NguoiDungId], [ThuTu]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_DashboardWidgetCauHinhs_NhomQuyenId_ThuTu' AND [object_id] = OBJECT_ID(N'[DashboardWidgetCauHinhs]'))
    CREATE INDEX [IX_DashboardWidgetCauHinhs_NhomQuyenId_ThuTu] ON [DashboardWidgetCauHinhs] ([NhomQuyenId], [ThuTu]);

IF OBJECT_ID(N'[__EFMigrationsHistory]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20261011123000_AddDashboardWidgetConfiguration')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261011123000_AddDashboardWidgetConfiguration', N'10.0.12');
END;

SELECT
    OBJECT_ID(N'[DashboardWidgets]', N'U') AS DashboardWidgetsObjectId,
    OBJECT_ID(N'[DashboardWidgetCauHinhs]', N'U') AS DashboardWidgetCauHinhsObjectId;
