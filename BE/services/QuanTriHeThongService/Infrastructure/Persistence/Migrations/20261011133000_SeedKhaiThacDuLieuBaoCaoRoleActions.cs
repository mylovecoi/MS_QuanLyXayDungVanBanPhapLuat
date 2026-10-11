using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using QuanTriHeThongService.Infrastructure.Persistence;

#nullable disable

namespace QuanTriHeThongService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(QuanTriHeThongDbContext))]
[Migration("20261011133000_SeedKhaiThacDuLieuBaoCaoRoleActions")]
public partial class SeedKhaiThacDuLieuBaoCaoRoleActions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            SET NOCOUNT ON;

            DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
            DECLARE @Now datetime2 = SYSUTCDATETIME();
            DECLARE @VanBanQpplGroupId uniqueidentifier;
            DECLARE @BaoCaoGroupId uniqueidentifier;

            SELECT @VanBanQpplGroupId = [Id] FROM [qtht].[RoleActions] WHERE [Role] = N'VanBanQPPL';
            IF @VanBanQpplGroupId IS NULL
                THROW 51000, N'Khong tim thay RoleAction cha VanBanQPPL.', 1;

            SELECT @BaoCaoGroupId = [Id] FROM [qtht].[RoleActions] WHERE [Role] = N'VanBanQPPL.BaoCaoThongKe';
            IF @BaoCaoGroupId IS NULL
                SET @BaoCaoGroupId = '94000000-0000-0000-0000-000000000004';

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
            (@BaoCaoGroupId, 430, N'Group', 0, N'VanBanQPPL.BaoCaoThongKe', @VanBanQpplGroupId,
             N'Báo cáo thống kê', NULL, NULL, NULL, N'/bao-cao-thong-ke', N'Báo cáo thống kê', N'file-chart-column'),
            ('94000000-0000-0000-0000-000000000401', 431, N'Detail', 1, N'VanBanQPPL.BaoCaoThongKe.TongHop', @BaoCaoGroupId,
             N'Báo cáo thống kê tổng hợp', N'BaoCaoThongKe', N'Index', NULL, N'/bao-cao-thong-ke', N'Danh mục báo cáo', N'file-chart-column');

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
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DELETE FROM [qtht].[RoleActions]
            WHERE [Role] = N'VanBanQPPL.BaoCaoThongKe.TongHop';

            DELETE ra
            FROM [qtht].[RoleActions] ra
            WHERE ra.[Role] = N'VanBanQPPL.BaoCaoThongKe'
              AND NOT EXISTS (
                  SELECT 1
                  FROM [qtht].[RoleActions] child
                  WHERE child.[RoleGroupId] = ra.[Id]
              );
            """);
    }
}
