namespace DanhMucService.Contracts.Requests.DanhMuc;

public class DanhMucLinhVucUpsertApiRequest
{
    public string MaLinhVuc { get; set; } = string.Empty;
    public string TenLinhVuc { get; set; } = string.Empty;
    public int ThuTuSapXep { get; set; }
    public bool TrangThai { get; set; } = true;
    public string? MoTa { get; set; }
    public string? GhiChu { get; set; }
}

