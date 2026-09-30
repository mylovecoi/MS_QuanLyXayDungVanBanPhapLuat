using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanTriHeThongService.Infrastructure.Persistence.Entities
{
    public class QuestionAnswer : BaseEntity
    {
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public string? Description { get; set; }        
    }   
}
