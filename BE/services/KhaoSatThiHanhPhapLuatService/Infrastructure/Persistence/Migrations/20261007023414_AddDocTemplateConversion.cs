using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDocTemplateConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DuongDanFileChuyenDoi",
                schema: "kspl",
                table: "PhienDocMauPhieuKhaoSats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoiChuyenDoi",
                schema: "kspl",
                table: "PhienDocMauPhieuKhaoSats",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuongDanFileChuyenDoi",
                schema: "kspl",
                table: "PhienDocMauPhieuKhaoSats");

            migrationBuilder.DropColumn(
                name: "LoiChuyenDoi",
                schema: "kspl",
                table: "PhienDocMauPhieuKhaoSats");
        }
    }
}
