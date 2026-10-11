using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanhMucService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedExpandedXayDungVanBanScoringCriteria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var seedDate = new System.DateTime(2026, 10, 11, 0, 0, 0, System.DateTimeKind.Unspecified);
            var seedUser = System.Guid.Empty;

            var dayDuTaiLieuId = new System.Guid("11111111-1111-1111-1111-111111111103");
            var tuanThuQuyTrinhId = new System.Guid("11111111-1111-1111-1111-111111111104");
            var ketQuaXuLyId = new System.Guid("11111111-1111-1111-1111-111111111105");

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
                        dayDuTaiLieuId, "DAY_DU_TAI_LIEU", "Đầy đủ thành phần tài liệu", "THANH_PHAN_TAI_LIEU",
                        "SO_LUONG", "FILE", 3, 20m, true,
                        "Chấm điểm theo số tài liệu bắt buộc còn thiếu khi trình xử lý.", "Áp dụng khi hệ thống đã cấu hình danh mục tài liệu bắt buộc.",
                        seedUser, seedDate, seedUser, seedDate
                    },
                    {
                        tuanThuQuyTrinhId, "TUAN_THU_QUY_TRINH", "Tuân thủ quy trình xử lý", "TUAN_THU_QUY_TRINH",
                        "SO_LAN", "LAN", 4, 20m, true,
                        "Chấm điểm theo số lần chuyển sai bước, thiếu điều kiện hoặc phải trả lại do lỗi nghiệp vụ.", null,
                        seedUser, seedDate, seedUser, seedDate
                    },
                    {
                        ketQuaXuLyId, "KET_QUA_XU_LY", "Kết quả xử lý cuối cùng", "KET_QUA_XU_LY",
                        "DIEM_DANH_GIA", "DIEM", 5, 20m, true,
                        "Chấm điểm theo kết quả cuối cùng của hồ sơ sau khi hoàn thành quy trình.", null,
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
                    { new System.Guid("11111111-1111-1111-1111-111111111401"), dayDuTaiLieuId, 0m, 0m, true, true, 20m, "Không thiếu tài liệu bắt buộc", 1, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111402"), dayDuTaiLieuId, 1m, 1m, true, true, 15m, "Thiếu 1 tài liệu bắt buộc", 2, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111403"), dayDuTaiLieuId, 2m, 2m, true, true, 10m, "Thiếu 2 tài liệu bắt buộc", 3, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111404"), dayDuTaiLieuId, 3m, null, true, true, 5m, "Thiếu từ 3 tài liệu bắt buộc", 4, true, null, seedUser, seedDate, seedUser, seedDate },

                    { new System.Guid("11111111-1111-1111-1111-111111111501"), tuanThuQuyTrinhId, 0m, 0m, true, true, 20m, "Không có lỗi quy trình", 1, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111502"), tuanThuQuyTrinhId, 1m, 1m, true, true, 15m, "Có 1 lỗi quy trình", 2, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111503"), tuanThuQuyTrinhId, 2m, 2m, true, true, 10m, "Có 2 lỗi quy trình", 3, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111504"), tuanThuQuyTrinhId, 3m, null, true, true, 5m, "Có từ 3 lỗi quy trình", 4, true, null, seedUser, seedDate, seedUser, seedDate },

                    { new System.Guid("11111111-1111-1111-1111-111111111601"), ketQuaXuLyId, 100m, 100m, true, true, 20m, "Hoàn thành và ban hành đúng quy định", 1, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111602"), ketQuaXuLyId, 80m, 99m, true, true, 15m, "Hoàn thành nhưng có nội dung phải chỉnh sửa sau thẩm tra/thẩm định", 2, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111603"), ketQuaXuLyId, 50m, 79m, true, true, 10m, "Hoàn thành sau nhiều lần trả lại", 3, true, null, seedUser, seedDate, seedUser, seedDate },
                    { new System.Guid("11111111-1111-1111-1111-111111111604"), ketQuaXuLyId, 0m, 49m, true, true, 5m, "Không hoàn thành hoặc không đủ điều kiện ban hành", 4, true, null, seedUser, seedDate, seedUser, seedDate }
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
                    new System.Guid("11111111-1111-1111-1111-111111111401"),
                    new System.Guid("11111111-1111-1111-1111-111111111402"),
                    new System.Guid("11111111-1111-1111-1111-111111111403"),
                    new System.Guid("11111111-1111-1111-1111-111111111404"),
                    new System.Guid("11111111-1111-1111-1111-111111111501"),
                    new System.Guid("11111111-1111-1111-1111-111111111502"),
                    new System.Guid("11111111-1111-1111-1111-111111111503"),
                    new System.Guid("11111111-1111-1111-1111-111111111504"),
                    new System.Guid("11111111-1111-1111-1111-111111111601"),
                    new System.Guid("11111111-1111-1111-1111-111111111602"),
                    new System.Guid("11111111-1111-1111-1111-111111111603"),
                    new System.Guid("11111111-1111-1111-1111-111111111604")
                });

            migrationBuilder.DeleteData(
                schema: "dm",
                table: "DanhMucTieuChiDiems",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    new System.Guid("11111111-1111-1111-1111-111111111103"),
                    new System.Guid("11111111-1111-1111-1111-111111111104"),
                    new System.Guid("11111111-1111-1111-1111-111111111105")
                });
        }
    }
}
