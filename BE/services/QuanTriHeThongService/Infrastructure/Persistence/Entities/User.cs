using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanTriHeThongService.Infrastructure.Persistence.Entities
{
    public class User : BaseEntity
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
        public bool SSA { get; set; } = false;

        public Guid DanhMucDonViId { get; set; }
        public Guid DoanhNghiepId { get; set; }

        public required string OTPSecretKey { get; set; }
        public required string Status { get; set; } = "Kích hoạt"; //Chờ kích hoạt, Kích hoạt, Khóa
        public bool FirstLogin { get; set; } = false;
        public int LoginCount { get; set; } = 0;

        public string? TenDonViBaoCao { get; set; }
        public string? TenDonViChuQuanBaoCao { get; set; }
        public string? DiaDanh { get; set; }
        public string? ChucDanhKy { get; set; }
        public string? HoTenNguoiKy { get; set; }

        public string? KyHieuDonVi { get; set; }

        public string? Content { get; set; } = "Fixted";
        public string? Menu { get; set; } = "Minimize";
        public string? Theme { get; set; } = "Light";

        public Guid GroupPermissionId { get; set; }

        public bool VNId { get; set; } = false;

        public string? AgentId { get; set; }
        public string? ScanDeviceId { get; set; }
        public string? ScanDeviceName { get; set; }
        public string? Level { get; set; } = "Nhà nước";
    }
}
