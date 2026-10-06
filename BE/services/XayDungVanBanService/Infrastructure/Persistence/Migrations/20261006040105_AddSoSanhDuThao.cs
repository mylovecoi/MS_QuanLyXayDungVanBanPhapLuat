using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XayDungVanBanService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSoSanhDuThao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanSoSanhDuThaos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileGocId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileSoSanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoNoiDungThem = table.Column<int>(type: "int", nullable: false),
                    SoNoiDungXoa = table.Column<int>(type: "int", nullable: false),
                    SoNoiDungSua = table.Column<int>(type: "int", nullable: false),
                    NoiDungSoSanhHtml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanSoSanhDuThaos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanSoSanhDuThaos_FileGocId_FileSoSanhId",
                table: "HoSoXayDungVanBanSoSanhDuThaos",
                columns: new[] { "FileGocId", "FileSoSanhId" });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanSoSanhDuThaos_HoSoXayDungVanBanId_CreatedAt",
                table: "HoSoXayDungVanBanSoSanhDuThaos",
                columns: new[] { "HoSoXayDungVanBanId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanSoSanhDuThaos");
        }
    }
}
