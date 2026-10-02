using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DangKyXayDungVanBanService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedDangKyCauHinhChuyenTrangThai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DangKyCauHinhChuyenTrangThais",
                columns: new[] { "Id", "BuocHienTaiId", "BuocTiepTheoId", "ChuyenBuocId", "CreatedAt", "CreatedBy", "HanhDongId", "IsDeleted", "LaKetThuc", "NhomNhapLieu", "QuyTrinhSoanThaoId", "TrangThai", "TrangThaiHienTaiId", "TrangThaiTiepTheoId", "UpdatedAt", "UpdatedBy", "YeuCauFileDinhKem", "YeuCauLyDo" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444401"), new Guid("33333333-3333-3333-3333-333333333311"), new Guid("33333333-3333-3333-3333-333333333311"), null, new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("22222222-2222-2222-2222-222222222202"), false, false, "DonViSoanThao", new Guid("33333333-3333-3333-3333-333333333301"), true, new Guid("11111111-1111-1111-1111-111111111102"), new Guid("11111111-1111-1111-1111-111111111102"), null, null, false, false },
                    { new Guid("44444444-4444-4444-4444-444444444402"), new Guid("33333333-3333-3333-3333-333333333311"), new Guid("33333333-3333-3333-3333-333333333311"), null, new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("22222222-2222-2222-2222-222222222202"), false, false, "DonViSoanThao", new Guid("33333333-3333-3333-3333-333333333301"), true, new Guid("11111111-1111-1111-1111-111111111105"), new Guid("11111111-1111-1111-1111-111111111102"), null, null, false, false },
                    { new Guid("44444444-4444-4444-4444-444444444403"), new Guid("33333333-3333-3333-3333-333333333311"), new Guid("33333333-3333-3333-3333-333333333312"), new Guid("33333333-3333-3333-3333-333333333321"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("22222222-2222-2222-2222-222222222203"), false, false, "DonViSoanThao", new Guid("33333333-3333-3333-3333-333333333301"), true, new Guid("11111111-1111-1111-1111-111111111102"), new Guid("11111111-1111-1111-1111-111111111103"), null, null, false, false },
                    { new Guid("44444444-4444-4444-4444-444444444404"), new Guid("33333333-3333-3333-3333-333333333311"), new Guid("33333333-3333-3333-3333-333333333312"), new Guid("33333333-3333-3333-3333-333333333321"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("22222222-2222-2222-2222-222222222203"), false, false, "DonViSoanThao", new Guid("33333333-3333-3333-3333-333333333301"), true, new Guid("11111111-1111-1111-1111-111111111105"), new Guid("11111111-1111-1111-1111-111111111103"), null, null, false, false },
                    { new Guid("44444444-4444-4444-4444-444444444405"), new Guid("33333333-3333-3333-3333-333333333312"), new Guid("33333333-3333-3333-3333-333333333313"), new Guid("33333333-3333-3333-3333-333333333322"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("22222222-2222-2222-2222-222222222204"), false, false, "DonViPheDuyet", new Guid("33333333-3333-3333-3333-333333333301"), true, new Guid("11111111-1111-1111-1111-111111111103"), new Guid("11111111-1111-1111-1111-111111111104"), null, null, false, false },
                    { new Guid("44444444-4444-4444-4444-444444444406"), new Guid("33333333-3333-3333-3333-333333333312"), new Guid("33333333-3333-3333-3333-333333333311"), new Guid("33333333-3333-3333-3333-333333333324"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("22222222-2222-2222-2222-222222222205"), false, false, "DonViPheDuyet", new Guid("33333333-3333-3333-3333-333333333301"), true, new Guid("11111111-1111-1111-1111-111111111103"), new Guid("11111111-1111-1111-1111-111111111105"), null, null, false, true },
                    { new Guid("44444444-4444-4444-4444-444444444407"), new Guid("33333333-3333-3333-3333-333333333312"), new Guid("33333333-3333-3333-3333-333333333315"), new Guid("33333333-3333-3333-3333-333333333325"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("22222222-2222-2222-2222-222222222206"), false, true, "DonViPheDuyet", new Guid("33333333-3333-3333-3333-333333333301"), true, new Guid("11111111-1111-1111-1111-111111111103"), new Guid("11111111-1111-1111-1111-111111111106"), null, null, false, true },
                    { new Guid("44444444-4444-4444-4444-444444444408"), new Guid("33333333-3333-3333-3333-333333333313"), new Guid("33333333-3333-3333-3333-333333333314"), new Guid("33333333-3333-3333-3333-333333333323"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("22222222-2222-2222-2222-222222222207"), false, false, "DonViSoanThao", new Guid("33333333-3333-3333-3333-333333333301"), true, new Guid("11111111-1111-1111-1111-111111111104"), new Guid("11111111-1111-1111-1111-111111111107"), null, null, true, false },
                    { new Guid("44444444-4444-4444-4444-444444444409"), new Guid("33333333-3333-3333-3333-333333333314"), new Guid("33333333-3333-3333-3333-333333333315"), new Guid("33333333-3333-3333-3333-333333333326"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("22222222-2222-2222-2222-222222222208"), false, true, "DonViSoanThao", new Guid("33333333-3333-3333-3333-333333333301"), true, new Guid("11111111-1111-1111-1111-111111111107"), new Guid("11111111-1111-1111-1111-111111111108"), null, null, false, false },
                    { new Guid("44444444-4444-4444-4444-444444444410"), new Guid("33333333-3333-3333-3333-333333333315"), new Guid("33333333-3333-3333-3333-333333333315"), null, new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("22222222-2222-2222-2222-222222222209"), false, true, "DonViSoanThao", new Guid("33333333-3333-3333-3333-333333333301"), true, new Guid("11111111-1111-1111-1111-111111111108"), new Guid("11111111-1111-1111-1111-111111111109"), null, null, false, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DangKyCauHinhChuyenTrangThais",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444401"));

            migrationBuilder.DeleteData(
                table: "DangKyCauHinhChuyenTrangThais",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444402"));

            migrationBuilder.DeleteData(
                table: "DangKyCauHinhChuyenTrangThais",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444403"));

            migrationBuilder.DeleteData(
                table: "DangKyCauHinhChuyenTrangThais",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444404"));

            migrationBuilder.DeleteData(
                table: "DangKyCauHinhChuyenTrangThais",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444405"));

            migrationBuilder.DeleteData(
                table: "DangKyCauHinhChuyenTrangThais",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444406"));

            migrationBuilder.DeleteData(
                table: "DangKyCauHinhChuyenTrangThais",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444407"));

            migrationBuilder.DeleteData(
                table: "DangKyCauHinhChuyenTrangThais",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444408"));

            migrationBuilder.DeleteData(
                table: "DangKyCauHinhChuyenTrangThais",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444409"));

            migrationBuilder.DeleteData(
                table: "DangKyCauHinhChuyenTrangThais",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444410"));
        }
    }
}
