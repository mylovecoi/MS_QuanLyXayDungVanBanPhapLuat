using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyReportSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaoCaoKhaoSats",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CuocKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenBaoCao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SoKyHieu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayBaoCao = table.Column<DateOnly>(type: "date", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayChot = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UuDiem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HanChe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KienNghi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoCaoKhaoSats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietBaoCaoKhaoSats",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaoCaoKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CauHoiThongKeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaCauHoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDungCauHoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaLuaChon = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NoiDungLuaChon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoLuong = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MauSoTyLe = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TyLe = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    YKienTuDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietBaoCaoKhaoSats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaoCaoKhaoSats_CuocKhaoSatId_TrangThai",
                schema: "kspl",
                table: "BaoCaoKhaoSats",
                columns: new[] { "CuocKhaoSatId", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietBaoCaoKhaoSats_BaoCaoKhaoSatId_CauHoiThongKeId_MaLuaChon",
                schema: "kspl",
                table: "ChiTietBaoCaoKhaoSats",
                columns: new[] { "BaoCaoKhaoSatId", "CauHoiThongKeId", "MaLuaChon" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaoCaoKhaoSats",
                schema: "kspl");

            migrationBuilder.DropTable(
                name: "ChiTietBaoCaoKhaoSats",
                schema: "kspl");
        }
    }
}
