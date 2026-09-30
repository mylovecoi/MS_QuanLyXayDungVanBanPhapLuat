using DanhMucService.Domain.Common;

namespace DanhMucService.Domain.Entities.DanhMuc;

public class DanhMucDiaDanhEntity : BaseEntity
{
    public string TenDiaDanh { get; set; } = string.Empty;
    public int Level { get; set; }
    public int STTSapXep { get; set; }
    public Guid DiaDanhCapTrenId { get; set; }
    public string? TenDiaDanhChuQuan { get; set; }
}

