using DanhMucService.Infrastructure.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanhMucService.Infrastructure.Persistence.Entities
{
    public class DanhMucCanBo : BaseEntity
    {
        [Required(ErrorMessage = "Cần chọn đơn vị quản lý")]
        public Guid DonViQuanLyId { get; set; }

        [ForeignKey(nameof(DonViQuanLyId))]
        public virtual DanhMucDonVi? DonViQuanLy { get; set; }

        [Required(ErrorMessage = "Tên cán bộ không được để trống")]
        [DisplayName("Tên cán bộ")]
        public string TenCanBo { get; set; } = string.Empty;

        [DisplayName("Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string Username { get; set; } = string.Empty;

        [NotMapped]
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [NotMapped]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = string.Empty;

        [NotMapped]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [NotMapped]
        public string Status { get; set; } = "Kích hoạt";

        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Cần chọn phòng ban")]
        public Guid PhongBanId { get; set; }

        [ForeignKey(nameof(PhongBanId))]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual DanhMucPhongBan? PhongBan { get; set; }

        public bool GioiTinh { get; set; } = false;

        [Required(ErrorMessage = "Trình độ chuyên môn không được để trống")]
        public string TrinhDoChuyenMon { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chọn 1 loại lao động")]
        public required LoaiLaoDong LoaiLaoDong { get; set; }

        [DisplayName("Số tiền BHXH đã nộp")]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal SoTienBHXH { get; set; }

        [DisplayName("Số tiền BHYT đã nộp")]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal SoTienBHYT { get; set; }

        public string? SoQuyetDinhDung { get; set; }
        public DateTime? NgayQuyetDinhDung { get; set; }

        [DisplayName("Ghi chú")]
        public string? GhiChu { get; set; }

        // Các trường riêng cho công chứng viên
        [DisplayName("Số Quyết định bổ nhiệm")]
        public string? SoQuyetDinhBoNhiem { get; set; }

        [DisplayName("Ngày ban hành Quyết định bổ nhiệm")]
        public DateTime? NgayQuyetDinhBoNhiem { get; set; }

        [DisplayName("Số Quyết định cấp Thẻ")]
        public string? SoQuyetDinhCapThe { get; set; }

        [DisplayName("Ngày ban hành Quyết định cấp Thẻ")]
        public DateTime? NgayQuyetDinhCapThe { get; set; }

        [DisplayName("Số Thẻ công chứng viên")]
        public string? SoTheCongChungVien { get; set; }

        [DisplayName("Chức vụ")]
        public string? ChucVu { get; set; }

        [DisplayName("Mức phí bảo hiểm trách nhiệm nghề nghiệp")]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? MucPhiBaoHiemTrachNhiem { get; set; }

        // Các trường riêng cho nhân viên nghiệp vụ và nhân viên khác
        [DisplayName("Vị trí việc làm")]
        public string? ViTriViecLam { get; set; }

        [DisplayName("Ngày được tuyển dụng")]
        public DateTime? NgayTuyenDung { get; set; }

        [DisplayName("Số hợp đồng lao động")]
        public string? SoHopDongLaoDong { get; set; }

        [DisplayName("Ngày ký hợp đồng lao động")]
        public DateTime? NgayKyHopDongLaoDong { get; set; }
    }
}