using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanTriHeThongService.Infrastructure.Persistence.Entities
{
    public class GroupPermision : BaseEntity
    {
        [Required(ErrorMessage = "Nhóm quyền không được để trống")]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Status { get; set; } = "Kích hoạt"; //Dừng kích hoạt, Kích hoạt
        [NotMapped]
        public List<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
