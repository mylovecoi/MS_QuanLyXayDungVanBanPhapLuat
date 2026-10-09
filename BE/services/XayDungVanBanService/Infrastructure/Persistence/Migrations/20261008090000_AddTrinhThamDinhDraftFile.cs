using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XayDungVanBanService.Infrastructure.Persistence.Migrations;

public partial class AddTrinhThamDinhDraftFile : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "FileDuThaoId",
            table: "HoSoXayDungVanBanTrinhThamDinhs",
            type: "uniqueidentifier",
            nullable: true);
        migrationBuilder.CreateIndex(
            name: "IX_HoSoXayDungVanBanTrinhThamDinhs_FileDuThaoId",
            table: "HoSoXayDungVanBanTrinhThamDinhs",
            column: "FileDuThaoId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_HoSoXayDungVanBanTrinhThamDinhs_FileDuThaoId", table: "HoSoXayDungVanBanTrinhThamDinhs");
        migrationBuilder.DropColumn(name: "FileDuThaoId", table: "HoSoXayDungVanBanTrinhThamDinhs");
    }
}
