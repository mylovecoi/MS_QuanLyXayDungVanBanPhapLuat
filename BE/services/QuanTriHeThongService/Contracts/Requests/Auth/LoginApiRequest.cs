namespace QuanTriHeThongService.Contracts.Requests.Auth;

public class LoginApiRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

