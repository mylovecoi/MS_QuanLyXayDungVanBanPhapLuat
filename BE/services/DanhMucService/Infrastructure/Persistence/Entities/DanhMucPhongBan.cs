using DanhMucService.Infrastructure.Persistence.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanhMucService.Infrastructure.Persistence.Entities
{
    public class DanhMucPhongBan : BaseEntity
    {
        [Required(ErrorMessage = "Tên phòng ban không được để trống")]
        public string TenPhongBan { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Mã phòng ban không được để trống")]
        public string MaPhongBan { get; set; } = string.Empty;
        
        public LoaiPhongBan LoaiPhongBan { get; set; }
        
        [ForeignKey(nameof(DanhMucDonVi))]
        [Required(ErrorMessage = "Cần chọn đơn vị")]
        public Guid DanhMucDonViId { get; set; }
        
        public DanhMucDonVi? DanhMucDonVi { get; set; }
    }
} 