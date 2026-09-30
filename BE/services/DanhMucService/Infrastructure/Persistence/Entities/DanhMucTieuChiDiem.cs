using System.ComponentModel.DataAnnotations;

namespace DanhMucService.Infrastructure.Persistence.Entities
{
    public class DanhMucTieuChiDiem : BaseEntity
    {
        [Required(ErrorMessage = "Ma tieu chi khong duoc de trong")]
        [StringLength(100)]
        public string MaTieuChi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ten tieu chi khong duoc de trong")]
        [StringLength(250)]
        public string TenTieuChi { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LoaiTieuChi { get; set; } = "THOI_GIAN";

        [Required]
        [StringLength(50)]
        public string KieuGiaTri { get; set; } = "TY_LE";

        [Required]
        [StringLength(50)]
        public string DonViGiaTri { get; set; } = "PERCENT";

        public int ThuTuSapXep { get; set; } = 1;

        public decimal DiemToiDa { get; set; } = 0;

        public bool TrangThai { get; set; } = true;

        public string? MoTa { get; set; }

        public string? GhiChu { get; set; }
    }
}
