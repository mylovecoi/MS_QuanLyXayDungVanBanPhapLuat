namespace DanhMucService.Application.DTOs.DanhMuc;

public class DanhMucTieuChiDiemMucDto
{
    public Guid Id { get; set; }
    public Guid DanhMucTieuChiDiemId { get; set; }
    public decimal? TuGiaTri { get; set; }
    public decimal? DenGiaTri { get; set; }
    public bool BaoGomTuGiaTri { get; set; }
    public bool BaoGomDenGiaTri { get; set; }
    public decimal Diem { get; set; }
    public string? NhanHienThi { get; set; }
    public int ThuTuSapXep { get; set; }
    public bool TrangThai { get; set; }
    public string? GhiChu { get; set; }
}

