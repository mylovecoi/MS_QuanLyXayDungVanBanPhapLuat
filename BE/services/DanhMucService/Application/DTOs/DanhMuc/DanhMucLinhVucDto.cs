namespace DanhMucService.Application.DTOs.DanhMuc;

public class DanhMucLinhVucDto
{
    public Guid Id { get; set; }
    public string MaLinhVuc { get; set; } = string.Empty;
    public string TenLinhVuc { get; set; } = string.Empty;
    public int ThuTuSapXep { get; set; }
    public bool TrangThai { get; set; }
    public string? MoTa { get; set; }
    public string? GhiChu { get; set; }
}

