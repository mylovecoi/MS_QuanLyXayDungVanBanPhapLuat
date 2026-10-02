using System.ComponentModel.DataAnnotations;

namespace DangKyXayDungVanBanService.Infrastructure.Persistence.Entities;

public class DangKyHanhDongXuLy : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string MaHanhDong { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string TenHanhDong { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? MoTa { get; set; }

    [Required]
    [MaxLength(100)]
    public string LoaiHanhDong { get; set; } = string.Empty;

    public bool YeuCauLyDo { get; set; }
    public bool YeuCauFileDinhKem { get; set; }
    public int ThuTuSapXep { get; set; }
    public bool TrangThai { get; set; } = true;
}
