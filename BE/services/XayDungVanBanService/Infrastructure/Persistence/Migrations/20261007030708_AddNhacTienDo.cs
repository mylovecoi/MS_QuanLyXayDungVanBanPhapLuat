using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XayDungVanBanService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNhacTienDo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoSoXayDungVanBanNhacTienDos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoSoXayDungVanBanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoaiNhacNho = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThaiXuLy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NoiDungNhacNho = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    NguoiGuiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViGuiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NguoiNhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonViNhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayXem = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhanHoi = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NgayPhanHoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiPhanHoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NgayXacNhanXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiXacNhanXuLyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GhiChuXuLy = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoXayDungVanBanNhacTienDos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoXayDungVanBanNhacTienDos_HoSoXayDungVanBans_HoSoXayDungVanBanId",
                        column: x => x.HoSoXayDungVanBanId,
                        principalTable: "HoSoXayDungVanBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanNhacTienDos_DonViNhanId_TrangThaiXuLy_NgayGui",
                table: "HoSoXayDungVanBanNhacTienDos",
                columns: new[] { "DonViNhanId", "TrangThaiXuLy", "NgayGui" });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanNhacTienDos_HoSoXayDungVanBanId_NgayGui",
                table: "HoSoXayDungVanBanNhacTienDos",
                columns: new[] { "HoSoXayDungVanBanId", "NgayGui" });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanNhacTienDos_NguoiNhanId_TrangThaiXuLy_NgayGui",
                table: "HoSoXayDungVanBanNhacTienDos",
                columns: new[] { "NguoiNhanId", "TrangThaiXuLy", "NgayGui" });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoXayDungVanBanNhacTienDos_TrangThaiXuLy_NgayGui",
                table: "HoSoXayDungVanBanNhacTienDos",
                columns: new[] { "TrangThaiXuLy", "NgayGui" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoSoXayDungVanBanNhacTienDos");
        }
    }
}
