using System.ComponentModel.DataAnnotations;

namespace DangKyXayDungVanBanService.Infrastructure.Persistence.Entities;

public class DangKyXayDungVanBanLienKetQuyTrinh : BaseEntity
{
    public Guid DangKyXayDungVanBanId { get; set; }
    public Guid HoSoXayDungVanBanId { get; set; }
    public Guid LoaiVanBanId { get; set; }
    public Guid QuyTrinhXayDungId { get; set; }

    [Required]
    [MaxLength(100)]
    public string MaQuyTrinhXayDung { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TenQuyTrinhXayDung { get; set; } = string.Empty;

    public DateTime NgayLienKet { get; set; } = DateTime.UtcNow;
}
