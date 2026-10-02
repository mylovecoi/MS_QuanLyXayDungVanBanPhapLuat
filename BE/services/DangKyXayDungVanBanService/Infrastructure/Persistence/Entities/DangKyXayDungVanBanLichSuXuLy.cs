using System.ComponentModel.DataAnnotations;

namespace DangKyXayDungVanBanService.Infrastructure.Persistence.Entities;

public class DangKyXayDungVanBanLichSuXuLy : BaseEntity
{
    public Guid DangKyXayDungVanBanId { get; set; }
    public Guid? TuBuocId { get; set; }
    public Guid? DenBuocId { get; set; }
    public Guid? ChuyenBuocId { get; set; }

    [MaxLength(255)]
    public string? TenBuocTu { get; set; }

    [MaxLength(255)]
    public string? TenBuocDen { get; set; }

    public Guid HanhDongId { get; set; }

    [Required]
    [MaxLength(100)]
    public string MaHanhDongSnapshot { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string TenHanhDongSnapshot { get; set; } = string.Empty;

    public Guid? TrangThaiTruocId { get; set; }
    public Guid? TrangThaiSauId { get; set; }

    [MaxLength(255)]
    public string? TenTrangThaiTruocSnapshot { get; set; }

    [MaxLength(255)]
    public string? TenTrangThaiSauSnapshot { get; set; }

    public string? NoiDungXuLy { get; set; }
    public string? LyDoTraLai { get; set; }
    public Guid NguoiXuLyId { get; set; }

    [Required]
    [MaxLength(255)]
    public string TenNguoiXuLy { get; set; } = string.Empty;

    public Guid DonViXuLyId { get; set; }

    [Required]
    [MaxLength(255)]
    public string TenDonViXuLy { get; set; } = string.Empty;

    public DateTime NgayXuLy { get; set; } = DateTime.UtcNow;

    public DangKyXayDungVanBan? DangKyXayDungVanBan { get; set; }
}
