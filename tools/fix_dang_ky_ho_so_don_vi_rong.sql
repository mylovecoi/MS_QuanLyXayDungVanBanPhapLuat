SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Empty uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @DonViSoanThao uniqueidentifier;
DECLARE @DonViPheDuyet uniqueidentifier;

SELECT TOP (1) @DonViSoanThao = [Id]
FROM [dm].[DanhMucDonVis]
WHERE [Id] <> @Empty
ORDER BY [CreatedDate], [Id];

SELECT TOP (1) @DonViPheDuyet = [Id]
FROM [dm].[DanhMucDonVis]
WHERE [Id] <> @Empty
  AND [Id] <> @DonViSoanThao
ORDER BY [CreatedDate], [Id];

SET @DonViPheDuyet = COALESCE(@DonViPheDuyet, @DonViSoanThao);

SELECT N'Truoc' AS [Muc],
       COUNT(1) AS [SoHoSoDangGanDonViRong]
FROM [dbo].[DangKyXayDungVanBans]
WHERE [IsDeleted] = 0
  AND [DonViSoanThaoId] = @Empty
  AND (
      [MaHoSo] LIKE N'DK-MAU-%'
      OR [MaHoSo] LIKE N'DK-STEP-%'
      OR [MaHoSo] LIKE N'DK-RETURN-%'
  );

BEGIN TRANSACTION;

UPDATE [dbo].[DangKyXayDungVanBans]
SET [DonViSoanThaoId] = @DonViSoanThao,
    [DonViPheDuyetId] = @DonViPheDuyet,
    [UpdatedAt] = SYSUTCDATETIME(),
    [UpdatedBy] = CONVERT(nvarchar(64), @Empty)
WHERE [IsDeleted] = 0
  AND [DonViSoanThaoId] = @Empty
  AND (
      [MaHoSo] LIKE N'DK-MAU-%'
      OR [MaHoSo] LIKE N'DK-STEP-%'
      OR [MaHoSo] LIKE N'DK-RETURN-%'
  );

SELECT @@ROWCOUNT AS [SoHoSoDaCapNhat],
       @DonViSoanThao AS [DonViSoanThao],
       @DonViPheDuyet AS [DonViPheDuyet];

COMMIT TRANSACTION;

SELECT N'Sau' AS [Muc],
       COUNT(1) AS [SoHoSoDangGanDonViRong]
FROM [dbo].[DangKyXayDungVanBans]
WHERE [IsDeleted] = 0
  AND [DonViSoanThaoId] = @Empty
  AND (
      [MaHoSo] LIKE N'DK-MAU-%'
      OR [MaHoSo] LIKE N'DK-STEP-%'
      OR [MaHoSo] LIKE N'DK-RETURN-%'
  );
