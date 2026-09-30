namespace DanhMucService.Contracts.Requests.DanhMuc;

public class DanhMucVanBanUpsertApiRequest
{
    public string TenLoaiVanBan { get; set; } = string.Empty;
    public string CapChinhQuyen { get; set; } = string.Empty;
    public string ChuTheBanHanh { get; set; } = string.Empty;
    public string? KyHieuMau { get; set; }
    public int ThuTuSapXep { get; set; } = 1;
    public bool TrangThai { get; set; } = true;
    public string? MoTa { get; set; }
    public string? GhiChu { get; set; }
}

