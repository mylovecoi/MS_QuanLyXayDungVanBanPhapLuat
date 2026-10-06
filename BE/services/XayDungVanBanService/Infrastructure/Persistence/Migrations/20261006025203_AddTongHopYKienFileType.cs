using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XayDungVanBanService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTongHopYKienFileType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LoaiTaiLieuTongHopYKienId",
                table: "HoSoXayDungVanBanSoanThaos",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiTaiLieuTongHopYKienId",
                table: "HoSoXayDungVanBanSoanThaos");
        }
    }
}
