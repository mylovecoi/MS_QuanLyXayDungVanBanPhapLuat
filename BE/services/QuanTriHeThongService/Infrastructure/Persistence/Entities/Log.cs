using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanTriHeThongService.Infrastructure.Persistence.Entities
{
    public class Log : BaseEntity
    {
        public string? Username { get; set; }
        public string? IpAddress { get; set; }
        public string? Url { get; set; }       
        public string? Method { get; set; }
        public string? Request { get; set; }
    }
}
