using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DangKyXayDungVanBanService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDangKyXayDungVanBan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DangKyCauHinhChuyenTrangThais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuyTrinhSoanThaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocHienTaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrangThaiHienTaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HanhDongId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocTiepTheoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrangThaiTiepTheoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChuyenBuocId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NhomNhapLieu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    YeuCauLyDo = table.Column<bool>(type: "bit", nullable: false),
                    YeuCauFileDinhKem = table.Column<bool>(type: "bit", nullable: false),
                    LaKetThuc = table.Column<bool>(type: "bit", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyCauHinhChuyenTrangThais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DangKyHanhDongXuLys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHanhDong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenHanhDong = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LoaiHanhDong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    YeuCauLyDo = table.Column<bool>(type: "bit", nullable: false),
                    YeuCauFileDinhKem = table.Column<bool>(type: "bit", nullable: false),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyHanhDongXuLys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DangKyTrangThaiHoSos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaTrangThai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenTrangThai = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MauHienThi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    LaTrangThaiKetThuc = table.Column<bool>(type: "bit", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyTrangThaiHoSos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DangKyXayDungVanBanLienKetQuyTrinhs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DangKyXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoaiVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuyTrinhXayDungId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaQuyTrinhXayDung = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenQuyTrinhXayDung = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NgayLienKet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyXayDungVanBanLienKetQuyTrinhs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DangKyXayDungVanBans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenHoSo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TenVanBanDuKien = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LoaiVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuyTrinhSoanThaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocHienTaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrangThaiHoSoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViSoanThaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViPheDuyetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NamDangKy = table.Column<int>(type: "int", nullable: false),
                    CanCuDeXuat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuCanThiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungChinhSach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuKienThoiGianTrinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KetQuaPheDuyetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DaKhoiTaoQuyTrinhXayDung = table.Column<bool>(type: "bit", nullable: false),
                    HoSoXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuyTrinhXayDungTiepTheoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayKhoiTaoQuyTrinhXayDung = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyXayDungVanBans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DangKyXayDungVanBanFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DangKyXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoaiFile = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenFile = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DuongDanFile = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DungLuong = table.Column<long>(type: "bigint", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyXayDungVanBanFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DangKyXayDungVanBanFiles_DangKyXayDungVanBans_DangKyXayDungVanBanId",
                        column: x => x.DangKyXayDungVanBanId,
                        principalTable: "DangKyXayDungVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DangKyXayDungVanBanKetQuaPheDuyets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DangKyXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoVanBan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayVanBan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CoQuanPheDuyetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenCoQuanPheDuyet = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NguoiKy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ChucVuNguoiKy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NoiDungKetQua = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileKetQuaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyXayDungVanBanKetQuaPheDuyets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DangKyXayDungVanBanKetQuaPheDuyets_DangKyXayDungVanBans_DangKyXayDungVanBanId",
                        column: x => x.DangKyXayDungVanBanId,
                        principalTable: "DangKyXayDungVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DangKyXayDungVanBanLichSuXuLys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DangKyXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TuBuocId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DenBuocId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChuyenBuocId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenBuocTu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TenBuocDen = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    HanhDongId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHanhDongSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenHanhDongSnapshot = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TrangThaiTruocId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrangThaiSauId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenTrangThaiTruocSnapshot = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TenTrangThaiSauSnapshot = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NoiDungXuLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDoTraLai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiXuLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenNguoiXuLy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DonViXuLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenDonViXuLy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyXayDungVanBanLichSuXuLys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DangKyXayDungVanBanLichSuXuLys_DangKyXayDungVanBans_DangKyXayDungVanBanId",
                        column: x => x.DangKyXayDungVanBanId,
                        principalTable: "DangKyXayDungVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DangKyCauHinhChuyenTrangThais_QuyTrinhSoanThaoId_BuocHienTaiId_TrangThaiHienTaiId_HanhDongId",
                table: "DangKyCauHinhChuyenTrangThais",
                columns: new[] { "QuyTrinhSoanThaoId", "BuocHienTaiId", "TrangThaiHienTaiId", "HanhDongId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DangKyHanhDongXuLys_MaHanhDong",
                table: "DangKyHanhDongXuLys",
                column: "MaHanhDong",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DangKyTrangThaiHoSos_MaTrangThai",
                table: "DangKyTrangThaiHoSos",
                column: "MaTrangThai",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBanFiles_DangKyXayDungVanBanId",
                table: "DangKyXayDungVanBanFiles",
                column: "DangKyXayDungVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBanKetQuaPheDuyets_DangKyXayDungVanBanId",
                table: "DangKyXayDungVanBanKetQuaPheDuyets",
                column: "DangKyXayDungVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBanLichSuXuLys_DangKyXayDungVanBanId",
                table: "DangKyXayDungVanBanLichSuXuLys",
                column: "DangKyXayDungVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBanLichSuXuLys_NgayXuLy",
                table: "DangKyXayDungVanBanLichSuXuLys",
                column: "NgayXuLy");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBanLienKetQuyTrinhs_DangKyXayDungVanBanId",
                table: "DangKyXayDungVanBanLienKetQuyTrinhs",
                column: "DangKyXayDungVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBans_BuocHienTaiId",
                table: "DangKyXayDungVanBans",
                column: "BuocHienTaiId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBans_CreatedAt",
                table: "DangKyXayDungVanBans",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBans_DonViPheDuyetId",
                table: "DangKyXayDungVanBans",
                column: "DonViPheDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBans_DonViSoanThaoId",
                table: "DangKyXayDungVanBans",
                column: "DonViSoanThaoId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBans_MaHoSo",
                table: "DangKyXayDungVanBans",
                column: "MaHoSo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBans_TrangThaiHoSoId",
                table: "DangKyXayDungVanBans",
                column: "TrangThaiHoSoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DangKyCauHinhChuyenTrangThais");

            migrationBuilder.DropTable(
                name: "DangKyHanhDongXuLys");

            migrationBuilder.DropTable(
                name: "DangKyTrangThaiHoSos");

            migrationBuilder.DropTable(
                name: "DangKyXayDungVanBanFiles");

            migrationBuilder.DropTable(
                name: "DangKyXayDungVanBanKetQuaPheDuyets");

            migrationBuilder.DropTable(
                name: "DangKyXayDungVanBanLichSuXuLys");

            migrationBuilder.DropTable(
                name: "DangKyXayDungVanBanLienKetQuyTrinhs");

            migrationBuilder.DropTable(
                name: "DangKyXayDungVanBans");
        }
    }
}
