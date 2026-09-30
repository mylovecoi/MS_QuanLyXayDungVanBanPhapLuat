namespace DanhMucService.Application.DTOs.DanhMuc;

public class DanhMucChuyenBuocQuyTrinhDto
{
    public Guid Id { get; set; }
    public Guid QuyTrinhSoanThaoId { get; set; }
    public Guid TuBuocId { get; set; }
    public Guid DenBuocId { get; set; }
    public string TuBuocMa { get; set; } = string.Empty;
    public string DenBuocMa { get; set; } = string.Empty;
    public string DieuKienKetQua { get; set; } = string.Empty;
    public string LoaiChuyenBuoc { get; set; } = "Forward";
    public bool LaNhanhMacDinh { get; set; }
    public bool YeuCauNhapLyDo { get; set; }
    public bool IsKetThuc { get; set; }
    public string? MoTa { get; set; }
    public string? GhiChu { get; set; }
}

