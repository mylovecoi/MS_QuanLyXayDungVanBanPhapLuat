using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanhMucService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitDanhMucSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dm");

            migrationBuilder.CreateTable(
                name: "DanhMucDiaDanhs",
                schema: "dm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenDiaDanh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    DiaDanhCapTrenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucDiaDanhs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucDonVis",
                schema: "dm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenDonVi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    STTSapXep = table.Column<int>(type: "int", nullable: false),
                    DonViChuQuanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaQHNS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChucDanhQuanLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoVaTenNguoiQuanLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanLoaiDonVi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TinhNangThanhToan = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucDonVis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucLinhVucs",
                schema: "dm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaLinhVuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenLinhVuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucLinhVucs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucTieuChiDiemMucs",
                schema: "dm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucTieuChiDiemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TuGiaTri = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DenGiaTri = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    BaoGomTuGiaTri = table.Column<bool>(type: "bit", nullable: false),
                    BaoGomDenGiaTri = table.Column<bool>(type: "bit", nullable: false),
                    Diem = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NhanHienThi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucTieuChiDiemMucs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucTieuChiDiems",
                schema: "dm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaTieuChi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenTieuChi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LoaiTieuChi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KieuGiaTri = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DonViGiaTri = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    DiemToiDa = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucTieuChiDiems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucTrangThais",
                schema: "dm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaTrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenTrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaMauHex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucTrangThais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucVanBans",
                schema: "dm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenLoaiVanBan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CapChinhQuyen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChuTheBanHanh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KyHieuMau = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucVanBans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucPhongBans",
                schema: "dm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenPhongBan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaPhongBan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiPhongBan = table.Column<int>(type: "int", nullable: false),
                    DanhMucDonViId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucPhongBans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucPhongBans_DanhMucDonVis_DanhMucDonViId",
                        column: x => x.DanhMucDonViId,
                        principalSchema: "dm",
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucQuyTrinhSoanThaos",
                schema: "dm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaQuyTrinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenQuyTrinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiQuyTrinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DanhMucVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DanhMucVanBanIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CapApDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhienBan = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucQuyTrinhSoanThaos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucQuyTrinhSoanThaos_DanhMucVanBans_DanhMucVanBanId",
                        column: x => x.DanhMucVanBanId,
                        principalSchema: "dm",
                        principalTable: "DanhMucVanBans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DanhMucCanBos",
                schema: "dm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViQuanLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenCanBo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhongBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GioiTinh = table.Column<bool>(type: "bit", nullable: false),
                    TrinhDoChuyenMon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiLaoDong = table.Column<int>(type: "int", nullable: false),
                    SoTienBHXH = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    SoTienBHYT = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    SoQuyetDinhDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayQuyetDinhDung = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoQuyetDinhBoNhiem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayQuyetDinhBoNhiem = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoQuyetDinhCapThe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayQuyetDinhCapThe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoTheCongChungVien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChucVu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MucPhiBaoHiemTrachNhiem = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    ViTriViecLam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTuyenDung = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoHopDongLaoDong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKyHopDongLaoDong = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucCanBos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucCanBos_DanhMucDonVis_DonViQuanLyId",
                        column: x => x.DonViQuanLyId,
                        principalSchema: "dm",
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhMucCanBos_DanhMucPhongBans_PhongBanId",
                        column: x => x.PhongBanId,
                        principalSchema: "dm",
                        principalTable: "DanhMucPhongBans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DanhMucBuocQuyTrinhs",
                schema: "dm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuyTrinhSoanThaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaBuoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenBuoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false),
                    LoaiBuoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatBuoc = table.Column<bool>(type: "bit", nullable: false),
                    ChoPhepBoQua = table.Column<bool>(type: "bit", nullable: false),
                    ChoPhepQuayLui = table.Column<bool>(type: "bit", nullable: false),
                    CachHoanThanh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoLuongPhanHoiToiThieu = table.Column<int>(type: "int", nullable: true),
                    YeuCauFileDinhKem = table.Column<bool>(type: "bit", nullable: false),
                    SoLanTraLaiToiDa = table.Column<int>(type: "int", nullable: false),
                    SoNgayXuLyTieuChuan = table.Column<int>(type: "int", nullable: true),
                    SoNgayCanhBaoSapHan = table.Column<int>(type: "int", nullable: true),
                    DonViTiepNhanMacDinhId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucBuocQuyTrinhs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucBuocQuyTrinhs_DanhMucDonVis_DonViTiepNhanMacDinhId",
                        column: x => x.DonViTiepNhanMacDinhId,
                        principalSchema: "dm",
                        principalTable: "DanhMucDonVis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DanhMucBuocQuyTrinhs_DanhMucQuyTrinhSoanThaos_QuyTrinhSoanThaoId",
                        column: x => x.QuyTrinhSoanThaoId,
                        principalSchema: "dm",
                        principalTable: "DanhMucQuyTrinhSoanThaos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucChuyenBuocQuyTrinhs",
                schema: "dm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuyTrinhSoanThaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TuBuocId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DenBuocId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DieuKienKetQua = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiChuyenBuoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LaNhanhMacDinh = table.Column<bool>(type: "bit", nullable: false),
                    YeuCauNhapLyDo = table.Column<bool>(type: "bit", nullable: false),
                    IsKetThuc = table.Column<bool>(type: "bit", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucChuyenBuocQuyTrinhs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucChuyenBuocQuyTrinhs_DanhMucBuocQuyTrinhs_DenBuocId",
                        column: x => x.DenBuocId,
                        principalSchema: "dm",
                        principalTable: "DanhMucBuocQuyTrinhs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DanhMucChuyenBuocQuyTrinhs_DanhMucBuocQuyTrinhs_TuBuocId",
                        column: x => x.TuBuocId,
                        principalSchema: "dm",
                        principalTable: "DanhMucBuocQuyTrinhs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DanhMucChuyenBuocQuyTrinhs_DanhMucQuyTrinhSoanThaos_QuyTrinhSoanThaoId",
                        column: x => x.QuyTrinhSoanThaoId,
                        principalSchema: "dm",
                        principalTable: "DanhMucQuyTrinhSoanThaos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucBuocQuyTrinhs_DonViTiepNhanMacDinhId",
                schema: "dm",
                table: "DanhMucBuocQuyTrinhs",
                column: "DonViTiepNhanMacDinhId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucBuocQuyTrinhs_QuyTrinhSoanThaoId",
                schema: "dm",
                table: "DanhMucBuocQuyTrinhs",
                column: "QuyTrinhSoanThaoId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucCanBos_DonViQuanLyId",
                schema: "dm",
                table: "DanhMucCanBos",
                column: "DonViQuanLyId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucCanBos_PhongBanId",
                schema: "dm",
                table: "DanhMucCanBos",
                column: "PhongBanId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucChuyenBuocQuyTrinhs_DenBuocId",
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                column: "DenBuocId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucChuyenBuocQuyTrinhs_QuyTrinhSoanThaoId",
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                column: "QuyTrinhSoanThaoId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucChuyenBuocQuyTrinhs_TuBuocId",
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                column: "TuBuocId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPhongBans_DanhMucDonViId",
                schema: "dm",
                table: "DanhMucPhongBans",
                column: "DanhMucDonViId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucQuyTrinhSoanThaos_DanhMucVanBanId",
                schema: "dm",
                table: "DanhMucQuyTrinhSoanThaos",
                column: "DanhMucVanBanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhMucCanBos",
                schema: "dm");

            migrationBuilder.DropTable(
                name: "DanhMucChuyenBuocQuyTrinhs",
                schema: "dm");

            migrationBuilder.DropTable(
                name: "DanhMucDiaDanhs",
                schema: "dm");

            migrationBuilder.DropTable(
                name: "DanhMucLinhVucs",
                schema: "dm");

            migrationBuilder.DropTable(
                name: "DanhMucTieuChiDiemMucs",
                schema: "dm");

            migrationBuilder.DropTable(
                name: "DanhMucTieuChiDiems",
                schema: "dm");

            migrationBuilder.DropTable(
                name: "DanhMucTrangThais",
                schema: "dm");

            migrationBuilder.DropTable(
                name: "DanhMucPhongBans",
                schema: "dm");

            migrationBuilder.DropTable(
                name: "DanhMucBuocQuyTrinhs",
                schema: "dm");

            migrationBuilder.DropTable(
                name: "DanhMucDonVis",
                schema: "dm");

            migrationBuilder.DropTable(
                name: "DanhMucQuyTrinhSoanThaos",
                schema: "dm");

            migrationBuilder.DropTable(
                name: "DanhMucVanBans",
                schema: "dm");
        }
    }
}
