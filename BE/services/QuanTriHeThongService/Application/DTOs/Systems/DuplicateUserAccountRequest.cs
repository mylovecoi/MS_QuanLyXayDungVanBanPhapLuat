namespace QuanTriHeThongService.Application.DTOs.Systems;

public class DuplicateUserAccountRequest
{
    public Guid SourceUserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

