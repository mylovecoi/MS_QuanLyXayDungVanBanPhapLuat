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

public sealed record CanhBaoNhacViecDto(
    Guid Id,
    Guid CanhBaoId,
    string TieuDe,
    string NoiDung,
    Guid NguoiGiaoId,
    Guid? DonViGiaoId,
    Guid? NguoiNhanId,
    Guid? DonViNhanId,
    DateTime? HanXuLy,
    DateTime? ThoiGianNhac,
    string MucDoUuTien,
    string TrangThai,
    DateTime NgayGui,
    DateTime? NgayXem,
    DateTime? NgayHoanThanh,
    string? GhiChuHoanThanh);

public sealed record TaoCanhBaoNhacViecRequest(
    string TieuDe,
    string NoiDung,
    Guid? NguoiNhanId,
    Guid? DonViNhanId,
    DateTime? HanXuLy,
    DateTime? ThoiGianNhac,
    string? MucDoUuTien);

public sealed record HoanThanhCanhBaoNhacViecRequest(string? GhiChuHoanThanh);

public sealed record CanhBaoLichSuXuLyDto(
    Guid Id,
    Guid CanhBaoId,
    Guid? NhacViecId,
    string HanhDong,
    string? NoiDung,
    Guid? NguoiThucHienId,
    Guid? DonViThucHienId,
    string? TrangThaiTruoc,
    string? TrangThaiSau,
    DateTime ThoiGian);

public sealed record SinhCanhBaoResultDto(
    int SoCauHinhDangHoatDong,
    int SoDoiTuongDuocKiemTra,
    int SoCanhBaoTaoMoi,
    int SoCanhBaoCapNhat,
    int SoCanhBaoTuDongDong,
    int SoNhacViecChuyenQuaHan);

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

public sealed record DashboardWidgetDto(
    Guid Id,
    string MaWidget,
    string TenWidget,
    string NhomWidget,
    string LoaiWidget,
    string? MoTa,
    bool HienThi,
    int ThuTu,
    string DangHienThi,
    string KichThuoc,
    string? CauHinhBoLocJson,
    int? TuDongLamMoiPhut,
    DashboardWidgetDataDto? DuLieu);

public sealed record DashboardWidgetDataDto(
    int GiaTri,
    string? DonViTinh,
    IReadOnlyList<ThongKeTheoNhomDto> NhomSoLieu);

public sealed record DashboardLayoutDto(
    IReadOnlyList<DashboardWidgetDto> Widgets,
    DashboardTongQuanDto TongQuan);

public sealed class DashboardFilterRequest
{
    public int? Nam { get; init; }
    public Guid? DonViId { get; init; }
    public Guid? LoaiVanBanId { get; init; }
    public Guid? QuyTrinhSoanThaoId { get; init; }
    public Guid? TrangThaiId { get; init; }
    public DateTime? TuNgay { get; init; }
    public DateTime? DenNgay { get; init; }
}

public sealed record DashboardWidgetCauHinhRequest(
    Guid WidgetId,
    bool HienThi,
    int ThuTu,
    string DangHienThi,
    string KichThuoc,
    string? CauHinhBoLocJson,
    int? TuDongLamMoiPhut);

public sealed record CapNhatDashboardLayoutRequest(
    IReadOnlyList<DashboardWidgetCauHinhRequest> Widgets);

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
    public Guid? QuyTrinhSoanThaoId { get; init; }
    public Guid? TrangThaiId { get; init; }
    public DateTime? TuNgay { get; init; }
    public DateTime? DenNgay { get; init; }
    public int? Nam { get; init; }
}

public sealed record BaoCaoDanhMucDto(
    string MaBaoCao,
    string TenBaoCao,
    string MoTa,
    string NhomBaoCao,
    string Route,
    bool DaCoDuLieu);

public sealed record BaoCaoTongHopDto(
    string LoaiBaoCao,
    string TenBaoCao,
    DateTime NgayTongHop,
    IReadOnlyList<ThongKeTheoNhomDto> SoLieu,
    IReadOnlyList<ThongKeTheoNhomDto> TheoTrangThai,
    IReadOnlyList<ThongKeTheoNhomDto> TheoDonVi,
    IReadOnlyList<BaoCaoChiTietDto> ChiTiet);

public sealed record BaoCaoChiTietDto(
    Guid Id,
    string MaDoiTuong,
    string TenDoiTuong,
    string? NoiDung,
    Guid? DonViId,
    string? TenDonVi,
    Guid? TrangThaiId,
    string? TrangThai,
    DateTime? NgayPhatSinh,
    DateTime? HanHoanThanh,
    string? GhiChu);
