using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XayDungVanBanService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddYKienDonVi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanYKienDonVis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViGopYId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayNhan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KetQua = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NoiDungYKien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanYKienDonVis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanYKienDonVis_HoSoXayDungVanBans_HoSoXayDungVanBanId",
                        column: x => x.HoSoXayDungVanBanId,
                        principalTable: "HoSoXayDungVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanYKienDonVis_HoSoXayDungVanBanId_DonViGopYId",
                table: "HoSoXayDungVanBanYKienDonVis",
                columns: new[] { "HoSoXayDungVanBanId", "DonViGopYId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanYKienDonVis_HoSoXayDungVanBanId_NgayNhan",
                table: "HoSoXayDungVanBanYKienDonVis",
                columns: new[] { "HoSoXayDungVanBanId", "NgayNhan" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanYKienDonVis");
        }
    }
}
