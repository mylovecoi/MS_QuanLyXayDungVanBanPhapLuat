using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using QuanTriHeThongService.Infrastructure.Persistence;

#nullable disable

namespace QuanTriHeThongService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(QuanTriHeThongDbContext))]
[Migration("20261009090000_SeedThiHanhPhapLuatPermissionGroups")]
public partial class SeedThiHanhPhapLuatPermissionGroups : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DECLARE @Now datetime2 = SYSUTCDATETIME();
            DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
            DECLARE @ChuTriId uniqueidentifier = '94000000-0000-0000-0000-000000000001';
            DECLARE @ThucHienId uniqueidentifier = '94000000-0000-0000-0000-000000000002';

            SELECT @ChuTriId = Id FROM [qtht].[GroupsPermision] WHERE Name = N'Đơn vị chủ trì THPL';
            IF NOT EXISTS (SELECT 1 FROM [qtht].[GroupsPermision] WHERE Id = @ChuTriId)
                INSERT INTO [qtht].[GroupsPermision] ([Id],[Name],[Description],[Status],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
                VALUES (@ChuTriId,N'Đơn vị chủ trì THPL',N'Quản lý kế hoạch, phân công, đánh giá và tổng hợp thi hành pháp luật.',N'Kích hoạt',@SystemUserId,@Now,@SystemUserId,@Now);

            SELECT @ThucHienId = Id FROM [qtht].[GroupsPermision] WHERE Name = N'Đơn vị thực hiện THPL';
            IF NOT EXISTS (SELECT 1 FROM [qtht].[GroupsPermision] WHERE Id = @ThucHienId)
                INSERT INTO [qtht].[GroupsPermision] ([Id],[Name],[Description],[Status],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
                VALUES (@ThucHienId,N'Đơn vị thực hiện THPL',N'Cập nhật và gửi báo cáo tiến độ thi hành pháp luật theo phân công.',N'Kích hoạt',@SystemUserId,@Now,@SystemUserId,@Now);

            DECLARE @Matrix TABLE ([GroupId] uniqueidentifier,[Role] nvarchar(250),[CanIndex] bit,[CanCreate] bit,[CanEdit] bit,[CanDelete] bit,[CanApprove] bit);
            INSERT INTO @Matrix VALUES
              (@ChuTriId,N'ThiHanhPhapLuat',1,0,0,0,0),
              (@ChuTriId,N'ThiHanhPhapLuat.DanhSach',1,0,0,0,0),
              (@ChuTriId,N'ThiHanhPhapLuat.KeHoach',1,1,1,1,0),
              (@ChuTriId,N'ThiHanhPhapLuat.NoiDungKeHoach',1,1,1,1,0),
              (@ChuTriId,N'ThiHanhPhapLuat.PhanCong',1,1,1,1,0),
              (@ChuTriId,N'ThiHanhPhapLuat.TienDo',1,0,0,0,0),
              (@ChuTriId,N'ThiHanhPhapLuat.DanhGia',1,0,0,0,1),
              (@ChuTriId,N'ThiHanhPhapLuat.TongHop',1,1,0,0,1),
              (@ThucHienId,N'ThiHanhPhapLuat',1,0,0,0,0),
              (@ThucHienId,N'ThiHanhPhapLuat.TienDo',1,1,1,0,0);

            INSERT INTO [qtht].[Permission] ([Id],[GroupPermissionId],[RoleActionId],[Status],[Index],[Create],[Edit],[Delete],[Approve],[Public],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
            SELECT NEWID(),m.GroupId,r.Id,N'Kích hoạt',m.CanIndex,m.CanCreate,m.CanEdit,m.CanDelete,m.CanApprove,0,@SystemUserId,@Now,@SystemUserId,@Now
            FROM @Matrix m JOIN [qtht].[RoleActions] r ON r.Role=m.Role
            WHERE NOT EXISTS (SELECT 1 FROM [qtht].[Permission] p WHERE p.GroupPermissionId=m.GroupId AND p.RoleActionId=r.Id);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DELETE p FROM [qtht].[Permission] p JOIN [qtht].[GroupsPermision] g ON g.Id=p.GroupPermissionId WHERE g.Name IN (N'Đơn vị chủ trì THPL',N'Đơn vị thực hiện THPL');
            DELETE FROM [qtht].[GroupsPermision] WHERE Name IN (N'Đơn vị chủ trì THPL',N'Đơn vị thực hiện THPL');
            """);
    }
}
