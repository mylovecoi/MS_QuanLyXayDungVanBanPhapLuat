namespace DanhMucService.Application.DTOs.DanhMuc;

public class DanhMucPhongBanDto
{
    public Guid Id { get; set; }
    public string TenPhongBan { get; set; } = string.Empty;
    public string MaPhongBan { get; set; } = string.Empty;
    public int LoaiPhongBan { get; set; }
    public string? TenLoaiPhongBan { get; set; }
    public Guid DanhMucDonViId { get; set; }
    public string? TenDonVi { get; set; }
}

