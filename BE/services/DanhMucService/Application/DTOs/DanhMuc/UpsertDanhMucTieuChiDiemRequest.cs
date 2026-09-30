namespace DanhMucService.Application.DTOs.DanhMuc;

public class UpsertDanhMucTieuChiDiemRequest
{
    public Guid? Id { get; set; }
    public string MaTieuChi { get; set; } = string.Empty;
    public string TenTieuChi { get; set; } = string.Empty;
    public string LoaiTieuChi { get; set; } = string.Empty;
    public string KieuGiaTri { get; set; } = string.Empty;
    public string DonViGiaTri { get; set; } = string.Empty;
    public int ThuTuSapXep { get; set; }
    public decimal DiemToiDa { get; set; }
    public bool TrangThai { get; set; } = true;
    public string? MoTa { get; set; }
    public string? GhiChu { get; set; }
    public List<UpsertDanhMucTieuChiDiemMucRequest> Mucs { get; set; } = new();
}

