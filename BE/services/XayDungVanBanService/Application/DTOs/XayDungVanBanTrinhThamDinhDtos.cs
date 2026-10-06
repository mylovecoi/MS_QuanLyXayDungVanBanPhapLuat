using System.ComponentModel.DataAnnotations;

namespace XayDungVanBanService.Application.DTOs;

public sealed class TaoHoSoTrinhThamDinhRequest
{
    public Guid HoSoId { get; init; }
    public Guid BuocQuyTrinhTiepTheoId { get; init; }
    public Guid DonViNhanThamDinhId { get; init; }
    public string? NoiDungGhiChu { get; init; }
}

public sealed class CapNhatHoSoTrinhThamDinhRequest
{
    [MaxLength(100)] public string? SoToTrinh { get; init; }
    public DateTime? NgayToTrinh { get; init; }
    public Guid DonViNhanThamDinhId { get; init; }
    public string? NoiDungDeNghiThamDinh { get; init; }
    public DateTime? HanDeNghiTraKetQua { get; init; }
    public string? NoiDungGhiChu { get; init; }
}

public sealed record XayDungVanBanTrinhThamDinhDto(
    Guid HoSoId,
    Guid BoHoSoId,
    Guid BoHoSoNguonId,
    string TrangThai,
    string? SoToTrinh,
    DateTime? NgayToTrinh,
    DateTime? NgayGuiThamDinh,
    Guid DonViNhanThamDinhId,
    string? NoiDungDeNghiThamDinh,
    DateTime? HanDeNghiTraKetQua,
    string? NoiDungGhiChu);

public sealed record DieuKienGuiThamDinhDto(bool Dat, IReadOnlyList<string> DieuKienChuaDat);

public sealed class GuiThamDinhRequest
{
    public Guid BuocQuyTrinhTiepTheoId { get; init; }
    public Guid TrangThaiHoSoTiepTheoId { get; init; }
}

public sealed record TaiTaiLieuTrinhThamDinhRequest(
    Guid LoaiTaiLieuId,
    string TenTaiLieu,
    string TenFile,
    string? MimeType,
    Stream NoiDung);
