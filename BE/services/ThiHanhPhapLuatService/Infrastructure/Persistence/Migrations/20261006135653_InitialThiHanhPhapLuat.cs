using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThiHanhPhapLuatService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialThiHanhPhapLuat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "thpl");

            migrationBuilder.CreateTable(
                name: "BaoCaoTienDoThiHanhs",
                schema: "thpl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoiDungKeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhanCongThiHanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KyBaoCao = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TyLeHoanThanh = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KhoKhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KienNghi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayBaoCao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoCaoTienDoThiHanhs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaoCaoTongHopChiTiets",
                schema: "thpl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaoCaoTongHopThiHanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoiDungKeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrangThaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TyLeHoanThanh = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KhoKhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KienNghi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoCaoTongHopChiTiets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaoCaoTongHopThiHanhs",
                schema: "thpl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaBaoCao = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KyBaoCao = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TuNgay = table.Column<DateOnly>(type: "date", nullable: false),
                    DenNgay = table.Column<DateOnly>(type: "date", nullable: false),
                    TrangThaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoLieuTongHopJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayChot = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiChotId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoCaoTongHopThiHanhs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhGiaThiHanhs",
                schema: "thpl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoiDungKeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaoCaoTienDoThiHanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KetQuaDanhGia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NhanXet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiSauId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiDanhGiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGiaThiHanhs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeHoachCanCuPhapLys",
                schema: "thpl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SoKyHieu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrichYeu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeHoachCanCuPhapLys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeHoachThiHanhPhapLuats",
                schema: "thpl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaKeHoach = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenKeHoach = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    TuNgay = table.Column<DateOnly>(type: "date", nullable: false),
                    DenNgay = table.Column<DateOnly>(type: "date", nullable: false),
                    DonViChuTriId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiPhuTrachId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrangThaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhamVi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MucTieu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeHoachThiHanhPhapLuats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LichSuNhacViecThiHanhs",
                schema: "thpl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoiDungKeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LoaiNhacViec = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NguoiNhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonViNhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThoiGianGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KenhGui = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuNhacViecThiHanhs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LichSuXuLyThiHanhs",
                schema: "thpl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoiDungKeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HanhDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiTruocId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrangThaiSauId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianXuLy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiXuLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViXuLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuXuLyThiHanhs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhanCongThiHanhs",
                schema: "thpl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoiDungKeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViDuocGiaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanBoDuocGiaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HanThucHien = table.Column<DateOnly>(type: "date", nullable: false),
                    MucDoUuTien = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanCongThiHanhs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TepDinhKemThiHanhs",
                schema: "thpl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NoiDungKeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BaoCaoTienDoThiHanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LoaiTaiLieu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenTep = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DuongDan = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    PhienBan = table.Column<int>(type: "int", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TepDinhKemThiHanhs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauBoSungThiHanhs",
                schema: "thpl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoiDungKeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaoCaoTienDoThiHanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NoiDungYeuCau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HanBoSung = table.Column<DateOnly>(type: "date", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayHoanThanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauBoSungThiHanhs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NoiDungKeHoachs",
                schema: "thpl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeHoachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaNoiDung = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenNoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChiTieu = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HanHoanThanh = table.Column<DateOnly>(type: "date", nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    TrangThaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TyLeHoanThanh = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoiDungKeHoachs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoiDungKeHoachs_KeHoachThiHanhPhapLuats_KeHoachId",
                        column: x => x.KeHoachId,
                        principalSchema: "thpl",
                        principalTable: "KeHoachThiHanhPhapLuats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaoCaoTienDoThiHanhs_NoiDungKeHoachId_KyBaoCao_CreatedAt",
                schema: "thpl",
                table: "BaoCaoTienDoThiHanhs",
                columns: new[] { "NoiDungKeHoachId", "KyBaoCao", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_BaoCaoTongHopChiTiets_BaoCaoTongHopThiHanhId_NoiDungKeHoachId",
                schema: "thpl",
                table: "BaoCaoTongHopChiTiets",
                columns: new[] { "BaoCaoTongHopThiHanhId", "NoiDungKeHoachId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BaoCaoTongHopThiHanhs_KeHoachId_KyBaoCao",
                schema: "thpl",
                table: "BaoCaoTongHopThiHanhs",
                columns: new[] { "KeHoachId", "KyBaoCao" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BaoCaoTongHopThiHanhs_MaBaoCao",
                schema: "thpl",
                table: "BaoCaoTongHopThiHanhs",
                column: "MaBaoCao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KeHoachThiHanhPhapLuats_DonViChuTriId_TrangThaiId_Nam",
                schema: "thpl",
                table: "KeHoachThiHanhPhapLuats",
                columns: new[] { "DonViChuTriId", "TrangThaiId", "Nam" });

            migrationBuilder.CreateIndex(
                name: "IX_KeHoachThiHanhPhapLuats_MaKeHoach",
                schema: "thpl",
                table: "KeHoachThiHanhPhapLuats",
                column: "MaKeHoach",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichSuNhacViecThiHanhs_NoiDungKeHoachId_LoaiNhacViec_ThoiGianGui",
                schema: "thpl",
                table: "LichSuNhacViecThiHanhs",
                columns: new[] { "NoiDungKeHoachId", "LoaiNhacViec", "ThoiGianGui" });

            migrationBuilder.CreateIndex(
                name: "IX_LichSuXuLyThiHanhs_KeHoachId_ThoiGianXuLy",
                schema: "thpl",
                table: "LichSuXuLyThiHanhs",
                columns: new[] { "KeHoachId", "ThoiGianXuLy" });

            migrationBuilder.CreateIndex(
                name: "IX_NoiDungKeHoachs_KeHoachId_MaNoiDung",
                schema: "thpl",
                table: "NoiDungKeHoachs",
                columns: new[] { "KeHoachId", "MaNoiDung" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NoiDungKeHoachs_KeHoachId_TrangThaiId_HanHoanThanh",
                schema: "thpl",
                table: "NoiDungKeHoachs",
                columns: new[] { "KeHoachId", "TrangThaiId", "HanHoanThanh" });

            migrationBuilder.CreateIndex(
                name: "IX_TepDinhKemThiHanhs_KeHoachId_LoaiTaiLieu_PhienBan",
                schema: "thpl",
                table: "TepDinhKemThiHanhs",
                columns: new[] { "KeHoachId", "LoaiTaiLieu", "PhienBan" });

            migrationBuilder.CreateIndex(
                name: "IX_TepDinhKemThiHanhs_NoiDungKeHoachId_BaoCaoTienDoThiHanhId_IsCurrent",
                schema: "thpl",
                table: "TepDinhKemThiHanhs",
                columns: new[] { "NoiDungKeHoachId", "BaoCaoTienDoThiHanhId", "IsCurrent" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaoCaoTienDoThiHanhs",
                schema: "thpl");

            migrationBuilder.DropTable(
                name: "BaoCaoTongHopChiTiets",
                schema: "thpl");

            migrationBuilder.DropTable(
                name: "BaoCaoTongHopThiHanhs",
                schema: "thpl");

            migrationBuilder.DropTable(
                name: "DanhGiaThiHanhs",
                schema: "thpl");

            migrationBuilder.DropTable(
                name: "KeHoachCanCuPhapLys",
                schema: "thpl");

            migrationBuilder.DropTable(
                name: "LichSuNhacViecThiHanhs",
                schema: "thpl");

            migrationBuilder.DropTable(
                name: "LichSuXuLyThiHanhs",
                schema: "thpl");

            migrationBuilder.DropTable(
                name: "NoiDungKeHoachs",
                schema: "thpl");

            migrationBuilder.DropTable(
                name: "PhanCongThiHanhs",
                schema: "thpl");

            migrationBuilder.DropTable(
                name: "TepDinhKemThiHanhs",
                schema: "thpl");

            migrationBuilder.DropTable(
                name: "YeuCauBoSungThiHanhs",
                schema: "thpl");

            migrationBuilder.DropTable(
                name: "KeHoachThiHanhPhapLuats",
                schema: "thpl");
        }
    }
}
