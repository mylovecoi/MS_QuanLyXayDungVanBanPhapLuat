using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using KhaiThacDuLieuService.Infrastructure.Persistence;

#nullable disable

namespace KhaiThacDuLieuService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(KhaiThacDuLieuDbContext))]
[Migration("20261011123000_AddDashboardWidgetConfiguration")]
public partial class AddDashboardWidgetConfiguration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DashboardWidgets",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                MaWidget = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                TenWidget = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                NhomWidget = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                LoaiWidget = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                TrangThai = table.Column<bool>(type: "bit", nullable: false),
                CauHinhMacDinhJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DashboardWidgets", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "DashboardWidgetCauHinhs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DashboardWidgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                NguoiDungId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                NhomQuyenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DonViId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                ThuTu = table.Column<int>(type: "int", nullable: false),
                HienThi = table.Column<bool>(type: "bit", nullable: false),
                DangHienThi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                KichThuoc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                CauHinhBoLocJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                TuDongLamMoiPhut = table.Column<int>(type: "int", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DashboardWidgetCauHinhs", x => x.Id);
                table.ForeignKey(
                    name: "FK_DashboardWidgetCauHinhs_DashboardWidgets_DashboardWidgetId",
                    column: x => x.DashboardWidgetId,
                    principalTable: "DashboardWidgets",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_DashboardWidgets_MaWidget", table: "DashboardWidgets", column: "MaWidget", unique: true);
        migrationBuilder.CreateIndex(name: "IX_DashboardWidgets_NhomWidget_TrangThai", table: "DashboardWidgets", columns: new[] { "NhomWidget", "TrangThai" });
        migrationBuilder.CreateIndex(name: "IX_DashboardWidgetCauHinhs_DashboardWidgetId_NguoiDungId_NhomQuyenId_DonViId", table: "DashboardWidgetCauHinhs", columns: new[] { "DashboardWidgetId", "NguoiDungId", "NhomQuyenId", "DonViId" });
        migrationBuilder.CreateIndex(name: "IX_DashboardWidgetCauHinhs_DonViId_ThuTu", table: "DashboardWidgetCauHinhs", columns: new[] { "DonViId", "ThuTu" });
        migrationBuilder.CreateIndex(name: "IX_DashboardWidgetCauHinhs_NguoiDungId_ThuTu", table: "DashboardWidgetCauHinhs", columns: new[] { "NguoiDungId", "ThuTu" });
        migrationBuilder.CreateIndex(name: "IX_DashboardWidgetCauHinhs_NhomQuyenId_ThuTu", table: "DashboardWidgetCauHinhs", columns: new[] { "NhomQuyenId", "ThuTu" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "DashboardWidgetCauHinhs");
        migrationBuilder.DropTable(name: "DashboardWidgets");
    }
}
