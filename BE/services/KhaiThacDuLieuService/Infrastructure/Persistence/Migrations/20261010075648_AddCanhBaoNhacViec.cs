using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaiThacDuLieuService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCanhBaoNhacViec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CanhBaoNhacViecs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanhBaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    NguoiGiaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViGiaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NguoiNhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DonViNhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HanXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianNhac = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MucDoUuTien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayXem = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayHoanThanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChuHoanThanh = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanhBaoNhacViecs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanhBaoNhacViecs_CanhBaoKhaiThacDuLieus_CanhBaoId",
                        column: x => x.CanhBaoId,
                        principalTable: "CanhBaoKhaiThacDuLieus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CanhBaoNhacViecs_CanhBaoId_TrangThai_HanXuLy",
                table: "CanhBaoNhacViecs",
                columns: new[] { "CanhBaoId", "TrangThai", "HanXuLy" });

            migrationBuilder.CreateIndex(
                name: "IX_CanhBaoNhacViecs_DonViNhanId_TrangThai_HanXuLy",
                table: "CanhBaoNhacViecs",
                columns: new[] { "DonViNhanId", "TrangThai", "HanXuLy" });

            migrationBuilder.CreateIndex(
                name: "IX_CanhBaoNhacViecs_NguoiNhanId_TrangThai_HanXuLy",
                table: "CanhBaoNhacViecs",
                columns: new[] { "NguoiNhanId", "TrangThai", "HanXuLy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CanhBaoNhacViecs");
        }
    }
}
