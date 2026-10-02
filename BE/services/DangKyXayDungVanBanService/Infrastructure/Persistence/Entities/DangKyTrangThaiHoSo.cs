using System.ComponentModel.DataAnnotations;

namespace DangKyXayDungVanBanService.Infrastructure.Persistence.Entities;

public class DangKyTrangThaiHoSo : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string MaTrangThai { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string TenTrangThai { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? MoTa { get; set; }

    [MaxLength(50)]
    public string? MauHienThi { get; set; }

    public int ThuTuSapXep { get; set; }
    public bool LaTrangThaiKetThuc { get; set; }
    public bool TrangThai { get; set; } = true;
}
