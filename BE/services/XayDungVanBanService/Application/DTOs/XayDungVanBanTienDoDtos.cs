namespace XayDungVanBanService.Application.DTOs;

public sealed class XayDungVanBanTienDoListRequest
{
    public string? Search { get; init; }
    public Guid? DanhMucVanBanId { get; init; }
    public Guid? QuyTrinhSoanThaoId { get; init; }
    public Guid? BuocHienTaiId { get; init; }
    public Guid? TrangThaiHoSoId { get; init; }
    public Guid? DonViChuTriSoanThaoId { get; init; }
    public Guid? NguoiPhuTrachId { get; init; }
    public int? NamXayDung { get; init; }
    public DateTime? TuNgayHan { get; init; }
    public DateTime? DenNgayHan { get; init; }
    public string? TinhTrangTienDo { get; init; }
    public int PageSize { get; init; } = 20;
    public int PageCurrent { get; init; } = 1;
}

public sealed record XayDungVanBanTienDoListItemDto(
    Guid HoSoId,
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
    string TinhTrangTienDo,
    int SoNgayConLai,
    int SoLanNhacNho,
    DateTime? LanNhacNhoGanNhat,
    DateTime CreatedAt);

public sealed record XayDungVanBanTienDoDetailDto(
    Guid HoSoId,
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
    string TinhTrangTienDo,
    int SoNgayConLai,
    int SoLanNhacNho,
    DateTime? LanNhacNhoGanNhat,
    IReadOnlyList<XayDungVanBanBoHoSoDto> BoHoSos,
    IReadOnlyList<XayDungVanBanNhacTienDoDto> NhacTienDos);

public sealed record XayDungVanBanNhacTienDoDto(
    Guid Id,
    Guid HoSoXayDungVanBanId,
    string LoaiNhacNho,
    string TrangThaiXuLy,
    string NoiDungNhacNho,
    Guid NguoiGuiId,
    Guid? DonViGuiId,
    Guid? NguoiNhanId,
    Guid? DonViNhanId,
    DateTime NgayGui,
    DateTime? NgayXem,
    string? PhanHoi,
    DateTime? NgayPhanHoi,
    Guid? NguoiPhanHoiId,
    DateTime? NgayXacNhanXuLy,
    Guid? NguoiXacNhanXuLyId,
    string? GhiChuXuLy);

public sealed record TaoNhacTienDoRequest(
    string NoiDungNhacNho,
    string? LoaiNhacNho,
    Guid? NguoiNhanId,
    Guid? DonViNhanId);

public sealed record CapNhatPhanHoiNhacTienDoRequest(string PhanHoi);

public sealed record XacNhanXuLyNhacTienDoRequest(string? GhiChuXuLy);
