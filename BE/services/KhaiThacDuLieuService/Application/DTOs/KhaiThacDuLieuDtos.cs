namespace KhaiThacDuLieuService.Application.DTOs;

public sealed record PagedResultDto<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageSize,
    int PageCurrent);

public sealed class CanhBaoListRequest
{
    public string? Search { get; init; }
    public string? NhomCanhBao { get; init; }
    public string? TrangThaiXuLy { get; init; }
    public string? MucDo { get; init; }
    public Guid? DonViNhanId { get; init; }
    public Guid? NguoiNhanId { get; init; }
    public DateTime? TuNgay { get; init; }
    public DateTime? DenNgay { get; init; }
    public int PageSize { get; init; } = 20;
    public int PageCurrent { get; init; } = 1;
}

public sealed record CanhBaoDto(
    Guid Id,
    string MaCanhBao,
    string NhomCanhBao,
    string DoiTuongNguon,
    Guid DoiTuongNguonId,
    string TieuDe,
    string NoiDung,
    string MucDo,
    string TrangThaiXuLy,
    DateTime? HanXuLy,
    DateTime NgayPhatSinh,
    Guid? NguoiNhanId,
    Guid? DonViNhanId,
    Guid? NguoiXuLyId,
    DateTime? NgayXem,
    DateTime? NgayXuLy,
    string? GhiChuXuLy);

public sealed record TaoCanhBaoRequest(
    string MaCanhBao,
    string NhomCanhBao,
    string DoiTuongNguon,
    Guid DoiTuongNguonId,
    string TieuDe,
    string NoiDung,
    string? MucDo,
    DateTime? HanXuLy,
    Guid? NguoiNhanId,
    Guid? DonViNhanId);

public sealed record XacNhanXuLyCanhBaoRequest(string? GhiChuXuLy);

public sealed record CauHinhCanhBaoDto(
    Guid Id,
    string MaCanhBao,
    string TenCanhBao,
    string NhomCanhBao,
    int SoNgayCanhBaoTruocHan,
    string MucDoMacDinh,
    string? KenhThongBao,
    bool TrangThai);

public sealed record CapNhatCauHinhCanhBaoRequest(
    string TenCanhBao,
    string NhomCanhBao,
    int SoNgayCanhBaoTruocHan,
    string MucDoMacDinh,
    string? KenhThongBao,
    bool TrangThai);

public sealed record DashboardTongQuanDto(
    int TongCanhBao,
    int CanhBaoMoi,
    int CanhBaoDangXuLy,
    int CanhBaoDaXuLy,
    IReadOnlyList<ThongKeTheoNhomDto> TheoNhomCanhBao,
    IReadOnlyList<ThongKeTheoNhomDto> TheoMucDo);

public sealed record ThongKeTheoNhomDto(string Nhom, int SoLuong);

public sealed class TraCuuRequest
{
    public string? Search { get; init; }
    public Guid? DonViId { get; init; }
    public Guid? NguoiPhuTrachId { get; init; }
    public Guid? LoaiVanBanId { get; init; }
    public Guid? TrangThaiId { get; init; }
    public DateTime? TuNgay { get; init; }
    public DateTime? DenNgay { get; init; }
    public int? Nam { get; init; }
    public int PageSize { get; init; } = 20;
    public int PageCurrent { get; init; } = 1;
}

public sealed record TraCuuTongHopItemDto(
    string NguonDuLieu,
    Guid DoiTuongId,
    string MaDoiTuong,
    string TenDoiTuong,
    Guid? DonViId,
    Guid? TrangThaiId,
    DateTime? NgayBatDau,
    DateTime? HanHoanThanh,
    string? TinhTrang);

public sealed class BaoCaoRequest
{
    public Guid? DonViId { get; init; }
    public Guid? LoaiVanBanId { get; init; }
    public DateTime? TuNgay { get; init; }
    public DateTime? DenNgay { get; init; }
    public int? Nam { get; init; }
}

public sealed record BaoCaoTongHopDto(
    string LoaiBaoCao,
    DateTime NgayTongHop,
    IReadOnlyList<ThongKeTheoNhomDto> SoLieu);
