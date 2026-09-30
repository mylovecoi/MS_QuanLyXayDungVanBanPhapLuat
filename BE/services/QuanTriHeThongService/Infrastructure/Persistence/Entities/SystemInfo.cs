using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanTriHeThongService.Infrastructure.Persistence.Entities
{
    public class SystemInfo : BaseEntity
    {
        public string? AppName { get; set; } = "Giải pháp phần mềm ";
        public string? Copyright { get; set; } = "LifeSoftware";
        public DateTime MfgDate { get; set; } = DateTime.Now;
        public DateTime ExpDate { get; set; } = DateTime.Now.AddYears(1);
        public int LoginLock { get; set; } = 5;
        public bool Train { get; set; } = false;
        public bool IsChatBot { get; set; } = false;
        public bool IsOPT { get; set; } = false;
        public string MenuLayout { get; set; } = "vertical";
    }
}
