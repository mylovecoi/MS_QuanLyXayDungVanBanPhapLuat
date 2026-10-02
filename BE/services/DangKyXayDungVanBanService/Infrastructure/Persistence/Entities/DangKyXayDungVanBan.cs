using System.ComponentModel.DataAnnotations;

namespace DangKyXayDungVanBanService.Infrastructure.Persistence.Entities;

public class DangKyXayDungVanBan : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string MaHoSo { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TenHoSo { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TenVanBanDuKien { get; set; } = string.Empty;

    public Guid LoaiVanBanId { get; set; }
    public Guid QuyTrinhSoanThaoId { get; set; }
    public Guid BuocHienTaiId { get; set; }
    public Guid TrangThaiHoSoId { get; set; }
    public Guid DonViSoanThaoId { get; set; }
    public Guid DonViPheDuyetId { get; set; }
    public int NamDangKy { get; set; }
    public string? CanCuDeXuat { get; set; }
    public string? SuCanThiet { get; set; }
    public string? NoiDungChinhSach { get; set; }
    public DateTime? DuKienThoiGianTrinh { get; set; }
    public Guid? KetQuaPheDuyetId { get; set; }
    public bool DaKhoiTaoQuyTrinhXayDung { get; set; }
    public Guid? HoSoXayDungVanBanId { get; set; }
    public Guid? QuyTrinhXayDungTiepTheoId { get; set; }
    public DateTime? NgayKhoiTaoQuyTrinhXayDung { get; set; }

    public ICollection<DangKyXayDungVanBanFile> Files { get; set; } = new List<DangKyXayDungVanBanFile>();
    public ICollection<DangKyXayDungVanBanLichSuXuLy> LichSuXuLys { get; set; } = new List<DangKyXayDungVanBanLichSuXuLy>();
    public ICollection<DangKyXayDungVanBanKetQuaPheDuyet> KetQuaPheDuyets { get; set; } = new List<DangKyXayDungVanBanKetQuaPheDuyet>();
}
