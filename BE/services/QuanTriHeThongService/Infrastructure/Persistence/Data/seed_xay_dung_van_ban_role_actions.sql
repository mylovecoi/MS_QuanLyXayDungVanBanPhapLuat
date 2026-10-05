/*
    Seed RoleAction cho XayDungVanBanService.
    Quy tac PM: moi RoleAction Detail co mot Form va mot Controller rieng.
    Script idempotent: co the chay lai an toan de bo sung/cap nhat cau hinh.
*/
SET NOCOUNT ON;

DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @Now datetime2 = SYSUTCDATETIME();
DECLARE @VanBanQpplGroupId uniqueidentifier;
DECLARE @XayDungVanBanGroupId uniqueidentifier = '91000000-0000-0000-0000-000000000001';

SELECT @VanBanQpplGroupId = [Id]
FROM [qtht].[RoleActions]
WHERE [Role] = N'VanBanQPPL';

IF @VanBanQpplGroupId IS NULL
    THROW 51000, N'Khong tim thay RoleAction cha VanBanQPPL. Hay seed nhom VanBanQPPL truoc.', 1;

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
(@XayDungVanBanGroupId, 200, N'Group', 0, N'VanBanQPPL.XayDungVanBan', @VanBanQpplGroupId,
 N'Xây dựng văn bản QPPL', NULL, NULL, NULL, N'/xay-dung-van-ban', N'Xây dựng văn bản QPPL', N'file-text'),
(N'91000000-0000-0000-0000-000000000002', 201, N'Detail', 1, N'VanBanQPPL.XayDungVanBan.DanhSach', @XayDungVanBanGroupId,
 N'Danh sách xây dựng văn bản', N'XayDungVanBanDanhSach', N'Index', N'HoSoXayDungVanBans', N'/xay-dung-van-ban/danh-sach', N'Danh sách', N'list'),
(N'91000000-0000-0000-0000-000000000003', 202, N'Detail', 1, N'VanBanQPPL.XayDungVanBan.SoanThao', @XayDungVanBanGroupId,
 N'Tổ chức soạn thảo và ý kiến góp ý', N'XayDungVanBanSoanThao', N'Index', N'HoSoXayDungVanBans', N'/xay-dung-van-ban/soan-thao', N'Soạn thảo', N'file-plus'),
(N'91000000-0000-0000-0000-000000000004', 203, N'Detail', 1, N'VanBanQPPL.XayDungVanBan.TrinhThamDinh', @XayDungVanBanGroupId,
 N'Trình thẩm định dự thảo văn bản', N'XayDungVanBanTrinhThamDinh', N'Index', N'BoHoSoNghiepVus', N'/xay-dung-van-ban/trinh-tham-dinh', N'Trình thẩm định', N'send'),
(N'91000000-0000-0000-0000-000000000005', 204, N'Detail', 1, N'VanBanQPPL.XayDungVanBan.ThamDinh', @XayDungVanBanGroupId,
 N'Thẩm định dự thảo văn bản', N'XayDungVanBanThamDinh', N'Index', N'HoSoXayDungVanBanThamDinhs', N'/xay-dung-van-ban/tham-dinh', N'Thẩm định', N'clipboard-check'),
(N'91000000-0000-0000-0000-000000000006', 205, N'Detail', 1, N'VanBanQPPL.XayDungVanBan.TrinhPheDuyet', @XayDungVanBanGroupId,
 N'Trình phê duyệt hoặc cho ý kiến', N'XayDungVanBanTrinhPheDuyet', N'Index', N'HoSoXayDungVanBanTrinhPheDuyets', N'/xay-dung-van-ban/trinh-phe-duyet', N'Trình phê duyệt', N'file-check'),
