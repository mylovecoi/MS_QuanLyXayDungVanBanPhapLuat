using DanhMucService.Domain.Common;

namespace DanhMucService.Domain.Entities.DanhMuc;

public class DanhMucTieuChiDiemEntity : BaseEntity
{
    public string MaTieuChi { get; set; } = string.Empty;
    public string TenTieuChi { get; set; } = string.Empty;
    public string LoaiTieuChi { get; set; } = string.Empty;
    public string KieuGiaTri { get; set; } = string.Empty;
    public string DonViGiaTri { get; set; } = string.Empty;
    public int ThuTuSapXep { get; set; }
    public decimal DiemToiDa { get; set; }
    public bool TrangThai { get; set; }
    public string? MoTa { get; set; }
    public string? GhiChu { get; set; }
    public List<DanhMucTieuChiDiemMucEntity> Mucs { get; set; } = new();
}

