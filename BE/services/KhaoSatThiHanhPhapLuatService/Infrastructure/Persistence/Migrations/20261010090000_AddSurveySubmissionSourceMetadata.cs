using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(KhaoSatThiHanhPhapLuatDbContext))]
[Migration("20261010090000_AddSurveySubmissionSourceMetadata")]
public partial class AddSurveySubmissionSourceMetadata : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name: "TenPhanMem", schema: "kspl", table: "PhieuNopKhaoSats", type: "nvarchar(250)", maxLength: 250, nullable: true);
        migrationBuilder.AddColumn<string>(name: "PhienBanPhanMem", schema: "kspl", table: "PhieuNopKhaoSats", type: "nvarchar(100)", maxLength: 100, nullable: true);
        migrationBuilder.AddColumn<string>(name: "DuongDanHeThongNguon", schema: "kspl", table: "PhieuNopKhaoSats", type: "nvarchar(1000)", maxLength: 1000, nullable: true);
        migrationBuilder.AddColumn<string>(name: "GhiChuNguonDuLieu", schema: "kspl", table: "PhieuNopKhaoSats", type: "nvarchar(max)", nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "TenPhanMem", schema: "kspl", table: "PhieuNopKhaoSats");
        migrationBuilder.DropColumn(name: "PhienBanPhanMem", schema: "kspl", table: "PhieuNopKhaoSats");
        migrationBuilder.DropColumn(name: "DuongDanHeThongNguon", schema: "kspl", table: "PhieuNopKhaoSats");
        migrationBuilder.DropColumn(name: "GhiChuNguonDuLieu", schema: "kspl", table: "PhieuNopKhaoSats");
    }
}
