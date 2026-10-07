using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ChoPhepNhieuLuaChon",
                schema: "kspl",
                table: "CauHoiThongKes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CoYKienTuDo",
                schema: "kspl",
                table: "CauHoiThongKes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MauSoTyLe",
                schema: "kspl",
                table: "CauHoiThongKes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "PHIEU_HOP_LE");

            migrationBuilder.AddColumn<bool>(
                name: "BatBuoc",
                schema: "kspl",
                table: "CauHoiMauPhieus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ChoPhepNhieuLuaChon",
                schema: "kspl",
                table: "CauHoiMauPhieus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CoYKienTuDo",
                schema: "kspl",
                table: "CauHoiMauPhieus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MauSoTyLe",
                schema: "kspl",
                table: "CauHoiMauPhieus",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "PHIEU_HOP_LE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChoPhepNhieuLuaChon",
                schema: "kspl",
                table: "CauHoiThongKes");

            migrationBuilder.DropColumn(
                name: "CoYKienTuDo",
                schema: "kspl",
                table: "CauHoiThongKes");

            migrationBuilder.DropColumn(
                name: "MauSoTyLe",
                schema: "kspl",
                table: "CauHoiThongKes");

            migrationBuilder.DropColumn(
                name: "BatBuoc",
                schema: "kspl",
                table: "CauHoiMauPhieus");

            migrationBuilder.DropColumn(
                name: "ChoPhepNhieuLuaChon",
                schema: "kspl",
                table: "CauHoiMauPhieus");

            migrationBuilder.DropColumn(
                name: "CoYKienTuDo",
                schema: "kspl",
                table: "CauHoiMauPhieus");

            migrationBuilder.DropColumn(
                name: "MauSoTyLe",
                schema: "kspl",
                table: "CauHoiMauPhieus");
        }
    }
}
