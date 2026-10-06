using System.ComponentModel.DataAnnotations;

namespace XayDungVanBanService.Infrastructure.Persistence.Entities;

public sealed class HoSoXayDungVanBanFile : BaseEntity
{
    public Guid HoSoXayDungVanBanId { get; set; }
    public Guid LoaiTaiLieuId { get; set; }
    [Required, MaxLength(500)] public string TenTaiLieu { get; set; } = string.Empty;
    public int PhienBan { get; set; } = 1;
    [Required, MaxLength(500)] public string TenFile { get; set; } = string.Empty;
    [Required, MaxLength(1000)] public string DuongDanFile { get; set; } = string.Empty;
    [MaxLength(255)] public string? MimeType { get; set; }
    public long? DungLuong { get; set; }
    [MaxLength(128)] public string? HashFile { get; set; }
    public bool IsCurrent { get; set; } = true;
    public Guid NguoiTaiLenId { get; set; }
    public DateTime NgayTaiLen { get; set; } = DateTime.UtcNow;
    public HoSoXayDungVanBan HoSoXayDungVanBan { get; set; } = null!;
}

public sealed class BoHoSoNghiepVuTaiLieu : BaseEntity
{
    public Guid BoHoSoNghiepVuId { get; set; }
    public Guid HoSoXayDungVanBanFileId { get; set; }
    public Guid LoaiTaiLieuId { get; set; }
    [Required, MaxLength(30)] public string HinhThucThem { get; set; } = "TaoMoi";
    public Guid? BoHoSoTaiLieuNguonId { get; set; }
    public bool BatBuoc { get; set; }
    public bool DaKiemTra { get; set; }
    public string? GhiChu { get; set; }
    public int ThuTu { get; set; }
    public BoHoSoNghiepVu BoHoSoNghiepVu { get; set; } = null!;
    public HoSoXayDungVanBanFile HoSoXayDungVanBanFile { get; set; } = null!;
    public BoHoSoNghiepVuTaiLieu? BoHoSoTaiLieuNguon { get; set; }
}

public sealed class HoSoXayDungVanBanLichSuXuLy : BaseEntity
{
    public Guid HoSoXayDungVanBanId { get; set; }
    public Guid? BoHoSoNghiepVuId { get; set; }
    public Guid? BuocQuyTrinhTruocId { get; set; }
    public Guid? BuocQuyTrinhSauId { get; set; }
    public Guid? TrangThaiTruocId { get; set; }
    public Guid? TrangThaiSauId { get; set; }
    [MaxLength(250)] public string? TenBuocTruoc { get; set; }
    [MaxLength(250)] public string? TenBuocSau { get; set; }
    public int? ThuTuBuocTruoc { get; set; }
    public int? ThuTuBuocSau { get; set; }
    [Required, MaxLength(100)] public string HanhDong { get; set; } = string.Empty;
    public string? NoiDung { get; set; }
    public string? LyDo { get; set; }
    public Guid NguoiXuLyId { get; set; }
    public Guid DonViXuLyId { get; set; }
    public DateTime ThoiGianXuLy { get; set; } = DateTime.UtcNow;
    public Guid? HoSoXayDungVanBanFileId { get; set; }
    public Guid? BoHoSoNghiepVuTaiLieuId { get; set; }
    public HoSoXayDungVanBan HoSoXayDungVanBan { get; set; } = null!;
    public BoHoSoNghiepVu? BoHoSoNghiepVu { get; set; }
}
