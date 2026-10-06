using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XayDungVanBanService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHoSoChamDiem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanChamDiems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanCham = table.Column<int>(type: "int", nullable: false),
                    TrangThaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TongDiemTuDong = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TongDiemDieuChinh = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TongDiemChinhThuc = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NgayBatDauThucTe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayHoanThanhThucTe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoNgayKeHoach = table.Column<int>(type: "int", nullable: true),
                    SoNgayThucTe = table.Column<int>(type: "int", nullable: true),
                    TyLeThoiGianThucTe = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    NgayCham = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiChamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayChot = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiChotId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanChamDiems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanChamDiems_HoSoXayDungVanBans_HoSoXayDungVanBanId",
                        column: x => x.HoSoXayDungVanBanId,
                        principalTable: "HoSoXayDungVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanChamDiemChiTiets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoXayDungVanBanChamDiemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucTieuChiDiemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucTieuChiDiemMucId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaTieuChi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenTieuChi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NhanMucDiem = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    GiaTriDauVao = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiemToiDa = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiemTuDong = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiemDieuChinh = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiemChinhThuc = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LyDoDieuChinh = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NguoiDieuChinhId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayDieuChinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanChamDiemChiTiets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanChamDiemChiTiets_HoSoXayDungVanBanChamDiems_HoSoXayDungVanBanChamDiemId",
                        column: x => x.HoSoXayDungVanBanChamDiemId,
                        principalTable: "HoSoXayDungVanBanChamDiems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanChamDiemLichSus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoXayDungVanBanChamDiemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoXayDungVanBanChamDiemChiTietId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LoaiThaoTac = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DuLieuCu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuLieuMoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiThucHienId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThoiGianThucHien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanChamDiemLichSus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanChamDiemLichSus_HoSoXayDungVanBanChamDiems_HoSoXayDungVanBanChamDiemId",
                        column: x => x.HoSoXayDungVanBanChamDiemId,
                        principalTable: "HoSoXayDungVanBanChamDiems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanChamDiemChiTiets_HoSoXayDungVanBanChamDiemId_DanhMucTieuChiDiemId",
                table: "HoSoXayDungVanBanChamDiemChiTiets",
                columns: new[] { "HoSoXayDungVanBanChamDiemId", "DanhMucTieuChiDiemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanChamDiemLichSus_HoSoXayDungVanBanChamDiemId_ThoiGianThucHien",
                table: "HoSoXayDungVanBanChamDiemLichSus",
                columns: new[] { "HoSoXayDungVanBanChamDiemId", "ThoiGianThucHien" });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanChamDiems_HoSoXayDungVanBanId_LanCham",
                table: "HoSoXayDungVanBanChamDiems",
                columns: new[] { "HoSoXayDungVanBanId", "LanCham" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanChamDiems_HoSoXayDungVanBanId_TrangThaiId",
                table: "HoSoXayDungVanBanChamDiems",
                columns: new[] { "HoSoXayDungVanBanId", "TrangThaiId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanChamDiemChiTiets");

            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanChamDiemLichSus");

            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanChamDiems");
        }
    }
}
