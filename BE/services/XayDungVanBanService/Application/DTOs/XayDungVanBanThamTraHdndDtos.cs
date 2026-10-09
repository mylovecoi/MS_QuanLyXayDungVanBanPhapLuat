namespace XayDungVanBanService.Application.DTOs;

public sealed class CapNhatThamTraHdndRequest
{
    public Guid? BanHdndThamTraId { get; init; }
    public DateTime? NgayTrinhThamTra { get; init; }
    public DateTime? NgayNhanKetQuaThamTra { get; init; }
    public string? KetQuaThamTra { get; init; }
    public string? NoiDungKienNghi { get; init; }
    public string? NoiDungTiepThuGiaiTrinh { get; init; }
    public DateTime? NgayNhanYKienThaoLuan { get; init; }
    public string? NoiDungTongHopYKienThaoLuan { get; init; }
}

public sealed record GuiKetQuaThamTraHdndRequest(
    Guid BuocQuyTrinhTiepTheoId,
    Guid TrangThaiHoSoTiepTheoId,
    DateTime? HanXuLy,
    DateTime? ThoiGianCanhBao,
    int? SoNgayXuLy,
    int? SoNgayCanhBao);

public sealed record HoSoThamTraHdndListItemDto(
    Guid HoSoId,
    Guid BoHoSoId,
    string MaHoSo,
    string TenHoSo,
    string? TenDuThaoVanBan,
    int NamXayDung,
    string TrangThai,
    DateTime NgayTao,
    DateTime? ThoiGianDuKienHoanThanh,
    DateTime? NgayTrinhThamTra,
    DateTime? NgayNhanKetQuaThamTra,
    string? KetQuaThamTra);

public sealed record XayDungVanBanThamTraHdndDto(
    Guid HoSoId,
    Guid BoHoSoId,
    Guid BuocQuyTrinhId,
    string TrangThai,
    Guid? BanHdndThamTraId,
    DateTime? NgayTrinhThamTra,
    DateTime? NgayNhanKetQuaThamTra,
    string? KetQuaThamTra,
    string? NoiDungKienNghi,
    string? NoiDungTiepThuGiaiTrinh,
    DateTime? NgayNhanYKienThaoLuan,
    string? NoiDungTongHopYKienThaoLuan);

public sealed record DieuKienGuiThamTraHdndDto(bool Dat, IReadOnlyList<string> DieuKienChuaDat);

public sealed record TaiTaiLieuThamTraHdndRequest(Guid LoaiTaiLieuId, string TenTaiLieu, string TenFile, string? MimeType, Stream NoiDung);
