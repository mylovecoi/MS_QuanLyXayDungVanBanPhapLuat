using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyReportExportFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DuongDanFileXuat",
                schema: "kspl",
                table: "BaoCaoKhaoSats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayXuat",
                schema: "kspl",
                table: "BaoCaoKhaoSats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenFileXuat",
                schema: "kspl",
                table: "BaoCaoKhaoSats",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuongDanFileXuat",
                schema: "kspl",
                table: "BaoCaoKhaoSats");

            migrationBuilder.DropColumn(
                name: "NgayXuat",
                schema: "kspl",
                table: "BaoCaoKhaoSats");

            migrationBuilder.DropColumn(
                name: "TenFileXuat",
                schema: "kspl",
                table: "BaoCaoKhaoSats");
        }
    }
}
