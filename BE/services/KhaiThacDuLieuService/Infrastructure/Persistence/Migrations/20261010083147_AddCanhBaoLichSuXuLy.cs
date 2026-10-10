using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaiThacDuLieuService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCanhBaoLichSuXuLy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CanhBaoLichSuXuLys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanhBaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhacViecId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HanhDong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NguoiThucHienId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonViThucHienId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrangThaiTruoc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThaiSau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanhBaoLichSuXuLys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanhBaoLichSuXuLys_CanhBaoKhaiThacDuLieus_CanhBaoId",
                        column: x => x.CanhBaoId,
                        principalTable: "CanhBaoKhaiThacDuLieus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CanhBaoLichSuXuLys_CanhBaoNhacViecs_NhacViecId",
                        column: x => x.NhacViecId,
                        principalTable: "CanhBaoNhacViecs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CanhBaoLichSuXuLys_CanhBaoId_ThoiGian",
                table: "CanhBaoLichSuXuLys",
                columns: new[] { "CanhBaoId", "ThoiGian" });

            migrationBuilder.CreateIndex(
                name: "IX_CanhBaoLichSuXuLys_NhacViecId_ThoiGian",
                table: "CanhBaoLichSuXuLys",
                columns: new[] { "NhacViecId", "ThoiGian" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CanhBaoLichSuXuLys");
        }
    }
}
