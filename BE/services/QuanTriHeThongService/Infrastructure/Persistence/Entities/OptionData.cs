using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanTriHeThongService.Infrastructure.Persistence.Entities
{
    public class OptionData : BaseEntity
    {
        public string? Code { get; set; }
        public string? DisplayName { get; set; }
        public string? Value { get; set; }
        public string? MoTa { get; set; }
    }
}
