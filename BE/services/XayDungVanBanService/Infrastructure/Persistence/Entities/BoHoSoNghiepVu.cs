namespace XayDungVanBanService.Infrastructure.Persistence.Entities;

public sealed class BoHoSoNghiepVu : BaseEntity
{
    public Guid HoSoXayDungVanBanId { get; set; }
    public Guid BuocQuyTrinhId { get; set; }
    public LoaiBoHoSo LoaiBoHoSo { get; set; }
    public TrangThaiBoHoSo TrangThai { get; set; } = TrangThaiBoHoSo.Nhap;
    public int LanXuLy { get; set; } = 1;
    public Guid? BoHoSoNguonId { get; set; }
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    public DateTime? NgayGui { get; set; }
    public DateTime? NgayHoanThanh { get; set; }
    public Guid NguoiLapId { get; set; }
    public Guid DonViLapId { get; set; }
    public string? LyDoTraLai { get; set; }
    public string? NoiDungGhiChu { get; set; }

    public HoSoXayDungVanBan HoSoXayDungVanBan { get; set; } = null!;
    public BoHoSoNghiepVu? BoHoSoNguon { get; set; }
    public ICollection<BoHoSoNghiepVuTaiLieu> TaiLieus { get; set; } = [];
    public ICollection<HoSoXayDungVanBanLichSuXuLy> LichSuXuLys { get; set; } = [];
    public HoSoXayDungVanBanSoanThao? SoanThao { get; set; }
    public HoSoXayDungVanBanTrinhThamDinh? TrinhThamDinh { get; set; }
    public HoSoXayDungVanBanThamDinh? ThamDinh { get; set; }
    public HoSoXayDungVanBanTrinhPheDuyet? TrinhPheDuyet { get; set; }
    public HoSoXayDungVanBanYKienUbnd? YKienUbnd { get; set; }
    public HoSoXayDungVanBanThamTraHdnd? ThamTraHdnd { get; set; }
    public HoSoXayDungVanBanKetQuaBanHanh? KetQuaBanHanh { get; set; }
}
