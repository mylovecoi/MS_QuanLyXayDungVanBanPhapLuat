using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanhMucService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusGroupForScoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NhomTrangThai",
                schema: "dm",
                table: "DanhMucTrangThais",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                schema: "dm",
                table: "DanhMucTrangThais",
                columns: new[] { "Id", "NhomTrangThai", "MaTrangThai", "TenTrangThai", "MaMauHex", "ThuTuSapXep", "TrangThai", "MoTa", "GhiChu", "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new System.Guid("22222222-2222-2222-2222-222222222201"), "CHAM_DIEM_HO_SO", "NHAP", "Nháp", "#6B7280", 1, true, "Bảng điểm đang được tạo hoặc tính lại.", null, System.Guid.Empty, new System.DateTime(2026, 10, 6), System.Guid.Empty, new System.DateTime(2026, 10, 6) },
                    { new System.Guid("22222222-2222-2222-2222-222222222202"), "CHAM_DIEM_HO_SO", "DA_CHOT", "Đã chốt", "#16A34A", 2, true, "Kết quả chấm điểm chính thức.", null, System.Guid.Empty, new System.DateTime(2026, 10, 6), System.Guid.Empty, new System.DateTime(2026, 10, 6) },
                    { new System.Guid("22222222-2222-2222-2222-222222222203"), "CHAM_DIEM_HO_SO", "DA_HUY", "Đã hủy", "#DC2626", 3, true, "Bảng điểm đã bị hủy sau khi chốt.", null, System.Guid.Empty, new System.DateTime(2026, 10, 6), System.Guid.Empty, new System.DateTime(2026, 10, 6) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucTrangThais",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    new System.Guid("22222222-2222-2222-2222-222222222201"),
                    new System.Guid("22222222-2222-2222-2222-222222222202"),
                    new System.Guid("22222222-2222-2222-2222-222222222203")
                });

            migrationBuilder.DropColumn(
                name: "NhomTrangThai",
                schema: "dm",
                table: "DanhMucTrangThais");
        }
    }
}
