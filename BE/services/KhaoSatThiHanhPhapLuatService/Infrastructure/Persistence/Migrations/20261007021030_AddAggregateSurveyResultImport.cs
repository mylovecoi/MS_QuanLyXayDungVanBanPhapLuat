using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAggregateSurveyResultImport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CuocKhaoSatId",
                schema: "kspl",
                table: "PhieuNopKhaoSats",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NhomDoiTuongKhaoSatId",
                schema: "kspl",
                table: "PhieuNopKhaoSats",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CuocKhaoSatId",
                schema: "kspl",
                table: "PhieuNopKhaoSats");

            migrationBuilder.DropColumn(
                name: "NhomDoiTuongKhaoSatId",
                schema: "kspl",
                table: "PhieuNopKhaoSats");
        }
    }
}
