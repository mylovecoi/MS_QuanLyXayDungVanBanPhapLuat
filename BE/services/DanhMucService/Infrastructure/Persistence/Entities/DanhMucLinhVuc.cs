using System.ComponentModel.DataAnnotations;

namespace DanhMucService.Infrastructure.Persistence.Entities;

public class DanhMucLinhVuc : BaseEntity
{
    [Required]
    public string MaLinhVuc { get; set; } = string.Empty;

    [Required]
    public string TenLinhVuc { get; set; } = string.Empty;

    public int ThuTuSapXep { get; set; }

    public bool TrangThai { get; set; } = true;

    public string? MoTa { get; set; }

    public string? GhiChu { get; set; }
}