(N'91000000-0000-0000-0000-000000000007', 206, N'Detail', 1, N'VanBanQPPL.XayDungVanBan.YKienUbnd', @XayDungVanBanGroupId,
 N'Ý kiến thành viên UBND tỉnh', N'XayDungVanBanYKienUbnd', N'Index', N'HoSoXayDungVanBanYKienUbnds', N'/xay-dung-van-ban/y-kien-ubnd', N'Ý kiến UBND', N'messages-square'),
(N'91000000-0000-0000-0000-000000000008', 207, N'Detail', 1, N'VanBanQPPL.XayDungVanBan.ThamTraHdnd', @XayDungVanBanGroupId,
 N'Thẩm tra HĐND dự thảo Nghị quyết', N'XayDungVanBanThamTraHdnd', N'Index', N'HoSoXayDungVanBanThamTraHdnds', N'/xay-dung-van-ban/tham-tra-hdnd', N'Thẩm tra HĐND', N'landmark'),
(N'91000000-0000-0000-0000-000000000009', 208, N'Detail', 1, N'VanBanQPPL.XayDungVanBan.BanHanh', @XayDungVanBanGroupId,
 N'Ban hành hoặc thông qua văn bản', N'XayDungVanBanBanHanh', N'Index', N'HoSoXayDungVanBanKetQuaBanHanhs', N'/xay-dung-van-ban/ban-hanh', N'Ban hành', N'badge-check');

/* Them moi neu RoleAction chua ton tai. */
INSERT INTO [qtht].[RoleActions]
(
    [Id], [STTSapXep], [PhanLoai], [Level], [Role], [RoleGroupId], [Title],
    [Controller], [Action], [Parameter], [Table], [Status], [UseGroup],
    [FrontendPath], [IsVisibleInMenu], [ClientApp], [MenuTitle], [MenuIcon], [Icon],
    [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]
)
SELECT r.[Id], r.[STTSapXep], r.[PhanLoai], r.[Level], r.[Role],
       CASE WHEN r.[Role] = N'VanBanQPPL.XayDungVanBan' THEN @VanBanQpplGroupId ELSE @XayDungVanBanGroupId END,
       r.[Title], r.[Controller], r.[Action], NULL, r.[Table], N'Kích hoạt', NULL,
       r.[FrontendPath], 1, N'frontend', r.[MenuTitle], r.[MenuIcon], NULL,
       @SystemUserId, @Now, @SystemUserId, @Now
FROM @RoleActions r
WHERE NOT EXISTS (SELECT 1 FROM [qtht].[RoleActions] e WHERE e.[Role] = r.[Role]);

/* Cap nhat mapping hien co de luon khop Form/Controller/route. */
UPDATE e
SET e.[STTSapXep] = r.[STTSapXep],
    e.[PhanLoai] = r.[PhanLoai],
    e.[Level] = r.[Level],
    e.[RoleGroupId] = CASE WHEN r.[Role] = N'VanBanQPPL.XayDungVanBan' THEN @VanBanQpplGroupId ELSE @XayDungVanBanGroupId END,
    e.[Title] = r.[Title], e.[Controller] = r.[Controller], e.[Action] = r.[Action],
    e.[Table] = r.[Table], e.[Status] = N'Kích hoạt', e.[FrontendPath] = r.[FrontendPath],
    e.[IsVisibleInMenu] = 1, e.[ClientApp] = N'frontend', e.[MenuTitle] = r.[MenuTitle],
    e.[MenuIcon] = r.[MenuIcon], e.[UpdatedBy] = @SystemUserId, e.[UpdatedDate] = @Now
FROM [qtht].[RoleActions] e
JOIN @RoleActions r ON r.[Role] = e.[Role];

SELECT [Role], [PhanLoai], [Controller], [Action], [FrontendPath], [Table], [Status]
FROM [qtht].[RoleActions]
WHERE [Role] = N'VanBanQPPL.XayDungVanBan'
   OR [Role] LIKE N'VanBanQPPL.XayDungVanBan.%'
ORDER BY [STTSapXep];
