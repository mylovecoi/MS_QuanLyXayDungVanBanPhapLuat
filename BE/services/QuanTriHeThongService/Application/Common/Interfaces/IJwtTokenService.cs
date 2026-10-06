using QuanTriHeThongService.Infrastructure.Persistence.Entities;

namespace QuanTriHeThongService.Application.Common.Interfaces;

public interface IJwtTokenService
{
    (string AccessToken, DateTime ExpiresAt) CreateAccessToken(User user);
}
