using DanhMucService.Domain.Common;

namespace DanhMucService.Domain.Entities.DanhMuc;

public class DanhMucQuyTrinhSoanThaoEntity : BaseEntity
{
    public string MaQuyTrinh { get; set; } = string.Empty;
    public string TenQuyTrinh { get; set; } = string.Empty;
    public string LoaiQuyTrinh { get; set; } = "XayDung";
    public Guid? DanhMucVanBanId { get; set; }
    public List<Guid> DanhMucVanBanIds { get; set; } = new();
    public string? CapApDung { get; set; }
    public List<string> CapApDungs { get; set; } = new();
    public int PhienBan { get; set; } = 1;
    public bool TrangThai { get; set; } = true;
    public string? MoTa { get; set; }
    public string? GhiChu { get; set; }
    public string? TenLoaiQuyTrinh { get; set; }
    public string? TenLoaiVanBan { get; set; }
    public int SoBuoc { get; set; }
    public int SoNhanhChuyen { get; set; }
    public List<DanhMucBuocQuyTrinhEntity> BuocQuyTrinhs { get; set; } = new();
    public List<DanhMucChuyenBuocQuyTrinhEntity> ChuyenBuocs { get; set; } = new();
}

