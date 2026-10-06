/*
    Seed RoleActions cho quy trinh De xuat danh muc/Dang ky xay dung van ban QPPL.

    Nguon nghiep vu: TaiLieu/QT dang ky.docx - muc "Bang Usecase".
    Quy tac PM: moi RoleAction phan loai Detail co 1 Form va 1 Controller rieng.
*/

SET NOCOUNT ON;

DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @Now datetime2 = SYSUTCDATETIME();

DECLARE @VanBanQpplGroupId uniqueidentifier;
DECLARE @DangKyXayDungGroupId uniqueidentifier = '90000000-0000-0000-0000-000000000001';
DECLARE @DanhSachDangKyId uniqueidentifier = '90000000-0000-0000-0000-000000000002';
DECLARE @QuanLyDangKyId uniqueidentifier = '90000000-0000-0000-0000-000000000003';
DECLARE @KetQuaDangKyId uniqueidentifier = '90000000-0000-0000-0000-000000000004';

SELECT @VanBanQpplGroupId = [Id]
FROM [qtht].[RoleActions]
WHERE [Role] = N'VanBanQPPL';

IF @VanBanQpplGroupId IS NULL
BEGIN
    THROW 51000, N'Khong tim thay RoleAction cha VanBanQPPL. Hay seed nhom VanBanQPPL truoc khi seed DangKyXayDung.', 1;
END;

IF NOT EXISTS (
    SELECT 1
    FROM [qtht].[RoleActions]
    WHERE [Role] = N'VanBanQPPL.DangKyXayDung'
)
BEGIN
    INSERT INTO [qtht].[RoleActions]
    (
        [Id],
        [STTSapXep],
        [PhanLoai],
        [Level],
        [Role],
        [RoleGroupId],
        [Title],
        [Controller],
        [Action],
        [Parameter],
        [Table],
        [Status],
        [UseGroup],
        [FrontendPath],
        [IsVisibleInMenu],
        [ClientApp],
        [MenuTitle],
        [MenuIcon],
        [Icon],
        [CreatedBy],
        [CreatedDate],
        [UpdatedBy],
        [UpdatedDate]
    )
    VALUES
    (
        @DangKyXayDungGroupId,
        100,
        N'Group',
        0,
        N'VanBanQPPL.DangKyXayDung',
        @VanBanQpplGroupId,
        N'Đăng ký soạn thảo văn bản',
        NULL,
        NULL,
        NULL,
        NULL,
        N'Kích hoạt',
        NULL,
        N'/dang-ky-xay-dung-van-ban',
        1,
        N'frontend',
        N'Đăng ký soạn thảo văn bản',
        N'file-text',
        NULL,
        @SystemUserId,
        @Now,
        @SystemUserId,
        @Now
    );
END;

SELECT @DangKyXayDungGroupId = [Id]
FROM [qtht].[RoleActions]
WHERE [Role] = N'VanBanQPPL.DangKyXayDung';

IF NOT EXISTS (
    SELECT 1
    FROM [qtht].[RoleActions]
    WHERE [Role] = N'VanBanQPPL.DangKyXayDung.DanhSachDangKy'
)
BEGIN
    INSERT INTO [qtht].[RoleActions]
    (
        [Id],
        [STTSapXep],
        [PhanLoai],
        [Level],
        [Role],
        [RoleGroupId],
        [Title],
        [Controller],
        [Action],
        [Parameter],
        [Table],
        [Status],
        [UseGroup],
        [FrontendPath],
        [IsVisibleInMenu],
        [ClientApp],
        [MenuTitle],
        [MenuIcon],
        [Icon],
        [CreatedBy],
        [CreatedDate],
        [UpdatedBy],
        [UpdatedDate]
    )
    VALUES
    (
        @DanhSachDangKyId,
        101,
        N'Detail',
        1,
        N'VanBanQPPL.DangKyXayDung.DanhSachDangKy',
        @DangKyXayDungGroupId,
        N'Danh sách đăng ký soạn thảo văn bản',
        N'DangKyXayDungVanBanDanhSach',
        N'Index',
        NULL,
        N'DangKyXayDungVanBans',
        N'Kích hoạt',
        NULL,
        N'/dang-ky-xay-dung-van-ban/danh-sach',
        1,
        N'frontend',
        N'Danh sách đăng ký',
        N'list',
        NULL,
        @SystemUserId,
        @Now,
        @SystemUserId,
        @Now
    );
