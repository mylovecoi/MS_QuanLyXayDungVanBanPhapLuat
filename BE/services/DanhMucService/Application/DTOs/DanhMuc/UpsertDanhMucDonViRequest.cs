namespace DanhMucService.Application.DTOs.DanhMuc;

public class UpsertDanhMucDonViRequest
{
    public string TenDonVi { get; set; } = string.Empty;
    public int Level { get; set; }
    public int STTSapXep { get; set; }
    public Guid DonViChuQuanId { get; set; }
    public string? DiaChi { get; set; }
    public string? MaQHNS { get; set; }
    public string? SoDienThoai { get; set; }
    public string? ChucDanhQuanLy { get; set; }
    public string? HoVaTenNguoiQuanLy { get; set; }
    public string? PhanLoaiDonVi { get; set; }
    public bool TinhNangThanhToan { get; set; } = true;
}

