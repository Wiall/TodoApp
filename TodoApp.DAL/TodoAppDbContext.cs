using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;

namespace TodoApp.DAL;

public class TodoAppDbContext 
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public TodoAppDbContext(DbContextOptions<TodoAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoTask> Tasks { get; set; }

    public DbSet<Category> Categories  { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(TodoAppDbContext).Assembly);
    }
}