using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using QuanTriHeThongService.Infrastructure.Persistence;

#nullable disable

namespace QuanTriHeThongService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(QuanTriHeThongDbContext))]
[Migration("20261007040000_SeedKhaiThacDuLieuTraCuuRoleActions")]
public partial class SeedKhaiThacDuLieuTraCuuRoleActions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DECLARE @Now datetime2 = SYSUTCDATETIME();
            DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
            DECLARE @RootId uniqueidentifier = '66666666-6666-6666-6666-666666666600';
            DECLARE @GroupId uniqueidentifier = '66666666-6666-6666-6666-666666666601';
            DECLARE @ExistingTraCuuGroupId uniqueidentifier;

            SELECT TOP (1) @ExistingTraCuuGroupId = [Id]
            FROM [qtht].[RoleActions]
            WHERE [PhanLoai] = N'Group'
              AND ([MenuTitle] = N'Tra cứu, tìm kiếm' OR [Title] = N'Tra cứu, tìm kiếm')
            ORDER BY [Level] DESC, [STTSapXep];

            IF @ExistingTraCuuGroupId IS NOT NULL
                SET @GroupId = @ExistingTraCuuGroupId;

            IF @ExistingTraCuuGroupId IS NULL AND NOT EXISTS (SELECT 1 FROM [qtht].[RoleActions] WHERE [Role] = N'KhaiThacDuLieu')
                INSERT INTO [qtht].[RoleActions] ([Id],[STTSapXep],[PhanLoai],[Level],[Role],[RoleGroupId],[Title],[Status],[FrontendPath],[IsVisibleInMenu],[ClientApp],[MenuTitle],[MenuIcon],[Icon],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
                VALUES (@RootId, 600, N'Group', 0, N'KhaiThacDuLieu', @RootId, N'Khai thác dữ liệu', N'Kích hoạt', N'/khai-thac-du-lieu', 1, N'frontend', N'Khai thác dữ liệu', N'search', N'search', @SystemUserId, @Now, @SystemUserId, @Now);

            IF @ExistingTraCuuGroupId IS NULL AND NOT EXISTS (SELECT 1 FROM [qtht].[RoleActions] WHERE [Role] = N'KhaiThacDuLieu.TraCuu')
                INSERT INTO [qtht].[RoleActions] ([Id],[STTSapXep],[PhanLoai],[Level],[Role],[RoleGroupId],[Title],[Status],[FrontendPath],[IsVisibleInMenu],[ClientApp],[MenuTitle],[MenuIcon],[Icon],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
                VALUES (@GroupId, 601, N'Group', 1, N'KhaiThacDuLieu.TraCuu', @RootId, N'Tra cứu dữ liệu', N'Kích hoạt', N'/khai-thac-du-lieu/tra-cuu', 1, N'frontend', N'Tra cứu', N'search', N'search', @SystemUserId, @Now, @SystemUserId, @Now);

            DECLARE @Rows TABLE ([Id] uniqueidentifier, [STT] int, [Role] nvarchar(250), [Title] nvarchar(500), [Controller] nvarchar(250), [Path] nvarchar(500), [MenuTitle] nvarchar(500));
            INSERT INTO @Rows VALUES
              ('66666666-6666-6666-6666-666666666602',602,N'KhaiThacDuLieu.TraCuu.TongHop',N'Tra cứu tổng hợp',N'TraCuuTongHop',N'/khai-thac-du-lieu/tra-cuu/tong-hop',N'Tra cứu tổng hợp'),
              ('66666666-6666-6666-6666-666666666603',603,N'KhaiThacDuLieu.TraCuu.DangKyXayDungVanBan',N'Tra cứu đăng ký xây dựng văn bản',N'TraCuuDangKyXayDungVanBan',N'/khai-thac-du-lieu/tra-cuu/dang-ky-xay-dung-van-ban',N'Tra cứu đăng ký xây dựng'),
              ('66666666-6666-6666-6666-666666666604',604,N'KhaiThacDuLieu.TraCuu.XayDungVanBan',N'Tra cứu xây dựng văn bản',N'TraCuuXayDungVanBan',N'/khai-thac-du-lieu/tra-cuu/xay-dung-van-ban',N'Tra cứu xây dựng văn bản'),
              ('66666666-6666-6666-6666-666666666605',605,N'KhaiThacDuLieu.TraCuu.ThiHanhPhapLuat',N'Tra cứu thi hành pháp luật',N'TraCuuThiHanhPhapLuat',N'/khai-thac-du-lieu/tra-cuu/thi-hanh-phap-luat',N'Tra cứu thi hành pháp luật');

            INSERT INTO [qtht].[RoleActions] ([Id],[STTSapXep],[PhanLoai],[Level],[Role],[RoleGroupId],[Title],[Controller],[Action],[Table],[Status],[FrontendPath],[IsVisibleInMenu],[ClientApp],[MenuTitle],[MenuIcon],[Icon],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
            SELECT r.[Id],r.[STT],N'Detail',2,r.[Role],@GroupId,r.[Title],r.[Controller],N'Index',NULL,N'Kích hoạt',r.[Path],1,N'frontend',r.[MenuTitle],N'search',N'search',@SystemUserId,@Now,@SystemUserId,@Now
            FROM @Rows r WHERE NOT EXISTS (SELECT 1 FROM [qtht].[RoleActions] e WHERE e.[Role] = r.[Role]);

            UPDATE e
            SET [RoleGroupId] = @GroupId,
                [Controller] = r.[Controller],
                [Action] = N'Index',
                [FrontendPath] = r.[Path],
                [IsVisibleInMenu] = 1,
                [Status] = N'Kích hoạt',
                [MenuTitle] = r.[MenuTitle],
                [UpdatedBy] = @SystemUserId,
                [UpdatedDate] = @Now
            FROM [qtht].[RoleActions] e
            JOIN @Rows r ON r.[Role] = e.[Role];
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("DELETE FROM [qtht].[RoleActions] WHERE [Id] IN ('66666666-6666-6666-6666-666666666602','66666666-6666-6666-6666-666666666603','66666666-6666-6666-6666-666666666604','66666666-6666-6666-6666-666666666605','66666666-6666-6666-6666-666666666601','66666666-6666-6666-6666-666666666600')");
}
