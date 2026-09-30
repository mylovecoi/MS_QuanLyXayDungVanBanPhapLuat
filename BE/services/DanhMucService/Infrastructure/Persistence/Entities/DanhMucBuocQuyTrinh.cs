using System.ComponentModel.DataAnnotations;

namespace DanhMucService.Infrastructure.Persistence.Entities
{
    public class DanhMucBuocQuyTrinh : BaseEntity
    {
        public Guid QuyTrinhSoanThaoId { get; set; }

        [Required(ErrorMessage = "Ma buoc khong duoc de trong")]
        public string MaBuoc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ten buoc khong duoc de trong")]
        public string TenBuoc { get; set; } = string.Empty;

        public int ThuTuSapXep { get; set; } = 1;

        [Required(ErrorMessage = "Loai buoc khong duoc de trong")]
        public string LoaiBuoc { get; set; } = string.Empty;

        public bool BatBuoc { get; set; } = true;

        public bool ChoPhepBoQua { get; set; } = false;

        public bool ChoPhepQuayLui { get; set; } = false;

        public string? CachHoanThanh { get; set; }

        public int? SoLuongPhanHoiToiThieu { get; set; }

        public bool YeuCauFileDinhKem { get; set; } = false;

        public int SoLanTraLaiToiDa { get; set; } = 0;

        public int? SoNgayXuLyTieuChuan { get; set; }

        public int? SoNgayCanhBaoSapHan { get; set; }

        public Guid? DonViTiepNhanMacDinhId { get; set; }

        public string? MoTa { get; set; }

        public string? GhiChu { get; set; }
    }
}
