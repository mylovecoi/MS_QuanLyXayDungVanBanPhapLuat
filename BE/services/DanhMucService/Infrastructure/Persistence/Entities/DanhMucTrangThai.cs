using System.ComponentModel.DataAnnotations;

namespace DanhMucService.Infrastructure.Persistence.Entities
{
    public class DanhMucTrangThai : BaseEntity
    {
        [Required(ErrorMessage = "Ma trang thai khong duoc de trong")]
        public string MaTrangThai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ten trang thai khong duoc de trong")]
        public string TenTrangThai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ma mau hien thi khong duoc de trong")]
        public string MaMauHex { get; set; } = string.Empty;

        public int ThuTuSapXep { get; set; } = 1;

        public bool TrangThai { get; set; } = true;

        public string? MoTa { get; set; }

        public string? GhiChu { get; set; }
    }
}
