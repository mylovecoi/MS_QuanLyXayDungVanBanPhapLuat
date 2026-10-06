namespace XayDungVanBanService.Application.DTOs;

public sealed record TaoChamDiemRequest(Guid TrangThaiNhapId, string? GhiChu);
public sealed record DieuChinhChamDiemRequest(decimal DiemDieuChinh, string LyDoDieuChinh);
public sealed record ChuyenTrangThaiChamDiemRequest(Guid TrangThaiId, string? GhiChu);
public sealed class ChamDiemListRequest
{
    public string? Search { get; init; }
    public Guid? HoSoId { get; init; }
    public Guid? TrangThaiId { get; init; }
    public int PageSize { get; init; } = 20;
    public int PageCurrent { get; init; } = 1;
}
public sealed record ChamDiemListItemDto(Guid Id, Guid HoSoId, string MaHoSo, string TenHoSo, string TenDuThaoVanBan, int LanCham, Guid TrangThaiId, decimal TongDiemChinhThuc, DateTime NgayCham, DateTime? NgayChot);
public sealed record ChamDiemChiTietDto(Guid Id, Guid DanhMucTieuChiDiemId, string MaTieuChi, string TenTieuChi, string? NhanMucDiem, decimal GiaTriDauVao, decimal DiemTuDong, decimal DiemDieuChinh, decimal DiemChinhThuc, decimal DiemToiDa, string? LyDoDieuChinh);
public sealed record ChamDiemDto(Guid Id, Guid HoSoId, int LanCham, Guid TrangThaiId, decimal TongDiemTuDong, decimal TongDiemDieuChinh, decimal TongDiemChinhThuc, DateTime NgayCham, DateTime? NgayChot, IReadOnlyList<ChamDiemChiTietDto> ChiTiets);
public sealed record ChamDiemLichSuDto(Guid Id, Guid? ChiTietId, string LoaiThaoTac, string NoiDung, DateTime ThoiGianThucHien, Guid NguoiThucHienId);
