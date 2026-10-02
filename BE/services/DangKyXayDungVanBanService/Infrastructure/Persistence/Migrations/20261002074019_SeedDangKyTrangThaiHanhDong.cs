using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DangKyXayDungVanBanService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedDangKyTrangThaiHanhDong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DangKyHanhDongXuLys",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "LoaiHanhDong", "MaHanhDong", "MoTa", "TenHanhDong", "ThuTuSapXep", "TrangThai", "UpdatedAt", "UpdatedBy", "YeuCauFileDinhKem", "YeuCauLyDo" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222201"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Create", "TAO_MOI", null, "Tạo mới", 1, true, null, null, false, false },
                    { new Guid("22222222-2222-2222-2222-222222222202"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Update", "CAP_NHAT_HO_SO", null, "Cập nhật hồ sơ", 2, true, null, null, false, false },
                    { new Guid("22222222-2222-2222-2222-222222222203"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Forward", "TRINH_PHE_DUYET", null, "Trình phê duyệt", 3, true, null, null, false, false },
                    { new Guid("22222222-2222-2222-2222-222222222204"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Approve", "PHE_DUYET", null, "Phê duyệt", 4, true, null, null, false, false },
                    { new Guid("22222222-2222-2222-2222-222222222205"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Return", "TRA_LAI", null, "Trả lại", 5, true, null, null, false, true },
                    { new Guid("22222222-2222-2222-2222-222222222206"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Reject", "KHONG_PHE_DUYET", null, "Không phê duyệt", 6, true, null, null, false, true },
                    { new Guid("22222222-2222-2222-2222-222222222207"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Update", "CAP_NHAT_KET_QUA", null, "Cập nhật kết quả", 7, true, null, null, true, false },
                    { new Guid("22222222-2222-2222-2222-222222222208"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Complete", "HOAN_THANH", null, "Hoàn thành", 8, true, null, null, false, false },
                    { new Guid("22222222-2222-2222-2222-222222222209"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "StartNextWorkflow", "KHOI_TAO_QUY_TRINH_XAY_DUNG", null, "Khởi tạo quy trình xây dựng", 9, true, null, null, false, false }
                });

            migrationBuilder.InsertData(
                table: "DangKyTrangThaiHoSos",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "LaTrangThaiKetThuc", "MaTrangThai", "MauHienThi", "MoTa", "TenTrangThai", "ThuTuSapXep", "TrangThai", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, false, "MOI_TAO", "gray", null, "Mới tạo", 1, true, null, null },
                    { new Guid("11111111-1111-1111-1111-111111111102"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, false, "DANG_SOAN_THAO", "blue", null, "Đang soạn thảo", 2, true, null, null },
                    { new Guid("11111111-1111-1111-1111-111111111103"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, false, "DA_TRINH_PHE_DUYET", "orange", null, "Đã trình phê duyệt", 3, true, null, null },
                    { new Guid("11111111-1111-1111-1111-111111111104"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, false, "DA_PHE_DUYET", "green", null, "Đã phê duyệt", 4, true, null, null },
                    { new Guid("11111111-1111-1111-1111-111111111105"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, false, "BI_TRA_LAI", "red", null, "Bị trả lại", 5, true, null, null },
                    { new Guid("11111111-1111-1111-1111-111111111106"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, true, "KHONG_PHE_DUYET", "red", null, "Không phê duyệt", 6, true, null, null },
                    { new Guid("11111111-1111-1111-1111-111111111107"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, false, "DA_CAP_NHAT_KET_QUA", "cyan", null, "Đã cập nhật kết quả", 7, true, null, null },
                    { new Guid("11111111-1111-1111-1111-111111111108"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, true, "HOAN_THANH", "green", null, "Hoàn thành", 8, true, null, null },
                    { new Guid("11111111-1111-1111-1111-111111111109"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, true, "DA_CHUYEN_QUY_TRINH_XAY_DUNG", "purple", null, "Đã chuyển quy trình xây dựng", 9, true, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DangKyHanhDongXuLys",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222201"));

            migrationBuilder.DeleteData(
                table: "DangKyHanhDongXuLys",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222202"));

            migrationBuilder.DeleteData(
                table: "DangKyHanhDongXuLys",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222203"));

            migrationBuilder.DeleteData(
                table: "DangKyHanhDongXuLys",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222204"));

            migrationBuilder.DeleteData(
                table: "DangKyHanhDongXuLys",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222205"));

            migrationBuilder.DeleteData(
                table: "DangKyHanhDongXuLys",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222206"));

            migrationBuilder.DeleteData(
                table: "DangKyHanhDongXuLys",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222207"));

            migrationBuilder.DeleteData(
                table: "DangKyHanhDongXuLys",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222208"));

            migrationBuilder.DeleteData(
                table: "DangKyHanhDongXuLys",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222209"));

            migrationBuilder.DeleteData(
                table: "DangKyTrangThaiHoSos",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111101"));

            migrationBuilder.DeleteData(
                table: "DangKyTrangThaiHoSos",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111102"));

            migrationBuilder.DeleteData(
                table: "DangKyTrangThaiHoSos",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111103"));

            migrationBuilder.DeleteData(
                table: "DangKyTrangThaiHoSos",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111104"));

            migrationBuilder.DeleteData(
                table: "DangKyTrangThaiHoSos",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111105"));

            migrationBuilder.DeleteData(
                table: "DangKyTrangThaiHoSos",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111106"));

            migrationBuilder.DeleteData(
                table: "DangKyTrangThaiHoSos",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111107"));

            migrationBuilder.DeleteData(
                table: "DangKyTrangThaiHoSos",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111108"));

            migrationBuilder.DeleteData(
                table: "DangKyTrangThaiHoSos",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111109"));
        }
    }
}
