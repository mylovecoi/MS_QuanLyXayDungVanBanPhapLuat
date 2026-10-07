using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaiThacDuLieuService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialKhaiThacDuLieu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CanhBaoKhaiThacDuLieus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaCanhBao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NhomCanhBao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DoiTuongNguon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DoiTuongNguonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    MucDo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThaiXuLy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HanXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayPhatSinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiNhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonViNhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NguoiXuLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayXem = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChuXuLy = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanhBaoKhaiThacDuLieus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CauHinhCanhBaoKhaiThacDuLieus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaCanhBao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenCanhBao = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NhomCanhBao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoNgayCanhBaoTruocHan = table.Column<int>(type: "int", nullable: false),
                    MucDoMacDinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KenhThongBao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhCanhBaoKhaiThacDuLieus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DongBoKhaiThacDuLieuLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguonDuLieu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoaiDongBo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BatDauLuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KetThucLuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SoBanGhi = table.Column<int>(type: "int", nullable: false),
                    Loi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DongBoKhaiThacDuLieuLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CanhBaoKhaiThacDuLieus_DoiTuongNguon_DoiTuongNguonId_MaCanhBao",
                table: "CanhBaoKhaiThacDuLieus",
                columns: new[] { "DoiTuongNguon", "DoiTuongNguonId", "MaCanhBao" });

            migrationBuilder.CreateIndex(
                name: "IX_CanhBaoKhaiThacDuLieus_DonViNhanId_TrangThaiXuLy_NgayPhatSinh",
                table: "CanhBaoKhaiThacDuLieus",
                columns: new[] { "DonViNhanId", "TrangThaiXuLy", "NgayPhatSinh" });

            migrationBuilder.CreateIndex(
                name: "IX_CanhBaoKhaiThacDuLieus_NguoiNhanId_TrangThaiXuLy_NgayPhatSinh",
                table: "CanhBaoKhaiThacDuLieus",
                columns: new[] { "NguoiNhanId", "TrangThaiXuLy", "NgayPhatSinh" });

            migrationBuilder.CreateIndex(
                name: "IX_CanhBaoKhaiThacDuLieus_NhomCanhBao_TrangThaiXuLy_NgayPhatSinh",
                table: "CanhBaoKhaiThacDuLieus",
                columns: new[] { "NhomCanhBao", "TrangThaiXuLy", "NgayPhatSinh" });

            migrationBuilder.CreateIndex(
                name: "IX_CauHinhCanhBaoKhaiThacDuLieus_MaCanhBao",
                table: "CauHinhCanhBaoKhaiThacDuLieus",
                column: "MaCanhBao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CauHinhCanhBaoKhaiThacDuLieus_NhomCanhBao_TrangThai",
                table: "CauHinhCanhBaoKhaiThacDuLieus",
                columns: new[] { "NhomCanhBao", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_DongBoKhaiThacDuLieuLogs_NguonDuLieu_LoaiDongBo_BatDauLuc",
                table: "DongBoKhaiThacDuLieuLogs",
                columns: new[] { "NguonDuLieu", "LoaiDongBo", "BatDauLuc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CanhBaoKhaiThacDuLieus");

            migrationBuilder.DropTable(
                name: "CauHinhCanhBaoKhaiThacDuLieus");

            migrationBuilder.DropTable(
                name: "DongBoKhaiThacDuLieuLogs");
        }
    }
}
