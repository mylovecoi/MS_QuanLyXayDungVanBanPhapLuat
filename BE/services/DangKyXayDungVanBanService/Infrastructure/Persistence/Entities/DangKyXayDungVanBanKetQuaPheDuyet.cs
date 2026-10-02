using System.ComponentModel.DataAnnotations;

namespace DangKyXayDungVanBanService.Infrastructure.Persistence.Entities;

public class DangKyXayDungVanBanKetQuaPheDuyet : BaseEntity
{
    public Guid DangKyXayDungVanBanId { get; set; }

    [Required]
    [MaxLength(100)]
    public string KetQua { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? SoVanBan { get; set; }

    public DateTime? NgayVanBan { get; set; }
    public Guid CoQuanPheDuyetId { get; set; }

    [Required]
    [MaxLength(255)]
    public string TenCoQuanPheDuyet { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? NguoiKy { get; set; }

    [MaxLength(255)]
    public string? ChucVuNguoiKy { get; set; }

    public string? NoiDungKetQua { get; set; }
    public Guid? FileKetQuaId { get; set; }

    public DangKyXayDungVanBan? DangKyXayDungVanBan { get; set; }
}
