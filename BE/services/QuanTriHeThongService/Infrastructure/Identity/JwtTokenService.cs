using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BuildingBlocks.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuanTriHeThongService.Application.Common.Interfaces;
using QuanTriHeThongService.Infrastructure.Persistence.Entities;

namespace QuanTriHeThongService.Infrastructure.Identity;

public sealed class JwtTokenService(IOptions<JwtSettings> options) : IJwtTokenService
{
    private readonly JwtSettings _settings = options.Value;

    public (string AccessToken, DateTime ExpiresAt) CreateAccessToken(User user)
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
        {
            throw new InvalidOperationException("Thiếu cấu hình JwtSettings:SecretKey.");
        }

        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_settings.ExpirationMinutes <= 0 ? 60 : _settings.ExpirationMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(ClaimTypes.Name, user.Username),
            new("UserId", user.Id.ToString()),
            new("GroupPermissionId", user.GroupPermissionId.ToString()),
            new("IsSSA", user.SSA.ToString())
        };

        if (user.DanhMucDonViId != Guid.Empty)
        {
            claims.Add(new Claim("DonViId", user.DanhMucDonViId.ToString()));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: string.IsNullOrWhiteSpace(_settings.Issuer) ? null : _settings.Issuer,
            audience: string.IsNullOrWhiteSpace(_settings.Audience) ? null : _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
