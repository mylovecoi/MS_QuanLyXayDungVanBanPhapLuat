using System.ComponentModel.DataAnnotations;

namespace DangKyXayDungVanBanService.Infrastructure.Persistence.Entities;

public class DangKyXayDungVanBanFile : BaseEntity
{
    public Guid DangKyXayDungVanBanId { get; set; }

    [Required]
    [MaxLength(100)]
    public string LoaiFile { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TenFile { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string DuongDanFile { get; set; } = string.Empty;

    public long DungLuong { get; set; }

    [MaxLength(255)]
    public string? MimeType { get; set; }

    [MaxLength(500)]
    public string? MoTa { get; set; }

    public DangKyXayDungVanBan? DangKyXayDungVanBan { get; set; }
}
