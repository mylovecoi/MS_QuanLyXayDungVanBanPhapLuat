namespace DangKyXayDungVanBanService.Application.DTOs;

public record DangKyXayDungVanBanDto(
    Guid Id,
    string MaHoSo,
    string TenHoSo,
    string TenVanBanDuKien,
    Guid LoaiVanBanId,
    Guid QuyTrinhSoanThaoId,
    Guid BuocHienTaiId,
    Guid TrangThaiHoSoId,
    Guid DonViSoanThaoId,
    Guid DonViPheDuyetId,
    int NamDangKy,
    bool DaKhoiTaoQuyTrinhXayDung,
    Guid? HoSoXayDungVanBanId,
    DateTime CreatedAt);

public record TaoDangKyXayDungVanBanRequest(
    string TenHoSo,
    string TenVanBanDuKien,
    Guid LoaiVanBanId,
    Guid DonViSoanThaoId,
    Guid DonViPheDuyetId,
    int NamDangKy,
    string? CanCuDeXuat,
    string? SuCanThiet,
    string? NoiDungChinhSach,
    DateTime? DuKienThoiGianTrinh,
    Guid NguoiXuLyId,
    string TenNguoiXuLy,
    string TenDonViXuLy);

public record CapNhatDangKyXayDungVanBanRequest(
    string TenHoSo,
    string TenVanBanDuKien,
    Guid LoaiVanBanId,
    Guid DonViPheDuyetId,
    int NamDangKy,
    string? CanCuDeXuat,
    string? SuCanThiet,
    string? NoiDungChinhSach,
    DateTime? DuKienThoiGianTrinh);

public record XuLyDangKyXayDungVanBanRequest(
    Guid HanhDongId,
    string? NoiDungXuLy,
    string? LyDoTraLai,
    Guid NguoiXuLyId,
    string TenNguoiXuLy,
    Guid DonViXuLyId,
    string TenDonViXuLy,
    string? TenBuocTu,
    string? TenBuocDen,
    string? TenTrangThaiTruoc,
    string? TenTrangThaiSau);

public record CapNhatKetQuaPheDuyetRequest(
    string KetQua,
    string? SoVanBan,
    DateTime? NgayVanBan,
    Guid CoQuanPheDuyetId,
    string TenCoQuanPheDuyet,
    string? NguoiKy,
    string? ChucVuNguoiKy,
    string? NoiDungKetQua,
    Guid? FileKetQuaId);

public record KhoiTaoQuyTrinhXayDungRequest(
    Guid HoSoXayDungVanBanId,
    Guid QuyTrinhXayDungId,
    string MaQuyTrinhXayDung,
    string TenQuyTrinhXayDung,
    Guid NguoiXuLyId,
    string TenNguoiXuLy,
    Guid DonViXuLyId,
    string TenDonViXuLy);

public record DangKyXayDungVanBanTimelineDto(
    Guid Id,
    Guid DangKyXayDungVanBanId,
    string MaHanhDong,
    string TenHanhDong,
    string? TenBuocTu,
    string? TenBuocDen,
    string? TenTrangThaiTruoc,
    string? TenTrangThaiSau,
    string? NoiDungXuLy,
    string? LyDoTraLai,
    string TenNguoiXuLy,
    string TenDonViXuLy,
    DateTime NgayXuLy);

public record HanhDongKhaDungDto(
    Guid HanhDongId,
    string MaHanhDong,
    string TenHanhDong,
    string LoaiHanhDong,
    bool YeuCauLyDo,
    bool YeuCauFileDinhKem,
    Guid BuocTiepTheoId,
    Guid TrangThaiTiepTheoId);

public record DangKyXayDungVanBanFileDto(
    Guid Id,
    Guid DangKyXayDungVanBanId,
    string LoaiFile,
    string TenFile,
    string DuongDanFile,
    long DungLuong,
    string? MimeType,
    string? MoTa,
    DateTime CreatedAt,
    string? CreatedBy);

public record TaiFileDangKyXayDungVanBanRequest(
    string LoaiFile,
    string TenFile,
    string? MimeType,
    string? MoTa,
    Guid NguoiTaiLenId,
    Stream NoiDung);
