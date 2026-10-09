using System.ComponentModel.DataAnnotations;

namespace XayDungVanBanService.Application.DTOs;

public sealed class TaoHoSoSoanThaoRequest
{
    [Required, MaxLength(500)] public string TenHoSo { get; init; } = string.Empty;
    [Required, MaxLength(500)] public string TenDuThaoVanBan { get; init; } = string.Empty;
    public Guid DanhMucVanBanId { get; init; }
    public Guid QuyTrinhSoanThaoId { get; init; }
    public Guid BuocHienTaiId { get; init; }
    public Guid TrangThaiHoSoId { get; init; }
    public Guid DonViChuTriSoanThaoId { get; init; }
    public Guid? NguoiPhuTrachId { get; init; }
    [Range(2000, 9999)] public int NamXayDung { get; init; }
    public DateTime? ThoiGianDuKienBatDau { get; init; }
    public DateTime? ThoiGianDuKienHoanThanh { get; init; }
    public string? MoTa { get; init; }
    public Guid? HoSoDangKyXayDungVanBanId { get; init; }
    public string? CanCuXayDung { get; init; }
    public string? PhamViDieuChinh { get; init; }
    public string? NoiDungChinhSach { get; init; }
}

public sealed class CapNhatHoSoSoanThaoRequest
{
    [Required, MaxLength(500)] public string TenHoSo { get; init; } = string.Empty;
    [Required, MaxLength(500)] public string TenDuThaoVanBan { get; init; } = string.Empty;
    public Guid? DanhMucVanBanId { get; init; }
    public Guid? QuyTrinhSoanThaoId { get; init; }
    public Guid? BuocHienTaiId { get; init; }
    public Guid? TrangThaiHoSoId { get; init; }
    public Guid? DonViChuTriSoanThaoId { get; init; }
    public Guid? NguoiPhuTrachId { get; init; }
    [Range(2000, 9999)] public int NamXayDung { get; init; }
    public DateTime? ThoiGianDuKienBatDau { get; init; }
    public DateTime? ThoiGianDuKienHoanThanh { get; init; }
    public string? MoTa { get; init; }
    public Guid? HoSoDangKyXayDungVanBanId { get; init; }
    public string? CanCuXayDung { get; init; }
    public string? PhamViDieuChinh { get; init; }
    public string? NoiDungChinhSach { get; init; }
}

public sealed record XayDungVanBanSoanThaoDto(
    Guid HoSoId,
    Guid BoHoSoId,
    string MaHoSo,
    string TenHoSo,
    string TenDuThaoVanBan,
    Guid DanhMucVanBanId,
    Guid QuyTrinhSoanThaoId,
    Guid BuocHienTaiId,
    Guid TrangThaiHoSoId,
    Guid DonViChuTriSoanThaoId,
    Guid? NguoiPhuTrachId,
    int NamXayDung,
    DateTime? ThoiGianDuKienBatDau,
    DateTime? ThoiGianDuKienHoanThanh,
    string? MoTa,
    string? CanCuXayDung,
    string? PhamViDieuChinh,
    string? NoiDungChinhSach,
    DateTime CreatedAt);

public sealed class TaoYKienDonViRequest
{
    public Guid DonViGopYId { get; init; }
    public DateTime? NgayNhan { get; init; }
    [Required, MaxLength(30)] public string KetQua { get; init; } = string.Empty;
    public string? NoiDungYKien { get; init; }
}

public sealed class CapNhatYKienDonViRequest
{
    public DateTime? NgayNhan { get; init; }
    [Required, MaxLength(30)] public string KetQua { get; init; } = string.Empty;
    public string? NoiDungYKien { get; init; }
}

public sealed record XayDungVanBanYKienDonViDto(
    Guid Id,
    Guid DonViGopYId,
    DateTime? NgayNhan,
    string KetQua,
    string? NoiDungYKien,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed class CapNhatTongHopYKienRequest
{
    public string? NoiDungTongHopTiepThuGiaiTrinh { get; init; }
}

public sealed record XayDungVanBanTongHopYKienDto(
    int TongSoYKien,
    int SoDongY,
    int SoKhongDongY,
    int SoYKienKhac,
    int SoKhongPhanHoi,
    string? NoiDungTongHopTiepThuGiaiTrinh,
    Guid? LoaiTaiLieuTongHopYKienId);

public sealed record XayDungVanBanTaiLieuDto(
    Guid Id,
    Guid LoaiTaiLieuId,
    string TenTaiLieu,
    int PhienBan,
    string TenFile,
    string DuongDanFile,
    string? MimeType,
    long? DungLuong,
    bool IsCurrent,
    DateTime NgayTaiLen,
    Guid? BoHoSoTaiLieuId = null,
    string? HinhThucThem = null,
    string? LoaiDinhKem = null);

public sealed record TaiFileTongHopYKienRequest(
    Guid LoaiTaiLieuId,
    string TenFile,
    string? MimeType,
    Stream NoiDung);

public sealed record TaiTaiLieuSoanThaoRequest(
    Guid LoaiTaiLieuId,
    string TenTaiLieu,
    string TenFile,
    string? MimeType,
    Stream NoiDung);

public sealed record DieuKienTrinhThamDinhDto(
    bool Dat,
    IReadOnlyList<string> DieuKienChuaDat);

public sealed class TrinhThamDinhRequest
{
    public Guid BuocQuyTrinhTiepTheoId { get; init; }
    public Guid TrangThaiHoSoTiepTheoId { get; init; }
    public Guid DonViNhanThamDinhId { get; init; }
    public DateTime? NgayChuyen { get; init; }
    public DateTime? HanDeNghiTraKetQua { get; init; }
    public string? NoiDungGhiChu { get; init; }
}

public sealed record TrinhThamDinhDto(
    Guid HoSoId,
    Guid BoHoSoSoanThaoId,
    Guid BoHoSoTrinhThamDinhId,
    Guid BuocQuyTrinhHienTaiId,
    Guid TrangThaiHoSoId);
