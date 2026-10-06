namespace DanhMucService.Application.DTOs.DanhMuc;

public class DanhMucTrangThaiDto
{
    public Guid Id { get; set; }
    public string NhomTrangThai { get; set; } = string.Empty;
    public string MaTrangThai { get; set; } = string.Empty;
    public string TenTrangThai { get; set; } = string.Empty;
    public string MaMauHex { get; set; } = string.Empty;
    public int ThuTuSapXep { get; set; }
    public bool TrangThai { get; set; }
    public string? MoTa { get; set; }
    public string? GhiChu { get; set; }
}

