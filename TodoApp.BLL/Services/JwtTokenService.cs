using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApp.BLL.Interfaces;
using TodoApp.BLL.Options;
using TodoApp.Domain.Entities;

namespace TodoApp.BLL.Services;

public class JwtTokenService(IOptions<JwtOptions> options, TimeProvider timeProvider) : IJwtTokenService
{
    private readonly JwtOptions jwtOptions = options.Value;

    public (string Token, DateTime ExpiresAt) GenerateToken(User user)
    {
        var now = timeProvider.GetUtcNow();

        var expiresAt = now.AddMinutes(jwtOptions.AccessTokenExpirationMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),

            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return (tokenValue, expiresAt.UtcDateTime);
    }
}