END;

IF NOT EXISTS (
    SELECT 1
    FROM [qtht].[RoleActions]
    WHERE [Role] = N'VanBanQPPL.DangKyXayDung.QuanLyDangKy'
)
BEGIN
    INSERT INTO [qtht].[RoleActions]
    (
        [Id],
        [STTSapXep],
        [PhanLoai],
        [Level],
        [Role],
        [RoleGroupId],
        [Title],
        [Controller],
        [Action],
        [Parameter],
        [Table],
        [Status],
        [UseGroup],
        [FrontendPath],
        [IsVisibleInMenu],
        [ClientApp],
        [MenuTitle],
        [MenuIcon],
        [Icon],
        [CreatedBy],
        [CreatedDate],
        [UpdatedBy],
        [UpdatedDate]
    )
    VALUES
    (
        @QuanLyDangKyId,
        102,
        N'Detail',
        1,
        N'VanBanQPPL.DangKyXayDung.QuanLyDangKy',
        @DangKyXayDungGroupId,
        N'Hồ sơ đăng ký soạn thảo văn bản',
        N'DangKyXayDungVanBanHoSo',
        N'Index',
        NULL,
        N'DangKyXayDungVanBans',
        N'Kích hoạt',
        NULL,
        N'/dang-ky-xay-dung-van-ban/ho-so',
        1,
        N'frontend',
        N'Hồ sơ đăng ký',
        N'file-plus',
        NULL,
        @SystemUserId,
        @Now,
        @SystemUserId,
        @Now
    );
END;

IF NOT EXISTS (
    SELECT 1
    FROM [qtht].[RoleActions]
    WHERE [Role] = N'VanBanQPPL.DangKyXayDung.KetQuaDangKy'
)
BEGIN
    INSERT INTO [qtht].[RoleActions]
    (
        [Id],
        [STTSapXep],
        [PhanLoai],
        [Level],
        [Role],
        [RoleGroupId],
        [Title],
        [Controller],
        [Action],
        [Parameter],
        [Table],
        [Status],
        [UseGroup],
        [FrontendPath],
        [IsVisibleInMenu],
        [ClientApp],
        [MenuTitle],
        [MenuIcon],
        [Icon],
        [CreatedBy],
        [CreatedDate],
        [UpdatedBy],
        [UpdatedDate]
    )
    VALUES
    (
        @KetQuaDangKyId,
        103,
        N'Detail',
        1,
        N'VanBanQPPL.DangKyXayDung.KetQuaDangKy',
        @DangKyXayDungGroupId,
        N'Kết quả đăng ký soạn thảo văn bản',
        N'DangKyXayDungVanBanKetQua',
        N'Index',
        NULL,
        N'DangKyXayDungVanBanKetQuaPheDuyets',
        N'Kích hoạt',
        NULL,
        N'/dang-ky-xay-dung-van-ban/ket-qua',
        1,
        N'frontend',
        N'Kết quả đăng ký',
        N'check-square',
        NULL,
        @SystemUserId,
        @Now,
        @SystemUserId,
        @Now
    );
END;

SELECT
    [Id],
    [STTSapXep],
    [PhanLoai],
    [Level],
    [Role],
    [RoleGroupId],
    [Title],
    [Controller],
    [Action],
    [Table],
    [Status],
    [FrontendPath],
    [ClientApp],
    [MenuTitle],
    [MenuIcon]
FROM [qtht].[RoleActions]
WHERE [Role] IN
(
    N'VanBanQPPL.DangKyXayDung',
    N'VanBanQPPL.DangKyXayDung.DanhSachDangKy',
    N'VanBanQPPL.DangKyXayDung.QuanLyDangKy',
    N'VanBanQPPL.DangKyXayDung.KetQuaDangKy'
)
ORDER BY [Level], [STTSapXep];
