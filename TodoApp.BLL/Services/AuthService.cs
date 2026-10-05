using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using TodoApp.BLL.Dtos.Auth;
using TodoApp.BLL.Interfaces;
using TodoApp.BLL.Options;
using TodoApp.Domain.Contracts.Interfaces.Repositories;
using TodoApp.Domain.Entities;

namespace TodoApp.BLL.Services;

public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IJwtTokenService jwtTokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IValidationService validationService,
    IOptions<JwtOptions> options,
    TimeProvider timeProvider) : IAuthService
{
    private readonly JwtOptions jwtOptions = options.Value;

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        await validationService.ValidateAsync(request, cancellationToken);

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(error => error.Description));

            throw new ArgumentException(errors);
        }

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        await validationService.ValidateAsync(request, cancellationToken);

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return null;
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (!signInResult.Succeeded)
        {
            return null;
        }

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponse?> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        await validationService.ValidateAsync(request, cancellationToken);

        var tokenHash = HashToken(request.RefreshToken);

        var currentToken = await refreshTokenRepository
            .GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

        if (currentToken is null)
        {
            return null;
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;

        if (currentToken.RevokedAt.HasValue)
        {
            await refreshTokenRepository.RevokeAllAsync(currentToken.UserId, now, cancellationToken);

            return null;
        }

        if (currentToken.ExpiresAt <= now)
        {
            return null;
        }

        var user = currentToken.User;

        if (user is null)
        {
            return null;
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            return null;
        }

        var replacementToken = CreateRefreshToken(user.Id, now);

        currentToken.RevokedAt = now;

        await refreshTokenRepository.RotateAsync(currentToken, replacementToken.Entity, cancellationToken);

        var accessToken = jwtTokenService.GenerateToken(user);

        return new AuthResponse(
            accessToken.Token,
            replacementToken.Value,
            accessToken.ExpiresAt,
            replacementToken.Entity.ExpiresAt);
    }

    public async Task LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        await validationService.ValidateAsync(request, cancellationToken);

        var tokenHash = HashToken(request.RefreshToken);

        var refreshToken = await refreshTokenRepository
            .GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

        if (refreshToken is null || refreshToken.RevokedAt.HasValue)
        {
            return;
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;

        await refreshTokenRepository.RevokeAsync(refreshToken, now, cancellationToken);
    }

    private async Task<AuthResponse> CreateAuthResponseAsync(User user, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var accessToken = jwtTokenService.GenerateToken(user);

        var refreshToken = CreateRefreshToken(user.Id, now);

        await refreshTokenRepository.AddAsync(refreshToken.Entity, cancellationToken);

        return new AuthResponse(
            accessToken.Token,
            refreshToken.Value,
            accessToken.ExpiresAt,
            refreshToken.Entity.ExpiresAt);
    }

    private (RefreshToken Entity, string Value) CreateRefreshToken(Guid userId, DateTime now)
    {
        var value = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));

        var hash = HashToken(value);

        var entity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = hash,
            CreatedAt = now,
            ExpiresAt = now.AddDays(
                jwtOptions.RefreshTokenExpirationDays)
        };

        return (entity, value);
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(hash);
    }
}