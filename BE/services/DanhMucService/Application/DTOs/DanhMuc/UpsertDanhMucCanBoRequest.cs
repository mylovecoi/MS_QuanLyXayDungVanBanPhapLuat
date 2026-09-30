using DanhMucService.Domain.Enums;

namespace DanhMucService.Application.DTOs.DanhMuc;

public class UpsertDanhMucCanBoRequest
{
    public Guid? Id { get; set; }
    public Guid DonViQuanLyId { get; set; }
    public string TenCanBo { get; set; } = string.Empty;
    public DateTime? NgaySinh { get; set; }
    public Guid UserId { get; set; }
    public Guid PhongBanId { get; set; }
    public bool GioiTinh { get; set; }
    public string TrinhDoChuyenMon { get; set; } = string.Empty;
    public LoaiLaoDongType LoaiLaoDong { get; set; }
    public decimal SoTienBHXH { get; set; }
    public decimal SoTienBHYT { get; set; }
    public string? SoQuyetDinhDung { get; set; }
    public DateTime? NgayQuyetDinhDung { get; set; }
    public string? GhiChu { get; set; }
    public string? SoQuyetDinhBoNhiem { get; set; }
    public DateTime? NgayQuyetDinhBoNhiem { get; set; }
    public string? SoQuyetDinhCapThe { get; set; }
    public DateTime? NgayQuyetDinhCapThe { get; set; }
    public string? SoTheCongChungVien { get; set; }
    public string? ChucVu { get; set; }
    public decimal? MucPhiBaoHiemTrachNhiem { get; set; }
    public string? ViTriViecLam { get; set; }
    public DateTime? NgayTuyenDung { get; set; }
    public string? SoHopDongLaoDong { get; set; }
    public DateTime? NgayKyHopDongLaoDong { get; set; }
}

