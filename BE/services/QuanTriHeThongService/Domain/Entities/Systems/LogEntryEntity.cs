using QuanTriHeThongService.Domain.Common;

namespace QuanTriHeThongService.Domain.Entities.Systems;

public class LogEntryEntity : BaseEntity
{
    public DateTime CreatedDate { get; set; }
    public string? Username { get; set; }
    public string? IpAddress { get; set; }
    public string? Url { get; set; }
    public string? Method { get; set; }
    public string? Request { get; set; }
}

