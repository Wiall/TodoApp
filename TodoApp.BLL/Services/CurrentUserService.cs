using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TodoApp.BLL.Interfaces;

namespace TodoApp.BLL.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid GetCurrentUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var id))
        {
            throw new UnauthorizedAccessException();
        }

        return id;
    }
}