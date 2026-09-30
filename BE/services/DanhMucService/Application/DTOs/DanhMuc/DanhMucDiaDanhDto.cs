namespace DanhMucService.Application.DTOs.DanhMuc;

public class DanhMucDiaDanhDto
{
    public Guid Id { get; set; }
    public string TenDiaDanh { get; set; } = string.Empty;
    public int Level { get; set; }
    public int STTSapXep { get; set; }
    public Guid DiaDanhCapTrenId { get; set; }
    public string? TenDiaDanhChuQuan { get; set; }
}

