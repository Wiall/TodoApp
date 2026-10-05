using Microsoft.AspNetCore.Identity;

namespace TodoApp.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public ICollection<TodoTask> Tasks { get; set; } = new List<TodoTask>();

    public ICollection<Category> Categories { get; set; } = new List<Category>();
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}