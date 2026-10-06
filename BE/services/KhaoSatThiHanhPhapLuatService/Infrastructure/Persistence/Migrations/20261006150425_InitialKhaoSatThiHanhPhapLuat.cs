using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialKhaoSatThiHanhPhapLuat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "kspl");

            migrationBuilder.CreateTable(
                name: "CauHoiMauPhieus",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MauPhieuKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CauHoiThongKeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaCauHoi = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiCauHoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoiMauPhieus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CauHoiThongKes",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CuocKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaCauHoiThongKe = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiCauHoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoiThongKes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CauTraLoiKhaoSats",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhieuNopKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CauHoiThongKeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CauHoiMauPhieuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LuaChonTraLoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GiaTriText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaTriSo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauTraLoiKhaoSats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CuocKhaoSats",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaCuocKhaoSat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenCuocKhaoSat = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MucDich = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhamVi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinhVucId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonViChuTriId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TuNgay = table.Column<DateOnly>(type: "date", nullable: false),
                    DenNgay = table.Column<DateOnly>(type: "date", nullable: false),
                    TrangThaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeHoachThiHanhPhapLuatId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NoiDungKeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuocKhaoSats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DoiTuongKhaoSats",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CuocKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhomDoiTuongKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MauPhieuKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanBoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HanNop = table.Column<DateOnly>(type: "date", nullable: false),
                    TrangThaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoiTuongKhaoSats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LichSuXuLyKhaoSats",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CuocKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoiTuongKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhieuNopKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HanhDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianXuLy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiXuLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViXuLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuXuLyKhaoSats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoiImportKhaoSats",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhieuNopKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViTri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaCauHoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDungLoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoiImportKhaoSats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LuaChonTraLois",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CauHoiMauPhieuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaLuaChon = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LuaChonTraLois", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MauPhieuKhaoSats",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CuocKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhomDoiTuongKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhienBan = table.Column<int>(type: "int", nullable: false),
                    TenFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuongDanFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DaPhatHanh = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MauPhieuKhaoSats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhomDoiTuongKhaoSats",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CuocKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaNhom = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenNhom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomDoiTuongKhaoSats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhieuNopKhaoSats",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoiTuongKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MauPhieuKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuongDanFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayImport = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuNopKhaoSats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CauHoiMauPhieus_MauPhieuKhaoSatId_MaCauHoi",
                schema: "kspl",
                table: "CauHoiMauPhieus",
                columns: new[] { "MauPhieuKhaoSatId", "MaCauHoi" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CauHoiThongKes_CuocKhaoSatId_MaCauHoiThongKe",
                schema: "kspl",
                table: "CauHoiThongKes",
                columns: new[] { "CuocKhaoSatId", "MaCauHoiThongKe" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CuocKhaoSats_DonViChuTriId_TrangThaiId_CreatedAt",
                schema: "kspl",
                table: "CuocKhaoSats",
                columns: new[] { "DonViChuTriId", "TrangThaiId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CuocKhaoSats_MaCuocKhaoSat",
                schema: "kspl",
                table: "CuocKhaoSats",
                column: "MaCuocKhaoSat",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoiTuongKhaoSats_CuocKhaoSatId_DonViId",
                schema: "kspl",
                table: "DoiTuongKhaoSats",
                columns: new[] { "CuocKhaoSatId", "DonViId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichSuXuLyKhaoSats_CuocKhaoSatId_ThoiGianXuLy",
                schema: "kspl",
                table: "LichSuXuLyKhaoSats",
                columns: new[] { "CuocKhaoSatId", "ThoiGianXuLy" });

            migrationBuilder.CreateIndex(
                name: "IX_LuaChonTraLois_CauHoiMauPhieuId_MaLuaChon",
                schema: "kspl",
                table: "LuaChonTraLois",
                columns: new[] { "CauHoiMauPhieuId", "MaLuaChon" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MauPhieuKhaoSats_NhomDoiTuongKhaoSatId_PhienBan",
                schema: "kspl",
                table: "MauPhieuKhaoSats",
                columns: new[] { "NhomDoiTuongKhaoSatId", "PhienBan" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhomDoiTuongKhaoSats_CuocKhaoSatId_MaNhom",
                schema: "kspl",
                table: "NhomDoiTuongKhaoSats",
                columns: new[] { "CuocKhaoSatId", "MaNhom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNopKhaoSats_DoiTuongKhaoSatId_CreatedAt",
                schema: "kspl",
                table: "PhieuNopKhaoSats",
                columns: new[] { "DoiTuongKhaoSatId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CauHoiMauPhieus",
                schema: "kspl");

            migrationBuilder.DropTable(
                name: "CauHoiThongKes",
                schema: "kspl");

            migrationBuilder.DropTable(
                name: "CauTraLoiKhaoSats",
                schema: "kspl");

            migrationBuilder.DropTable(
                name: "CuocKhaoSats",
                schema: "kspl");

            migrationBuilder.DropTable(
                name: "DoiTuongKhaoSats",
                schema: "kspl");

            migrationBuilder.DropTable(
                name: "LichSuXuLyKhaoSats",
                schema: "kspl");

            migrationBuilder.DropTable(
                name: "LoiImportKhaoSats",
                schema: "kspl");

            migrationBuilder.DropTable(
                name: "LuaChonTraLois",
                schema: "kspl");

            migrationBuilder.DropTable(
                name: "MauPhieuKhaoSats",
                schema: "kspl");

            migrationBuilder.DropTable(
                name: "NhomDoiTuongKhaoSats",
                schema: "kspl");

            migrationBuilder.DropTable(
                name: "PhieuNopKhaoSats",
                schema: "kspl");
        }
    }
}
