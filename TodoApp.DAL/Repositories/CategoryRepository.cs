using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Contracts.Interfaces.Repositories;
using TodoApp.Domain.Entities;

namespace TodoApp.DAL.Repositories;

public class CategoryRepository(TodoAppDbContext context) : ICategoryRepository
{
    public async Task<IReadOnlyCollection<Category>> GetAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Categories
            .AsNoTracking()
            .Where(category => category.UserId == userId)
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        return await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(category => category.Id == id && category.UserId == userId, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(Guid userId, string name, Guid? excludedId, CancellationToken cancellationToken)
    {
        return await context.Categories
            .AnyAsync(
                category =>
                    category.UserId == userId &&
                    category.Name == name &&
                    (!excludedId.HasValue || category.Id != excludedId.Value),
                cancellationToken);
    }

    public async Task AddAsync(Category entity, CancellationToken cancellationToken)
    {
        await context.Categories.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Category entity, CancellationToken cancellationToken)
    {
        context.Categories.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Category entity, CancellationToken cancellationToken)
    {
        context.Categories.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
}