using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Contracts.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

    Task RotateAsync(RefreshToken currentToken, RefreshToken replacementToken, CancellationToken cancellationToken);

    Task RevokeAsync(RefreshToken refreshToken, DateTime revokedAt, CancellationToken cancellationToken);

    Task RevokeAllAsync(Guid userId, DateTime revokedAt, CancellationToken cancellationToken);
}