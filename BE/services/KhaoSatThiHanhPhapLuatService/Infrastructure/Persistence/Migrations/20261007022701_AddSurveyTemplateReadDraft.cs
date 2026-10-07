using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyTemplateReadDraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CauHoiNhapMauPhieuKhaoSats",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhienDocMauPhieuKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaCauHoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiCauHoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaLuaChon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungLuaChon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BatBuoc = table.Column<bool>(type: "bit", nullable: false),
                    ChoPhepNhieuLuaChon = table.Column<bool>(type: "bit", nullable: false),
                    CoYKienTuDo = table.Column<bool>(type: "bit", nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoiNhapMauPhieuKhaoSats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhienDocMauPhieuKhaoSats",
                schema: "kspl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CuocKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhomDoiTuongKhaoSatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuongDanFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhienDocMauPhieuKhaoSats", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CauHoiNhapMauPhieuKhaoSats",
                schema: "kspl");

            migrationBuilder.DropTable(
                name: "PhienDocMauPhieuKhaoSats",
                schema: "kspl");
        }
    }
}
