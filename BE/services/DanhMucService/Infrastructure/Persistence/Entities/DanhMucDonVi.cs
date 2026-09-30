using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanhMucService.Infrastructure.Persistence.Entities
{
    public class DanhMucDonVi : BaseEntity
    {
        public required string TenDonVi { get; set; }
        public int Level { get; set; } = 0;
        public int STTSapXep { get; set; } = 0;
        public Guid DonViChuQuanId { get; set; }
        public string? DiaChi { get; set; }
        public string? MaQHNS { get; set; }
        public string? SoDienThoai { get; set; }
        public string? ChucDanhQuanLy { get; set; }
        public string? HoVaTenNguoiQuanLy { get; set; }
        public string? PhanLoaiDonVi { get; set; }
        public bool TinhNangThanhToan { get; set; } = true;
        [NotMapped]
        public string? TenDonViChuQuan { get; set; }
        public ICollection<DanhMucPhongBan> DanhMucPhongBans { get; set; } = [];
    }
}
