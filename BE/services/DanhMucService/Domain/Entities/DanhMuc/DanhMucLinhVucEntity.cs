using DanhMucService.Domain.Common;

namespace DanhMucService.Domain.Entities.DanhMuc;

public class DanhMucLinhVucEntity : BaseEntity
{
    public string MaLinhVuc { get; set; } = string.Empty;
    public string TenLinhVuc { get; set; } = string.Empty;
    public int ThuTuSapXep { get; set; }
    public bool TrangThai { get; set; }
    public string? MoTa { get; set; }
    public string? GhiChu { get; set; }
}

