using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanhMucService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedXayDungVanBanScoringCriteria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var seedDate = new System.DateTime(2026, 10, 6, 0, 0, 0, System.DateTimeKind.Unspecified);
            var seedUser = System.Guid.Empty;
            var chatLuongId = new System.Guid("11111111-1111-1111-1111-111111111101");
            var tienDoId = new System.Guid("11111111-1111-1111-1111-111111111102");

            migrationBuilder.InsertData(
                schema: "dm",
                table: "DanhMucTieuChiDiems",
                columns: new[]
                {
                    "Id", "MaTieuChi", "TenTieuChi", "LoaiTieuChi", "KieuGiaTri", "DonViGiaTri",
                    "ThuTuSapXep", "DiemToiDa", "TrangThai", "MoTa", "GhiChu",
                    "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate"
                },
                values: new object[,]
                {
                    {
                        chatLuongId, "CHAT_LUONG_SOAN_THAO", "Chất lượng soạn thảo", "CHAT_LUONG_SOAN_THAO",
                        "SO_LAN_GUI_THAM_DINH", "LAN", 1, 60m, true,
                        "Chấm điểm theo số lần gửi hồ sơ thẩm định.", null,
                        seedUser, seedDate, seedUser, seedDate
                    },
                    {
                        tienDoId, "TIEN_DO_SOAN_THAO", "Tiến độ soạn thảo", "THOI_GIAN_SOAN_THAO",
                        "TY_LE_THOI_GIAN_THUC_TE", "PERCENT", 2, 40m, true,
                        "Chấm điểm theo tỷ lệ thời gian soạn thảo thực tế so với kế hoạch trên hồ sơ.", null,
                        seedUser, seedDate, seedUser, seedDate
                    }
                });

            migrationBuilder.InsertData(
                schema: "dm",
                table: "DanhMucTieuChiDiemMucs",
                columns: new[]
                {
                    "Id", "DanhMucTieuChiDiemId", "TuGiaTri", "DenGiaTri", "BaoGomTuGiaTri", "BaoGomDenGiaTri",
                    "Diem", "NhanHienThi", "ThuTuSapXep", "TrangThai", "GhiChu",
                    "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate"
                },
                values: new object[,]
                {
                    { new System.Guid("11111111-1111-1111-1111-111111111201"), chatLuongId, 1m, 1m, true, true, 60m, "Gửi thẩm định lần 1", 1, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111202"), chatLuongId, 2m, 2m, true, true, 45m, "Gửi thẩm định lần 2", 2, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111203"), chatLuongId, 3m, 3m, true, true, 30m, "Gửi thẩm định lần 3", 3, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111204"), chatLuongId, 4m, null, true, true, 15m, "Gửi thẩm định từ lần 4", 4, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111301"), tienDoId, null, 100m, true, true, 40m, "Hoàn thành không quá 100% thời gian kế hoạch", 1, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111302"), tienDoId, 100m, 120m, false, true, 30m, "Hoàn thành trên 100% đến 120% thời gian kế hoạch", 2, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111303"), tienDoId, 120m, 150m, false, true, 20m, "Hoàn thành trên 120% đến 150% thời gian kế hoạch", 3, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111304"), tienDoId, 150m, null, false, true, 10m, "Hoàn thành trên 150% thời gian kế hoạch", 4, true, null, seedUser, seedDate, seedUser, seedDate }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucTieuChiDiemMucs",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    new System.Guid("11111111-1111-1111-1111-111111111201"),
                    new System.Guid("11111111-1111-1111-1111-111111111202"),
                    new System.Guid("11111111-1111-1111-1111-111111111203"),
                    new System.Guid("11111111-1111-1111-1111-111111111204"),
                    new System.Guid("11111111-1111-1111-1111-111111111301"),
                    new System.Guid("11111111-1111-1111-1111-111111111302"),
                    new System.Guid("11111111-1111-1111-1111-111111111303"),
                    new System.Guid("11111111-1111-1111-1111-111111111304")
                });

            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucTieuChiDiems",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    new System.Guid("11111111-1111-1111-1111-111111111101"),
                    new System.Guid("11111111-1111-1111-1111-111111111102")
                });
        }
    }
}
