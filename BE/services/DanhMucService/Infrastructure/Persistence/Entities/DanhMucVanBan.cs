using System.ComponentModel.DataAnnotations;

namespace DanhMucService.Infrastructure.Persistence.Entities
{
    public class DanhMucVanBan : BaseEntity
    {
       
        [Required(ErrorMessage = "Tên loại văn bản không được để trống")]
        public string TenLoaiVanBan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cấp chính quyền không được để trống")]
        public string CapChinhQuyen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chủ thể ban hành không được để trống")]
        public string ChuTheBanHanh { get; set; } = string.Empty;      

        public string? KyHieuMau { get; set; }

        public int ThuTuSapXep { get; set; } = 1;

        public bool TrangThai { get; set; } =true;

        public string? MoTa { get; set; }

        public string? GhiChu { get; set; }
    }

}
