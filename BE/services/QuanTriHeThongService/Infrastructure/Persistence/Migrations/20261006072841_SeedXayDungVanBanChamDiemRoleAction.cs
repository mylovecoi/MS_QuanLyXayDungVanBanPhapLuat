using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanTriHeThongService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedXayDungVanBanChamDiemRoleAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "qtht",
                table: "RoleActions",
                columns: new[] { "Id", "STTSapXep", "PhanLoai", "Level", "Role", "RoleGroupId", "Title", "Controller", "Action", "Parameter", "Table", "Status", "UseGroup", "FrontendPath", "IsVisibleInMenu", "ClientApp", "MenuTitle", "MenuIcon", "Icon", "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" },
                values: new object[] { new System.Guid("44444444-4444-4444-4444-444444444401"), 90, "Detail", 0, "XayDungVanBanChamDiem", System.Guid.Empty, "Chấm điểm xây dựng văn bản", "XayDungVanBanChamDiem", "Index", null, "HoSoXayDungVanBanChamDiems", "Kích hoạt", null, "/xay-dung-van-ban/cham-diem", true, "Frontend", "Chấm điểm xây dựng văn bản", "star", "star", System.Guid.Empty, new System.DateTime(2026, 10, 6), System.Guid.Empty, new System.DateTime(2026, 10, 6) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(schema: "qtht", table: "RoleActions", keyColumn: "Id", keyValue: new System.Guid("44444444-4444-4444-4444-444444444401"));
        }
    }
}
