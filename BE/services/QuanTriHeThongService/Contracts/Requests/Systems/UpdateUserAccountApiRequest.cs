namespace QuanTriHeThongService.Contracts.Requests.Systems;

public class UpdateUserAccountApiRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Status { get; set; } = "Kích hoạt";
    public string? Password { get; set; }
    public string? Content { get; set; }
    public Guid GroupPermissionId { get; set; }
}

