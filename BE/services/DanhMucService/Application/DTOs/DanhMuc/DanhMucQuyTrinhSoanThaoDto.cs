namespace DanhMucService.Application.DTOs.DanhMuc;

public class DanhMucQuyTrinhSoanThaoDto
{
    public Guid Id { get; set; }
    public string MaQuyTrinh { get; set; } = string.Empty;
    public string TenQuyTrinh { get; set; } = string.Empty;
    public string LoaiQuyTrinh { get; set; } = "XayDung";
    public string? TenLoaiQuyTrinh { get; set; }
    public Guid? DanhMucVanBanId { get; set; }
    public IReadOnlyList<Guid> DanhMucVanBanIds { get; set; } = Array.Empty<Guid>();
    public string? TenLoaiVanBan { get; set; }
    public string? CapApDung { get; set; }
    public IReadOnlyList<string> CapApDungs { get; set; } = Array.Empty<string>();
    public int PhienBan { get; set; }
    public bool TrangThai { get; set; }
    public string? MoTa { get; set; }
    public string? GhiChu { get; set; }
    public int SoBuoc { get; set; }
    public int SoNhanhChuyen { get; set; }
    public IReadOnlyList<DanhMucBuocQuyTrinhDto> BuocQuyTrinhs { get; set; } = Array.Empty<DanhMucBuocQuyTrinhDto>();
    public IReadOnlyList<DanhMucChuyenBuocQuyTrinhDto> ChuyenBuocs { get; set; } = Array.Empty<DanhMucChuyenBuocQuyTrinhDto>();
}

