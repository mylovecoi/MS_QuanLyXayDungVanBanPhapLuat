using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanTriHeThongService.Infrastructure.Persistence.Entities
{
    public class Permission : BaseEntity
    {
        public Guid GroupPermissionId { get; set; }
        public Guid RoleActionId { get; set; }
        public string? Status { get; set; }
        public bool Index { get; set; } = false;
        public bool Create { get; set; } = false;
        public bool Edit { get; set; } = false;
        public bool Delete { get; set; } = false;
        public bool Approve { get; set; } = false;
        public bool Public { get; set; } = false;

        [NotMapped]
        public string? PhanLoai { get; set; }
        [NotMapped]
        public Guid RoleActionGroupId { get; set; }
        [NotMapped]
        public int Level { get; set; }
        [NotMapped]
        public int STTSapXep { get; set; }

        [NotMapped]
        public string? Title { get; set; }
        [NotMapped]
        public string? Role { get; set; }
        [NotMapped]
        public string? MenuActive { get; set; }
        [NotMapped]
        public string? Controller { get; set; }
        [NotMapped]
        public string? Action { get; set; }
        [NotMapped]
        public string? Parameter { get; set; }
        [NotMapped]
        public string? TitleGroupRole { get; set; }
        [NotMapped]
        public string? Table { get; set; }
        [NotMapped]
        public string? Icon { get; set; }

    }
}
