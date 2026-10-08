namespace XayDungVanBanService.Application.DTOs;

public sealed class XayDungVanBanHoSoListRequest
{
    public string? Search { get; init; }
    public Guid? DanhMucVanBanId { get; init; }
    public Guid? QuyTrinhSoanThaoId { get; init; }
    public Guid? BuocHienTaiId { get; init; }
    public Guid? TrangThaiHoSoId { get; init; }
    public Guid? DonViChuTriSoanThaoId { get; init; }
    public Guid? NguoiPhuTrachId { get; init; }
    public int? NamXayDung { get; init; }
    public int PageSize { get; init; } = 20;
    public int PageCurrent { get; init; } = 1;
}

public sealed record PagedResultDto<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageSize,
    int PageCurrent);

public sealed record XayDungVanBanHoSoListItemDto(
    Guid Id,
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
    DateTime CreatedAt,
    int SoYKienDonVi);

public sealed record XayDungVanBanHoSoDetailDto(
    Guid Id,
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
    Guid? HoSoDangKyXayDungVanBanId,
    DateTime CreatedAt,
    IReadOnlyList<XayDungVanBanBoHoSoDto> BoHoSos);

public sealed record XayDungVanBanBoHoSoDto(
    Guid Id,
    Guid BuocQuyTrinhId,
    int LoaiBoHoSo,
    int TrangThai,
    int LanXuLy,
    DateTime NgayTao,
    DateTime? NgayGui,
    DateTime? NgayHoanThanh);

public sealed record XayDungVanBanTimelineItemDto(
    Guid Id,
    Guid? BoHoSoNghiepVuId,
    Guid? BuocQuyTrinhTruocId,
    Guid? BuocQuyTrinhSauId,
    Guid? TrangThaiTruocId,
    Guid? TrangThaiSauId,
    string? TenBuocTruoc,
    string? TenBuocSau,
    int? ThuTuBuocTruoc,
    int? ThuTuBuocSau,
    string HanhDong,
    string? NoiDung,
    string? LyDo,
    Guid NguoiXuLyId,
    Guid DonViXuLyId,
    DateTime ThoiGianXuLy,
    Guid? HoSoXayDungVanBanFileId,
    Guid? BoHoSoNghiepVuTaiLieuId);
