using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DangKyXayDungVanBanService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDangKyListIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBans_CreatedBy_CreatedAt",
                table: "DangKyXayDungVanBans",
                columns: new[] { "CreatedBy", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBans_DonViPheDuyetId_TrangThaiHoSoId_CreatedAt",
                table: "DangKyXayDungVanBans",
                columns: new[] { "DonViPheDuyetId", "TrangThaiHoSoId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBans_DonViSoanThaoId_TrangThaiHoSoId_CreatedAt",
                table: "DangKyXayDungVanBans",
                columns: new[] { "DonViSoanThaoId", "TrangThaiHoSoId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBans_LoaiVanBanId",
                table: "DangKyXayDungVanBans",
                column: "LoaiVanBanId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyXayDungVanBans_NamDangKy",
                table: "DangKyXayDungVanBans",
                column: "NamDangKy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DangKyXayDungVanBans_CreatedBy_CreatedAt",
                table: "DangKyXayDungVanBans");

            migrationBuilder.DropIndex(
                name: "IX_DangKyXayDungVanBans_DonViPheDuyetId_TrangThaiHoSoId_CreatedAt",
                table: "DangKyXayDungVanBans");

            migrationBuilder.DropIndex(
                name: "IX_DangKyXayDungVanBans_DonViSoanThaoId_TrangThaiHoSoId_CreatedAt",
                table: "DangKyXayDungVanBans");

            migrationBuilder.DropIndex(
                name: "IX_DangKyXayDungVanBans_LoaiVanBanId",
                table: "DangKyXayDungVanBans");

            migrationBuilder.DropIndex(
                name: "IX_DangKyXayDungVanBans_NamDangKy",
                table: "DangKyXayDungVanBans");
        }
    }
}
