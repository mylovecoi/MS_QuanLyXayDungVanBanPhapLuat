using DanhMucService.Domain.Common;

namespace DanhMucService.Domain.Entities.DanhMuc;

public class DanhMucBuocQuyTrinhEntity : BaseEntity
{
    public Guid QuyTrinhSoanThaoId { get; set; }
    public string MaBuoc { get; set; } = string.Empty;
    public string TenBuoc { get; set; } = string.Empty;
    public int ThuTuSapXep { get; set; } = 1;
    public string LoaiBuoc { get; set; } = "XuLy";
    public bool BatBuoc { get; set; } = true;
    public bool ChoPhepBoQua { get; set; }
    public bool ChoPhepQuayLui { get; set; }
    public string? CachHoanThanh { get; set; }
    public int? SoLuongPhanHoiToiThieu { get; set; }
    public bool YeuCauFileDinhKem { get; set; }
    public int SoLanTraLaiToiDa { get; set; }
    public int? SoNgayXuLyTieuChuan { get; set; }
    public int? SoNgayCanhBaoSapHan { get; set; }
    public Guid? DonViTiepNhanMacDinhId { get; set; }
    public string? TenDonViTiepNhanMacDinh { get; set; }
    public string? MoTa { get; set; }
    public string? GhiChu { get; set; }
}

