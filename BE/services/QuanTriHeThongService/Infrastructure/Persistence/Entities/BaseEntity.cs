using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanTriHeThongService.Infrastructure.Persistence.Entities;

public class BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }

    public Guid UpdatedBy { get; set; }
    public DateTime UpdatedDate { get; set; }
}
