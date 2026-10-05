using System.ComponentModel.DataAnnotations;

namespace XayDungVanBanService.Infrastructure.Persistence.Entities;

public sealed class HoSoXayDungVanBan : BaseEntity
{
    [Required, MaxLength(50)] public string MaHoSo { get; set; } = string.Empty;
    [Required, MaxLength(500)] public string TenHoSo { get; set; } = string.Empty;
    [Required, MaxLength(500)] public string TenDuThaoVanBan { get; set; } = string.Empty;

    public Guid DanhMucVanBanId { get; set; }
    public Guid QuyTrinhSoanThaoId { get; set; }
    public Guid BuocHienTaiId { get; set; }
    public Guid TrangThaiHoSoId { get; set; }
    public Guid DonViChuTriSoanThaoId { get; set; }
    public Guid? NguoiPhuTrachId { get; set; }
    public int NamXayDung { get; set; }
    public DateTime? ThoiGianDuKienBatDau { get; set; }
    public DateTime? ThoiGianDuKienHoanThanh { get; set; }
    public string? MoTa { get; set; }
    public Guid? HoSoDangKyXayDungVanBanId { get; set; }

    public ICollection<BoHoSoNghiepVu> BoHoSos { get; set; } = [];
    public ICollection<HoSoXayDungVanBanFile> Files { get; set; } = [];
    public ICollection<HoSoXayDungVanBanLichSuXuLy> LichSuXuLys { get; set; } = [];
}
