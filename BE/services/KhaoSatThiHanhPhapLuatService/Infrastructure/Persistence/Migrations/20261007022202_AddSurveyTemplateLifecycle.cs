using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyTemplateLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NgayHetHieuLuc",
                schema: "kspl",
                table: "MauPhieuKhaoSats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayHieuLuc",
                schema: "kspl",
                table: "MauPhieuKhaoSats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhienDocMauPhieuKhaoSatId",
                schema: "kspl",
                table: "MauPhieuKhaoSats",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThaiMauPhieu",
                schema: "kspl",
                table: "MauPhieuKhaoSats",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MauPhieuKhaoSats_NhomDoiTuongKhaoSatId",
                schema: "kspl",
                table: "MauPhieuKhaoSats",
                column: "NhomDoiTuongKhaoSatId",
                unique: true,
                filter: "[TrangThaiMauPhieu] = N'DANG_SU_DUNG'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MauPhieuKhaoSats_NhomDoiTuongKhaoSatId",
                schema: "kspl",
                table: "MauPhieuKhaoSats");

            migrationBuilder.DropColumn(
                name: "NgayHetHieuLuc",
                schema: "kspl",
                table: "MauPhieuKhaoSats");

            migrationBuilder.DropColumn(
                name: "NgayHieuLuc",
                schema: "kspl",
                table: "MauPhieuKhaoSats");

            migrationBuilder.DropColumn(
                name: "PhienDocMauPhieuKhaoSatId",
                schema: "kspl",
                table: "MauPhieuKhaoSats");

            migrationBuilder.DropColumn(
                name: "TrangThaiMauPhieu",
                schema: "kspl",
                table: "MauPhieuKhaoSats");
        }
    }
}
