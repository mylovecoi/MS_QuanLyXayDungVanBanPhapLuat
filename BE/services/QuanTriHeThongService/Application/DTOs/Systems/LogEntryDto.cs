namespace QuanTriHeThongService.Application.DTOs.Systems;

public class LogEntryDto
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? IpAddress { get; set; }
    public string? Url { get; set; }
    public string? Method { get; set; }
    public string? Request { get; set; }
    public DateTime CreatedDate { get; set; }
}

