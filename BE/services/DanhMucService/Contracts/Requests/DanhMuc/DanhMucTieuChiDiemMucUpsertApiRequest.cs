namespace DanhMucService.Contracts.Requests.DanhMuc;

public class DanhMucTieuChiDiemMucUpsertApiRequest
{
    public Guid? Id { get; set; }
    public decimal? TuGiaTri { get; set; }
    public decimal? DenGiaTri { get; set; }
    public bool BaoGomTuGiaTri { get; set; } = true;
    public bool BaoGomDenGiaTri { get; set; } = true;
    public decimal Diem { get; set; }
    public string? NhanHienThi { get; set; }
    public int ThuTuSapXep { get; set; } = 1;
    public bool TrangThai { get; set; } = true;
    public string? GhiChu { get; set; }
}

