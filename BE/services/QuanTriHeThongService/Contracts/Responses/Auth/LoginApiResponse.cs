namespace QuanTriHeThongService.Contracts.Responses.Auth;

public class LoginApiResponse
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public Guid? DonViId { get; set; }
    public Guid GroupPermissionId { get; set; }
    public bool IsSSA { get; set; }
    public bool FirstLogin { get; set; }
    public bool MustChangePassword { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
}

