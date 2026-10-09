using System.ComponentModel.DataAnnotations;

namespace XayDungVanBanService.Application.DTOs;

public sealed record TaoHoSoThamDinhRequest(Guid HoSoId);
public sealed record HoSoThamDinhListItemDto(Guid HoSoId, Guid? BoHoSoId, string MaHoSo, string TenHoSo, string? TenDuThaoVanBan, int NamXayDung, string TrangThai, DateTime? NgayGuiThamDinh, DateTime? NgayTiepNhan);
public sealed record TiepNhanThamDinhRequest(DateTime? NgayTiepNhan, Guid NguoiTiepNhanId);
public sealed class CapNhatKetQuaThamDinhRequest { [Required] public string HinhThucThamDinh { get; init; } = string.Empty; public DateTime? NgayThamDinh { get; init; } [Required] public string KetQuaThamDinh { get; init; } = string.Empty; public string? NoiDungKetLuan { get; init; } public Guid? NguoiKetLuanId { get; init; } }
public sealed record YeuCauBoSungThamDinhRequest(string NoiDungYeuCauBoSung, DateTime? HanBoSung, Guid BuocQuyTrinhSoanThaoId, Guid TrangThaiHoSoBoSungId);
public sealed record TraLaiTrinhThamDinhRequest(string NoiDungYeuCauBoSung);
public sealed record GuiKetQuaThamDinhRequest(DateTime? NgayGuiKetQua, Guid BuocQuyTrinhTiepTheoId, Guid TrangThaiHoSoTiepTheoId);
public sealed record XayDungVanBanThamDinhDto(Guid HoSoId, Guid BoHoSoId, string TrangThai, DateTime? NgayTiepNhan, string? HinhThucThamDinh, DateTime? NgayThamDinh, string? KetQuaThamDinh, string? NoiDungKetLuan, string? NoiDungYeuCauBoSung, DateTime? HanBoSung, Guid? NguoiKetLuanId);
public sealed record DieuKienGuiKetQuaThamDinhDto(bool Dat, IReadOnlyList<string> DieuKienChuaDat);
public sealed record TaiTaiLieuThamDinhRequest(Guid LoaiTaiLieuId, string TenTaiLieu, string TenFile, string? MimeType, Stream NoiDung);
public sealed record SoSanhDuThaoRequest(Guid FileGocId, Guid FileSoSanhId);
public sealed record KetQuaSoSanhDuThaoDto(Guid Id, Guid HoSoId, Guid FileGocId, Guid FileSoSanhId, int SoNoiDungThem, int SoNoiDungXoa, int SoNoiDungSua, string NoiDungSoSanhHtml, DateTime CreatedAt);
