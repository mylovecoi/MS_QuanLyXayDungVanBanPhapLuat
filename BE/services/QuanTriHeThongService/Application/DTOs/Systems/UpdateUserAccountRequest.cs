namespace QuanTriHeThongService.Application.DTOs.Systems;

public class UpdateUserAccountRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = "Kích hoạt";
    public string? Password { get; set; }
    public string? Content { get; set; } = "Fixted";
    public Guid GroupPermissionId { get; set; }
}

