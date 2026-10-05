using TodoApp.Domain.Entities;

namespace TodoApp.BLL.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}