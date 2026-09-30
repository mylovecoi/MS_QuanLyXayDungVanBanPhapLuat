namespace QuanTriHeThongService.Contracts.Requests.Systems;

public class DuplicateUserAccountApiRequest
{
    public Guid SourceUserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
}

