using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DanhMucService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedDangKyXayDungQpplWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "dm",
                table: "DanhMucQuyTrinhSoanThaos",
                columns: new[] { "Id", "CapApDung", "CreatedBy", "CreatedDate", "DanhMucVanBanId", "DanhMucVanBanIds", "GhiChu", "LoaiQuyTrinh", "MaQuyTrinh", "MoTa", "PhienBan", "TenQuyTrinh", "TrangThai", "UpdatedBy", "UpdatedDate" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333301"), "CapTinh", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), null, null, null, "DangKyXayDungQPPL", "DXDM_DKXD_QPPL", "Quy trình đăng ký xây dựng văn bản QPPL cấp tỉnh", 1, "Đề xuất danh mục / Đăng ký xây dựng QPPL", true, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.InsertData(
                schema: "dm",
                table: "DanhMucBuocQuyTrinhs",
                columns: new[] { "Id", "BatBuoc", "CachHoanThanh", "ChoPhepBoQua", "ChoPhepQuayLui", "CreatedBy", "CreatedDate", "DonViTiepNhanMacDinhId", "GhiChu", "LoaiBuoc", "MaBuoc", "MoTa", "QuyTrinhSoanThaoId", "SoLanTraLaiToiDa", "SoLuongPhanHoiToiThieu", "SoNgayCanhBaoSapHan", "SoNgayXuLyTieuChuan", "TenBuoc", "ThuTuSapXep", "UpdatedBy", "UpdatedDate", "YeuCauFileDinhKem" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333311"), true, null, false, false, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), null, null, "NhapLieu", "LAP_HO_SO", "Đơn vị soạn thảo lập hồ sơ đăng ký trên phần mềm", new Guid("33333333-3333-3333-3333-333333333301"), 0, null, null, null, "Lập hồ sơ đề nghị/đăng ký", 1, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), false },
                    { new Guid("33333333-3333-3333-3333-333333333312"), true, null, false, false, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), null, null, "XuLy", "TRINH_HO_SO", "Hồ sơ đã trình sang đơn vị phê duyệt", new Guid("33333333-3333-3333-3333-333333333301"), 0, null, null, null, "Trình hồ sơ/Chờ phê duyệt", 2, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), false },
                    { new Guid("33333333-3333-3333-3333-333333333313"), true, null, false, false, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), null, null, "PheDuyet", "PHE_DUYET", "Đơn vị phê duyệt nhập kết quả phê duyệt hoặc trả lại", new Guid("33333333-3333-3333-3333-333333333301"), 0, null, null, null, "Phê duyệt hồ sơ", 3, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), false },
                    { new Guid("33333333-3333-3333-3333-333333333314"), true, null, false, false, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), null, null, "NhapLieu", "CAP_NHAT_KET_QUA", "Đơn vị soạn thảo cập nhật số/ngày văn bản và file kết quả", new Guid("33333333-3333-3333-3333-333333333301"), 0, null, null, null, "Cập nhật kết quả phê duyệt", 4, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), true },
                    { new Guid("33333333-3333-3333-3333-333333333315"), true, null, false, false, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), null, null, "KetThuc", "HOAN_THANH", "Hoàn thành quy trình đăng ký xây dựng văn bản", new Guid("33333333-3333-3333-3333-333333333301"), 0, null, null, null, "Hoàn thành", 5, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), false }
                });

            migrationBuilder.InsertData(
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DenBuocId", "DieuKienKetQua", "GhiChu", "IsKetThuc", "LaNhanhMacDinh", "LoaiChuyenBuoc", "MoTa", "QuyTrinhSoanThaoId", "TuBuocId", "UpdatedBy", "UpdatedDate", "YeuCauNhapLyDo" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333321"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), new Guid("33333333-3333-3333-3333-333333333312"), "TRINH_PHE_DUYET", null, false, true, "Forward", null, new Guid("33333333-3333-3333-3333-333333333301"), new Guid("33333333-3333-3333-3333-333333333311"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), false },
                    { new Guid("33333333-3333-3333-3333-333333333322"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), new Guid("33333333-3333-3333-3333-333333333313"), "TIEP_NHAN_PHE_DUYET", null, false, true, "Forward", null, new Guid("33333333-3333-3333-3333-333333333301"), new Guid("33333333-3333-3333-3333-333333333312"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), false },
                    { new Guid("33333333-3333-3333-3333-333333333323"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), new Guid("33333333-3333-3333-3333-333333333314"), "PHE_DUYET", null, false, true, "Forward", null, new Guid("33333333-3333-3333-3333-333333333301"), new Guid("33333333-3333-3333-3333-333333333313"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), false },
                    { new Guid("33333333-3333-3333-3333-333333333324"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), new Guid("33333333-3333-3333-3333-333333333311"), "TRA_LAI", null, false, false, "Return", null, new Guid("33333333-3333-3333-3333-333333333301"), new Guid("33333333-3333-3333-3333-333333333313"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), true },
                    { new Guid("33333333-3333-3333-3333-333333333325"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), new Guid("33333333-3333-3333-3333-333333333315"), "KHONG_PHE_DUYET", null, true, false, "Reject", null, new Guid("33333333-3333-3333-3333-333333333301"), new Guid("33333333-3333-3333-3333-333333333313"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), true },
                    { new Guid("33333333-3333-3333-3333-333333333326"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), new Guid("33333333-3333-3333-3333-333333333315"), "HOAN_THANH", null, true, false, "Forward", null, new Guid("33333333-3333-3333-3333-333333333301"), new Guid("33333333-3333-3333-3333-333333333314"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Local), false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333321"));

            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333322"));

            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333323"));

            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333324"));

            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333325"));

            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333326"));

            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333311"));

            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333312"));

            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333313"));

            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333314"));

            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333315"));

            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucQuyTrinhSoanThaos",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333301"));
        }
    }
}
