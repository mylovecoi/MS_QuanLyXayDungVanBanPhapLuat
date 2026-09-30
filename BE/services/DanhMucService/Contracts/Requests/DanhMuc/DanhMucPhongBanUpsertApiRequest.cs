namespace DanhMucService.Contracts.Requests.DanhMuc;

public class DanhMucPhongBanUpsertApiRequest
{
    public string TenPhongBan { get; set; } = string.Empty;
    public string MaPhongBan { get; set; } = string.Empty;
    public int LoaiPhongBan { get; set; }
    public Guid DanhMucDonViId { get; set; }
}

