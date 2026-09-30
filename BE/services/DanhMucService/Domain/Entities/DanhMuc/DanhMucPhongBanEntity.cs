using DanhMucService.Domain.Common;

namespace DanhMucService.Domain.Entities.DanhMuc;

public class DanhMucPhongBanEntity : BaseEntity
{
    public string TenPhongBan { get; set; } = string.Empty;
    public string MaPhongBan { get; set; } = string.Empty;
    public int LoaiPhongBan { get; set; }
    public Guid DanhMucDonViId { get; set; }
    public string? TenDonVi { get; set; }
}

