namespace DanhMucService.Application.DTOs.DanhMuc;

public class UpsertDanhMucChuyenBuocQuyTrinhRequest
{
    public Guid Id { get; set; }
    public string TuBuocMa { get; set; } = string.Empty;
    public string DenBuocMa { get; set; } = string.Empty;
    public string DieuKienKetQua { get; set; } = string.Empty;
    public string LoaiChuyenBuoc { get; set; } = "Forward";
    public bool LaNhanhMacDinh { get; set; } = true;
    public bool YeuCauNhapLyDo { get; set; }
    public bool IsKetThuc { get; set; }
    public string? MoTa { get; set; }
    public string? GhiChu { get; set; }
}

