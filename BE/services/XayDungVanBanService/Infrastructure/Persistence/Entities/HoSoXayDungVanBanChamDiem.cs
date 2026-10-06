using System.ComponentModel.DataAnnotations;

namespace XayDungVanBanService.Infrastructure.Persistence.Entities;

public sealed class HoSoXayDungVanBanChamDiem : BaseEntity
{
    public Guid HoSoXayDungVanBanId { get; set; }
    public int LanCham { get; set; }
    public Guid TrangThaiId { get; set; }
    public decimal TongDiemTuDong { get; set; }
    public decimal TongDiemDieuChinh { get; set; }
    public decimal TongDiemChinhThuc { get; set; }
    public DateTime? NgayBatDauThucTe { get; set; }
    public DateTime? NgayHoanThanhThucTe { get; set; }
    public int? SoNgayKeHoach { get; set; }
    public int? SoNgayThucTe { get; set; }
    public decimal? TyLeThoiGianThucTe { get; set; }
    public DateTime NgayCham { get; set; } = DateTime.UtcNow;
    public Guid NguoiChamId { get; set; }
    public DateTime? NgayChot { get; set; }
    public Guid? NguoiChotId { get; set; }
    [MaxLength(2000)] public string? GhiChu { get; set; }
    public ICollection<HoSoXayDungVanBanChamDiemChiTiet> ChiTiets { get; set; } = [];
    public ICollection<HoSoXayDungVanBanChamDiemLichSu> LichSus { get; set; } = [];
}

public sealed class HoSoXayDungVanBanChamDiemChiTiet : BaseEntity
{
    public Guid HoSoXayDungVanBanChamDiemId { get; set; }
    public Guid DanhMucTieuChiDiemId { get; set; }
    public Guid DanhMucTieuChiDiemMucId { get; set; }
    [MaxLength(100)] public string MaTieuChi { get; set; } = string.Empty;
    [MaxLength(250)] public string TenTieuChi { get; set; } = string.Empty;
    [MaxLength(250)] public string? NhanMucDiem { get; set; }
    public decimal GiaTriDauVao { get; set; }
    public decimal DiemToiDa { get; set; }
    public decimal DiemTuDong { get; set; }
    public decimal DiemDieuChinh { get; set; }
    public decimal DiemChinhThuc { get; set; }
    [MaxLength(2000)] public string? LyDoDieuChinh { get; set; }
    public Guid? NguoiDieuChinhId { get; set; }
    public DateTime? NgayDieuChinh { get; set; }
    public HoSoXayDungVanBanChamDiem? ChamDiem { get; set; }
}

public sealed class HoSoXayDungVanBanChamDiemLichSu : BaseEntity
{
    public Guid HoSoXayDungVanBanChamDiemId { get; set; }
    public Guid? HoSoXayDungVanBanChamDiemChiTietId { get; set; }
    [MaxLength(50)] public string LoaiThaoTac { get; set; } = string.Empty;
    [MaxLength(2000)] public string NoiDung { get; set; } = string.Empty;
    public string? DuLieuCu { get; set; }
    public string? DuLieuMoi { get; set; }
    public Guid NguoiThucHienId { get; set; }
    public DateTime ThoiGianThucHien { get; set; } = DateTime.UtcNow;
    public HoSoXayDungVanBanChamDiem? ChamDiem { get; set; }
}
