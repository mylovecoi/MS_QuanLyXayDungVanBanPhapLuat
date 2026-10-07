using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddKetQuaTongHopFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SoLuong",
                schema: "kspl",
                table: "CauTraLoiKhaoSats",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TongSoTraLoi",
                schema: "kspl",
                table: "CauTraLoiKhaoSats",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoLuong",
                schema: "kspl",
                table: "CauTraLoiKhaoSats");

            migrationBuilder.DropColumn(
                name: "TongSoTraLoi",
                schema: "kspl",
                table: "CauTraLoiKhaoSats");
        }
    }
}
