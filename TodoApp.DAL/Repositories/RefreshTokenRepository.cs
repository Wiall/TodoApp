using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Contracts.Interfaces.Repositories;
using TodoApp.Domain.Entities;

namespace TodoApp.DAL.Repositories;

public class RefreshTokenRepository(
    TodoAppDbContext dbContext) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await dbContext.RefreshTokens
            .Include(refreshToken => refreshToken.User)
            .FirstOrDefaultAsync(refreshToken => refreshToken.TokenHash == tokenHash, cancellationToken);
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RotateAsync(RefreshToken currentToken, RefreshToken replacementToken, CancellationToken cancellationToken)
    {
        await dbContext.RefreshTokens.AddAsync(replacementToken, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAsync(RefreshToken refreshToken, DateTime revokedAt, CancellationToken cancellationToken)
    {
        refreshToken.RevokedAt = revokedAt;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllAsync(Guid userId, DateTime revokedAt, CancellationToken cancellationToken)
    {
        await dbContext.RefreshTokens
            .Where(refreshToken =>
                refreshToken.UserId == userId && refreshToken.RevokedAt == null)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(refreshToken => refreshToken.RevokedAt, revokedAt),
                cancellationToken);
    }
}