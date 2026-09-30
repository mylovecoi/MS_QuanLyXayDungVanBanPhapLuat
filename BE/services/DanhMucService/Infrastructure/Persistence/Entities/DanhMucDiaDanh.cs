using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanhMucService.Infrastructure.Persistence.Entities
{
    public class DanhMucDiaDanh : BaseEntity
    {
        public required string TenDiaDanh { get; set; }
        public int Level { get; set; } = 0;
        public int STTSapXep { get; set; } = 1;
        public Guid DiaDanhCapTrenId { get; set; }
        [NotMapped]
        public string? TenDiaDanhChuQuan { get; set; } 
    }
}
