using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThiHanhPhapLuatService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTongHopLanChot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BaoCaoTongHopChiTiets_BaoCaoTongHopThiHanhId_NoiDungKeHoachId",
                schema: "thpl",
                table: "BaoCaoTongHopChiTiets");

            migrationBuilder.AddColumn<int>(
                name: "LanChot",
                schema: "thpl",
                table: "BaoCaoTongHopChiTiets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BaoCaoTongHopChiTiets_BaoCaoTongHopThiHanhId_NoiDungKeHoachId_LanChot",
                schema: "thpl",
                table: "BaoCaoTongHopChiTiets",
                columns: new[] { "BaoCaoTongHopThiHanhId", "NoiDungKeHoachId", "LanChot" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BaoCaoTongHopChiTiets_BaoCaoTongHopThiHanhId_NoiDungKeHoachId_LanChot",
                schema: "thpl",
                table: "BaoCaoTongHopChiTiets");

            migrationBuilder.DropColumn(
                name: "LanChot",
                schema: "thpl",
                table: "BaoCaoTongHopChiTiets");

            migrationBuilder.CreateIndex(
                name: "IX_BaoCaoTongHopChiTiets_BaoCaoTongHopThiHanhId_NoiDungKeHoachId",
                schema: "thpl",
                table: "BaoCaoTongHopChiTiets",
                columns: new[] { "BaoCaoTongHopThiHanhId", "NoiDungKeHoachId" },
                unique: true);
        }
    }
}
