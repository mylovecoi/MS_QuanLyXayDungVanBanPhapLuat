using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using QuanTriHeThongService.Infrastructure.Persistence;

#nullable disable

namespace QuanTriHeThongService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(QuanTriHeThongDbContext))]
    [Migration("20261011113000_SeedXayDungVanBanKetQuaChamDiemRoleActions")]
    public partial class SeedXayDungVanBanKetQuaChamDiemRoleActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                SET NOCOUNT ON;

                DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
                DECLARE @Now datetime2 = SYSUTCDATETIME();
                DECLARE @XayDungVanBanGroupId uniqueidentifier;

                SELECT @XayDungVanBanGroupId = [Id]
                FROM [qtht].[RoleActions]
                WHERE [Role] = N'VanBanQPPL.XayDungVanBan';

                IF @XayDungVanBanGroupId IS NULL
                    THROW 51000, N'Khong tim thay RoleAction cha VanBanQPPL.XayDungVanBan.', 1;

                UPDATE [qtht].[RoleActions]
                SET [IsVisibleInMenu] = 0,
                    [UpdatedBy] = @SystemUserId,
                    [UpdatedDate] = @Now
                WHERE [Role] = N'XayDungVanBanChamDiem';

                DECLARE @Rows TABLE
                (
                    [Id] uniqueidentifier NOT NULL,
                    [STTSapXep] int NOT NULL,
                    [Role] nvarchar(250) NOT NULL,
                    [Title] nvarchar(500) NOT NULL,
                    [Controller] nvarchar(250) NOT NULL,
                    [FrontendPath] nvarchar(500) NOT NULL,
                    [MenuTitle] nvarchar(500) NOT NULL,
                    [MenuIcon] nvarchar(100) NOT NULL
                );

                INSERT INTO @Rows
                VALUES
                ('91000000-0000-0000-0000-000000000011', 210, N'VanBanQPPL.XayDungVanBan.ChamDiem',
                 N'Chấm điểm hồ sơ xây dựng văn bản', N'XayDungVanBanChamDiem', N'/xay-dung-van-ban/cham-diem', N'Chấm điểm', N'star'),
                ('91000000-0000-0000-0000-000000000012', 211, N'VanBanQPPL.XayDungVanBan.KetQuaChamDiem',
                 N'Kết quả chấm điểm hồ sơ xây dựng văn bản', N'XayDungVanBanChamDiem', N'/xay-dung-van-ban/ket-qua-cham-diem', N'Kết quả chấm điểm', N'file-spreadsheet');

                INSERT INTO [qtht].[RoleActions]
                (
                    [Id], [STTSapXep], [PhanLoai], [Level], [Role], [RoleGroupId], [Title],
                    [Controller], [Action], [Parameter], [Table], [Status], [UseGroup],
                    [FrontendPath], [IsVisibleInMenu], [ClientApp], [MenuTitle], [MenuIcon], [Icon],
                    [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]
                )
                SELECT r.[Id], r.[STTSapXep], N'Detail', 1, r.[Role], @XayDungVanBanGroupId, r.[Title],
                       r.[Controller], N'Index', NULL, N'HoSoXayDungVanBanChamDiems', N'Kích hoạt', NULL,
                       r.[FrontendPath], 1, N'frontend', r.[MenuTitle], r.[MenuIcon], r.[MenuIcon],
                       @SystemUserId, @Now, @SystemUserId, @Now
                FROM @Rows r
                WHERE NOT EXISTS (SELECT 1 FROM [qtht].[RoleActions] e WHERE e.[Role] = r.[Role]);

                UPDATE e
                SET e.[STTSapXep] = r.[STTSapXep],
                    e.[PhanLoai] = N'Detail',
                    e.[Level] = 1,
                    e.[RoleGroupId] = @XayDungVanBanGroupId,
                    e.[Title] = r.[Title],
                    e.[Controller] = r.[Controller],
                    e.[Action] = N'Index',
                    e.[Table] = N'HoSoXayDungVanBanChamDiems',
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
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM [qtht].[RoleActions]
                WHERE [Role] IN (N'VanBanQPPL.XayDungVanBan.ChamDiem', N'VanBanQPPL.XayDungVanBan.KetQuaChamDiem');

                UPDATE [qtht].[RoleActions]
                SET [IsVisibleInMenu] = 1
                WHERE [Role] = N'XayDungVanBanChamDiem';
                """);
        }
    }
}
