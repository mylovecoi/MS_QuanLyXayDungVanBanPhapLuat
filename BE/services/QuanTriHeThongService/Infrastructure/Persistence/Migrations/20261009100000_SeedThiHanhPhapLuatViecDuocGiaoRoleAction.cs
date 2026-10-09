using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using QuanTriHeThongService.Infrastructure.Persistence;

#nullable disable

namespace QuanTriHeThongService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(QuanTriHeThongDbContext))]
[Migration("20261009100000_SeedThiHanhPhapLuatViecDuocGiaoRoleAction")]
public partial class SeedThiHanhPhapLuatViecDuocGiaoRoleAction : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DECLARE @Now datetime2 = SYSUTCDATETIME();
            DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
            DECLARE @ParentId uniqueidentifier;

            SELECT @ParentId = Id FROM [qtht].[RoleActions] WHERE [Role] = N'ThiHanhPhapLuat';

            IF @ParentId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [qtht].[RoleActions] WHERE [Role] = N'ThiHanhPhapLuat.ViecDuocGiao')
                INSERT INTO [qtht].[RoleActions]
                    ([Id],[STTSapXep],[PhanLoai],[Level],[Role],[RoleGroupId],[Title],[Controller],[Action],[Table],[Status],[FrontendPath],[IsVisibleInMenu],[ClientApp],[MenuTitle],[MenuIcon],[Icon],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
                VALUES
                    ('93000000-0000-0000-0000-000000000009',308,N'Detail',1,N'ThiHanhPhapLuat.ViecDuocGiao',@ParentId,N'Việc được giao thi hành pháp luật',N'ThiHanhPhapLuatViecDuocGiao',N'Index',N'PhanCongThiHanhs',N'Kích hoạt',N'/thi-hanh-phap-luat/viec-duoc-giao',1,N'frontend',N'Việc được giao',N'clipboard-list',N'clipboard-list',@SystemUserId,@Now,@SystemUserId,@Now);

            INSERT INTO [qtht].[Permission] ([Id],[GroupPermissionId],[RoleActionId],[Status],[Index],[Create],[Edit],[Delete],[Approve],[Public],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
            SELECT NEWID(),g.Id,r.Id,N'Kích hoạt',1,0,0,0,0,0,@SystemUserId,@Now,@SystemUserId,@Now
            FROM [qtht].[GroupsPermision] g
            JOIN [qtht].[RoleActions] r ON r.[Role] = N'ThiHanhPhapLuat.ViecDuocGiao'
            WHERE g.[Name] IN (N'Đơn vị chủ trì THPL',N'Đơn vị thực hiện THPL')
              AND NOT EXISTS (SELECT 1 FROM [qtht].[Permission] p WHERE p.GroupPermissionId = g.Id AND p.RoleActionId = r.Id);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DELETE p FROM [qtht].[Permission] p
            JOIN [qtht].[RoleActions] r ON r.Id = p.RoleActionId
            WHERE r.[Role] = N'ThiHanhPhapLuat.ViecDuocGiao';
            DELETE FROM [qtht].[RoleActions] WHERE [Role] = N'ThiHanhPhapLuat.ViecDuocGiao';
            """);
    }
}
