namespace XayDungVanBanService.Application.DTOs;
public sealed record TaoHoSoYKienUbndRequest(Guid HoSoId);
public sealed class CapNhatYKienUbndRequest { public DateTime? NgayNhanYKien { get; init; } public int? TongSoThanhVienDuocLayYKien { get; init; } public int? SoDongY { get; init; } public int? SoKhongDongY { get; init; } public int? SoYKienKhac { get; init; } public string? KetLuanTongHop { get; init; } public string? NoiDungTongHop { get; init; } public string? NoiDungGiaiTrinh { get; init; } }
public sealed record GuiYKienUbndRequest(Guid BuocQuyTrinhTiepTheoId,Guid TrangThaiHoSoTiepTheoId);
public sealed record XayDungVanBanYKienUbndDto(Guid HoSoId,Guid BoHoSoId,string TrangThai,DateTime? NgayNhanYKien,int? TongSoThanhVienDuocLayYKien,int? SoDongY,int? SoKhongDongY,int? SoYKienKhac,string? KetLuanTongHop,string? NoiDungTongHop,string? NoiDungGiaiTrinh);
public sealed record DieuKienGuiYKienUbndDto(bool Dat, IReadOnlyList<string> DieuKienChuaDat);
public sealed record TaiTaiLieuYKienUbndRequest(Guid LoaiTaiLieuId, string TenTaiLieu, string TenFile, string? MimeType, Stream NoiDung);
