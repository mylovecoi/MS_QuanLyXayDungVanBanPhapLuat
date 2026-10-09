namespace XayDungVanBanService.Application.DTOs;
public sealed class TaoHoSoTrinhPheDuyetRequest { public Guid HoSoId { get; init; } public string CapTrinh { get; init; } = string.Empty; public string MucDichTrinh { get; init; } = string.Empty; }
public sealed class CapNhatHoSoTrinhPheDuyetRequest { public string CapTrinh { get; init; } = string.Empty; public string MucDichTrinh { get; init; } = string.Empty; public string? SoToTrinh { get; init; } public DateTime? NgayToTrinh { get; init; } public string? NoiDungTrinh { get; init; } public Guid? DonViDongGuiId { get; init; } }
public sealed record GuiPheDuyetRequest(DateTime? NgayTrinh, Guid BuocQuyTrinhTiepTheoId, Guid TrangThaiHoSoTiepTheoId, DateTime? HanXuLy, DateTime? ThoiGianCanhBao, int? SoNgayXuLy, int? SoNgayCanhBao);
public sealed record HoSoTrinhPheDuyetListItemDto(Guid HoSoId, Guid? BoHoSoId, string MaHoSo, string TenHoSo, string? TenDuThaoVanBan, int NamXayDung, string TrangThai, DateTime? NgayThamDinh, DateTime? NgayTrinh);
public sealed record XayDungVanBanTrinhPheDuyetDto(Guid HoSoId, Guid BoHoSoId, string TrangThai, string CapTrinh, string MucDichTrinh, string? SoToTrinh, DateTime? NgayToTrinh, DateTime? NgayTrinh, string? NoiDungTrinh, Guid? DonViDongGuiId);
public sealed record DieuKienGuiPheDuyetDto(bool Dat, IReadOnlyList<string> DieuKienChuaDat);
public sealed record TaiTaiLieuTrinhPheDuyetRequest(Guid LoaiTaiLieuId, string TenTaiLieu, string TenFile, string? MimeType, Stream NoiDung);
