using TodoApp.BLL.Dtos.Auth;
using TodoApp.Domain.Entities;

namespace TodoApp.BLL.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);

    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    Task<AuthResponse?> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken);

    Task LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
}