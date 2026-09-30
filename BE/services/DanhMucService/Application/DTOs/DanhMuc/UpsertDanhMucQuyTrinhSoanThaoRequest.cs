namespace DanhMucService.Application.DTOs.DanhMuc;

public class UpsertDanhMucQuyTrinhSoanThaoRequest
{
    public Guid Id { get; set; }
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
    public List<UpsertDanhMucBuocQuyTrinhRequest> BuocQuyTrinhs { get; set; } = new();
    public List<UpsertDanhMucChuyenBuocQuyTrinhRequest> ChuyenBuocs { get; set; } = new();
}

