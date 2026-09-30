using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanTriHeThongService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemInfoMenuLayout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MenuLayout",
                schema: "qtht",
                table: "SystemInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "vertical");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuLayout",
                schema: "qtht",
                table: "SystemInfo");
        }
    }
}
