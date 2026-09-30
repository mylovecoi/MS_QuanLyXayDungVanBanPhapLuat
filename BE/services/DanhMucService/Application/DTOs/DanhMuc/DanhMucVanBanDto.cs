namespace DanhMucService.Application.DTOs.DanhMuc;

public class DanhMucVanBanDto
{
    public Guid Id { get; set; }
    public string TenLoaiVanBan { get; set; } = string.Empty;
    public string CapChinhQuyen { get; set; } = string.Empty;
    public string ChuTheBanHanh { get; set; } = string.Empty;
    public string? KyHieuMau { get; set; }
    public int ThuTuSapXep { get; set; }
    public bool TrangThai { get; set; }
    public string? MoTa { get; set; }
    public string? GhiChu { get; set; }
}

