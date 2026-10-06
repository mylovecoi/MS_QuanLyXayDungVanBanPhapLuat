using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XayDungVanBanService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialXayDungVanBan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenHoSo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TenDuThaoVanBan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DanhMucVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuyTrinhSoanThaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocHienTaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrangThaiHoSoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViChuTriSoanThaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiPhuTrachId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NamXayDung = table.Column<int>(type: "int", nullable: false),
                    ThoiGianDuKienBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianDuKienHoanThanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoSoDangKyXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoHoSoNghiepVus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuocQuyTrinhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoaiBoHoSo = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    LanXuLy = table.Column<int>(type: "int", nullable: false),
                    BoHoSoNguonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayHoanThanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiLapId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViLapId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LyDoTraLai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungGhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoHoSoNghiepVus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoHoSoNghiepVus_BoHoSoNghiepVus_BoHoSoNguonId",
                        column: x => x.BoHoSoNguonId,
                        principalTable: "BoHoSoNghiepVus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BoHoSoNghiepVus_HoSoXayDungVanBans_HoSoXayDungVanBanId",
                        column: x => x.HoSoXayDungVanBanId,
                        principalTable: "HoSoXayDungVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoaiTaiLieuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenTaiLieu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PhienBan = table.Column<int>(type: "int", nullable: false),
                    TenFile = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DuongDanFile = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DungLuong = table.Column<long>(type: "bigint", nullable: true),
                    HashFile = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    NguoiTaiLenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayTaiLen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanFiles_HoSoXayDungVanBans_HoSoXayDungVanBanId",
                        column: x => x.HoSoXayDungVanBanId,
                        principalTable: "HoSoXayDungVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanKetQuaBanHanhs",
                columns: table => new
                {
                    BoHoSoNghiepVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoVanBan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayBanHanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CoQuanBanHanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NguoiKyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChucVuNguoiKy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayCoHieuLuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiDungKetQua = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDoKhongThongQua = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanKetQuaBanHanhs", x => x.BoHoSoNghiepVuId);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanKetQuaBanHanhs_BoHoSoNghiepVus_BoHoSoNghiepVuId",
                        column: x => x.BoHoSoNghiepVuId,
                        principalTable: "BoHoSoNghiepVus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanLichSuXuLys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoHoSoNghiepVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BuocQuyTrinhTruocId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BuocQuyTrinhSauId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrangThaiTruocId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrangThaiSauId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenBuocTruoc = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TenBuocSau = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ThuTuBuocTruoc = table.Column<int>(type: "int", nullable: true),
                    ThuTuBuocSau = table.Column<int>(type: "int", nullable: true),
                    HanhDong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiXuLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViXuLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThoiGianXuLy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoSoXayDungVanBanFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BoHoSoNghiepVuTaiLieuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanLichSuXuLys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanLichSuXuLys_BoHoSoNghiepVus_BoHoSoNghiepVuId",
                        column: x => x.BoHoSoNghiepVuId,
                        principalTable: "BoHoSoNghiepVus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanLichSuXuLys_HoSoXayDungVanBans_HoSoXayDungVanBanId",
                        column: x => x.HoSoXayDungVanBanId,
                        principalTable: "HoSoXayDungVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanSoanThaos",
                columns: table => new
                {
                    BoHoSoNghiepVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanCuXayDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhamViDieuChinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungChinhSach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HinhThucLayYKien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TuNgayLayYKien = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DenNgayLayYKien = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TongSoYKien = table.Column<int>(type: "int", nullable: true),
                    NoiDungTongHopTiepThuGiaiTrinh = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanSoanThaos", x => x.BoHoSoNghiepVuId);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanSoanThaos_BoHoSoNghiepVus_BoHoSoNghiepVuId",
                        column: x => x.BoHoSoNghiepVuId,
                        principalTable: "BoHoSoNghiepVus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanThamDinhs",
                columns: table => new
                {
                    BoHoSoNghiepVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayTiepNhan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HinhThucThamDinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayThamDinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KetQuaThamDinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungKetLuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungYeuCauBoSung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HanBoSung = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayGuiKetQua = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiKetLuanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanThamDinhs", x => x.BoHoSoNghiepVuId);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanThamDinhs_BoHoSoNghiepVus_BoHoSoNghiepVuId",
                        column: x => x.BoHoSoNghiepVuId,
                        principalTable: "BoHoSoNghiepVus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanThamTraHdnds",
                columns: table => new
                {
                    BoHoSoNghiepVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BanHdndThamTraId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayTrinhThamTra = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayNhanKetQuaThamTra = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KetQuaThamTra = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungKienNghi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungTiepThuGiaiTrinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayNhanYKienThaoLuan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiDungTongHopYKienThaoLuan = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanThamTraHdnds", x => x.BoHoSoNghiepVuId);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanThamTraHdnds_BoHoSoNghiepVus_BoHoSoNghiepVuId",
                        column: x => x.BoHoSoNghiepVuId,
                        principalTable: "BoHoSoNghiepVus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanTrinhPheDuyets",
                columns: table => new
                {
                    BoHoSoNghiepVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapTrinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MucDichTrinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoToTrinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayToTrinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTrinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiDungTrinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViDongGuiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TinhTrangXuLyBenNgoaiHeThong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayCapNhatTinhTrang = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanTrinhPheDuyets", x => x.BoHoSoNghiepVuId);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanTrinhPheDuyets_BoHoSoNghiepVus_BoHoSoNghiepVuId",
                        column: x => x.BoHoSoNghiepVuId,
                        principalTable: "BoHoSoNghiepVus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanTrinhThamDinhs",
                columns: table => new
                {
                    BoHoSoNghiepVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoToTrinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayToTrinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayGuiThamDinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DonViNhanThamDinhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoiDungDeNghiThamDinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HanDeNghiTraKetQua = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LyDoTrinhLai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanTrinhThamDinhs", x => x.BoHoSoNghiepVuId);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanTrinhThamDinhs_BoHoSoNghiepVus_BoHoSoNghiepVuId",
                        column: x => x.BoHoSoNghiepVuId,
                        principalTable: "BoHoSoNghiepVus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanYKienUbnds",
                columns: table => new
                {
                    BoHoSoNghiepVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayNhanYKien = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TongSoThanhVienDuocLayYKien = table.Column<int>(type: "int", nullable: true),
                    SoDongY = table.Column<int>(type: "int", nullable: true),
                    SoKhongDongY = table.Column<int>(type: "int", nullable: true),
                    SoYKienKhac = table.Column<int>(type: "int", nullable: true),
                    KetLuanTongHop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungTongHop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungGiaiTrinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanBoSungHoSo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanYKienUbnds", x => x.BoHoSoNghiepVuId);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanYKienUbnds_BoHoSoNghiepVus_BoHoSoNghiepVuId",
                        column: x => x.BoHoSoNghiepVuId,
                        principalTable: "BoHoSoNghiepVus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoHoSoNghiepVuTaiLieus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoHoSoNghiepVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoXayDungVanBanFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoaiTaiLieuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HinhThucThem = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BoHoSoTaiLieuNguonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BatBuoc = table.Column<bool>(type: "bit", nullable: false),
                    DaKiemTra = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoHoSoNghiepVuTaiLieus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoHoSoNghiepVuTaiLieus_BoHoSoNghiepVuTaiLieus_BoHoSoTaiLieuNguonId",
                        column: x => x.BoHoSoTaiLieuNguonId,
                        principalTable: "BoHoSoNghiepVuTaiLieus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BoHoSoNghiepVuTaiLieus_BoHoSoNghiepVus_BoHoSoNghiepVuId",
                        column: x => x.BoHoSoNghiepVuId,
                        principalTable: "BoHoSoNghiepVus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoHoSoNghiepVuTaiLieus_HoSoXayDungVanBanFiles_HoSoXayDungVanBanFileId",
                        column: x => x.HoSoXayDungVanBanFileId,
                        principalTable: "HoSoXayDungVanBanFiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoHoSoNghiepVus_BoHoSoNguonId",
                table: "BoHoSoNghiepVus",
                column: "BoHoSoNguonId");

            migrationBuilder.CreateIndex(
                name: "IX_BoHoSoNghiepVus_HoSoXayDungVanBanId_BuocQuyTrinhId_LanXuLy",
                table: "BoHoSoNghiepVus",
                columns: new[] { "HoSoXayDungVanBanId", "BuocQuyTrinhId", "LanXuLy" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoHoSoNghiepVus_HoSoXayDungVanBanId_LoaiBoHoSo_TrangThai",
                table: "BoHoSoNghiepVus",
                columns: new[] { "HoSoXayDungVanBanId", "LoaiBoHoSo", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_BoHoSoNghiepVuTaiLieus_BoHoSoNghiepVuId_HoSoXayDungVanBanFileId",
                table: "BoHoSoNghiepVuTaiLieus",
                columns: new[] { "BoHoSoNghiepVuId", "HoSoXayDungVanBanFileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoHoSoNghiepVuTaiLieus_BoHoSoTaiLieuNguonId",
                table: "BoHoSoNghiepVuTaiLieus",
                column: "BoHoSoTaiLieuNguonId");

            migrationBuilder.CreateIndex(
                name: "IX_BoHoSoNghiepVuTaiLieus_HoSoXayDungVanBanFileId",
                table: "BoHoSoNghiepVuTaiLieus",
                column: "HoSoXayDungVanBanFileId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanFiles_HoSoXayDungVanBanId_IsCurrent",
                table: "HoSoXayDungVanBanFiles",
                columns: new[] { "HoSoXayDungVanBanId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanFiles_HoSoXayDungVanBanId_LoaiTaiLieuId_PhienBan",
                table: "HoSoXayDungVanBanFiles",
                columns: new[] { "HoSoXayDungVanBanId", "LoaiTaiLieuId", "PhienBan" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanLichSuXuLys_BoHoSoNghiepVuId_ThoiGianXuLy",
                table: "HoSoXayDungVanBanLichSuXuLys",
                columns: new[] { "BoHoSoNghiepVuId", "ThoiGianXuLy" });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanLichSuXuLys_HoSoXayDungVanBanId_ThoiGianXuLy",
                table: "HoSoXayDungVanBanLichSuXuLys",
                columns: new[] { "HoSoXayDungVanBanId", "ThoiGianXuLy" });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBans_CreatedBy_CreatedAt",
                table: "HoSoXayDungVanBans",
                columns: new[] { "CreatedBy", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBans_DanhMucVanBanId_NamXayDung",
                table: "HoSoXayDungVanBans",
                columns: new[] { "DanhMucVanBanId", "NamXayDung" });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBans_DonViChuTriSoanThaoId_TrangThaiHoSoId_CreatedAt",
                table: "HoSoXayDungVanBans",
                columns: new[] { "DonViChuTriSoanThaoId", "TrangThaiHoSoId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBans_HoSoDangKyXayDungVanBanId",
                table: "HoSoXayDungVanBans",
                column: "HoSoDangKyXayDungVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBans_MaHoSo",
                table: "HoSoXayDungVanBans",
                column: "MaHoSo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBans_NguoiPhuTrachId_CreatedAt",
                table: "HoSoXayDungVanBans",
                columns: new[] { "NguoiPhuTrachId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBans_QuyTrinhSoanThaoId_BuocHienTaiId_TrangThaiHoSoId",
                table: "HoSoXayDungVanBans",
                columns: new[] { "QuyTrinhSoanThaoId", "BuocHienTaiId", "TrangThaiHoSoId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoHoSoNghiepVuTaiLieus");

            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanKetQuaBanHanhs");

            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanLichSuXuLys");

            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanSoanThaos");

            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanThamDinhs");

            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanThamTraHdnds");

            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanTrinhPheDuyets");

            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanTrinhThamDinhs");

            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanYKienUbnds");

            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanFiles");

            migrationBuilder.DropTable(
                name: "BoHoSoNghiepVus");

            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBans");
        }
    }
}
