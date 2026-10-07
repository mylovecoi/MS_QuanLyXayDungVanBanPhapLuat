using System.ComponentModel.DataAnnotations;

namespace XayDungVanBanService.Infrastructure.Persistence.Entities;

public sealed class HoSoXayDungVanBanNhacTienDo : BaseEntity
{
    public Guid HoSoXayDungVanBanId { get; set; }

    [Required, MaxLength(50)]
    public string LoaiNhacNho { get; set; } = "NHAC_TIEN_DO";

    [Required, MaxLength(50)]
    public string TrangThaiXuLy { get; set; } = "DA_GUI";

    [Required, MaxLength(2000)]
    public string NoiDungNhacNho { get; set; } = string.Empty;

    public Guid NguoiGuiId { get; set; }
    public Guid? DonViGuiId { get; set; }
    public Guid? NguoiNhanId { get; set; }
    public Guid? DonViNhanId { get; set; }
    public DateTime NgayGui { get; set; } = DateTime.UtcNow;
    public DateTime? NgayXem { get; set; }

    [MaxLength(2000)]
    public string? PhanHoi { get; set; }

    public DateTime? NgayPhanHoi { get; set; }
    public Guid? NguoiPhanHoiId { get; set; }
    public DateTime? NgayXacNhanXuLy { get; set; }
    public Guid? NguoiXacNhanXuLyId { get; set; }

    [MaxLength(2000)]
    public string? GhiChuXuLy { get; set; }

    public HoSoXayDungVanBan HoSoXayDungVanBan { get; set; } = null!;
}
