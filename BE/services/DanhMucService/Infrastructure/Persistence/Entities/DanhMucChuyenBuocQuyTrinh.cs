using System.ComponentModel.DataAnnotations;

namespace DanhMucService.Infrastructure.Persistence.Entities
{
    public class DanhMucChuyenBuocQuyTrinh : BaseEntity
    {
        public Guid QuyTrinhSoanThaoId { get; set; }

        public Guid TuBuocId { get; set; }

        public Guid DenBuocId { get; set; }

        [Required(ErrorMessage = "Dieu kien ket qua khong duoc de trong")]
        public string DieuKienKetQua { get; set; } = string.Empty;

        public string LoaiChuyenBuoc { get; set; } = "Forward";

        public bool LaNhanhMacDinh { get; set; } = false;

        public bool YeuCauNhapLyDo { get; set; } = false;

        public bool IsKetThuc { get; set; } = false;

        public string? MoTa { get; set; }

        public string? GhiChu { get; set; }
    }
}
