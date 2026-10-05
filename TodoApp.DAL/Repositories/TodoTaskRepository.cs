using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Contracts.Interfaces.Repositories;
using TodoApp.Domain.Entities;

namespace TodoApp.DAL.Repositories;

public class TodoTaskRepository(TodoAppDbContext context) : ITodoTaskRepository
{
    public IQueryable<TodoTask> Query()
    {
        return context.Tasks
            .AsNoTracking()
            .Include(task => task.Category);
    }

    public async Task<TodoTask?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        return await context.Tasks
            .AsNoTracking()
            .Include(task => task.Category)
            .FirstOrDefaultAsync(
                task => task.Id == id && task.UserId == userId,
                cancellationToken);
    }
    
    public async Task<TodoTask?> GetByIdForUpdateAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        return await context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(
                task => task.Id == id && task.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(TodoTask entity, CancellationToken cancellationToken)
    {
        await context.Tasks.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TodoTask entity, CancellationToken cancellationToken)
    {
        context.Tasks.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TodoTask entity, CancellationToken cancellationToken)
    {
        context.Tasks.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
}