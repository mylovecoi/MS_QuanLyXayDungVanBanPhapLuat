namespace XayDungVanBanService.Application.DTOs;

public sealed class CapNhatBanHanhRequest
{
    public string? KetQua { get; init; }
    public string? SoVanBan { get; init; }
    public DateTime? NgayBanHanh { get; init; }
    public Guid? CoQuanBanHanhId { get; init; }
    public Guid? NguoiKyId { get; init; }
    public string? ChucVuNguoiKy { get; init; }
    public DateTime? NgayCoHieuLuc { get; init; }
    public string? NoiDungKetQua { get; init; }
    public string? LyDoKhongThongQua { get; init; }
}

public sealed record HoSoBanHanhListItemDto(
    Guid HoSoId,
    Guid BoHoSoId,
    string MaHoSo,
    string TenHoSo,
    string? TenDuThaoVanBan,
    int NamXayDung,
    string TrangThai,
    DateTime NgayTao,
    DateTime? ThoiGianDuKienHoanThanh,
    DateTime? NgayHoanThanh,
    string? KetQua,
    string? SoVanBan,
    DateTime? NgayBanHanh);

public sealed record XayDungVanBanBanHanhDto(
    Guid HoSoId,
    Guid BoHoSoId,
    string TrangThai,
    string? KetQua,
    string? SoVanBan,
    DateTime? NgayBanHanh,
    Guid? CoQuanBanHanhId,
    Guid? NguoiKyId,
    string? ChucVuNguoiKy,
    DateTime? NgayCoHieuLuc,
    string? NoiDungKetQua,
    string? LyDoKhongThongQua);

public sealed record DieuKienHoanThanhBanHanhDto(bool Dat, IReadOnlyList<string> DieuKienChuaDat);

public sealed record TaiTaiLieuBanHanhRequest(Guid LoaiTaiLieuId, string TenTaiLieu, string TenFile, string? MimeType, Stream NoiDung);
