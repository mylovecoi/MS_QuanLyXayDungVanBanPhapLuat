using System.ComponentModel.DataAnnotations;

namespace XayDungVanBanService.Infrastructure.Persistence.Entities;

public sealed class HoSoXayDungVanBanYKienDonVi : BaseEntity
{
    public Guid HoSoXayDungVanBanId { get; set; }
    public Guid DonViGopYId { get; set; }
    public DateTime? NgayNhan { get; set; }
    [Required, MaxLength(30)] public string KetQua { get; set; } = string.Empty;
    public string? NoiDungYKien { get; set; }
    public HoSoXayDungVanBan HoSoXayDungVanBan { get; set; } = null!;
}
