using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Contracts.Interfaces.Repositories;

public interface ITodoTaskRepository
{
    IQueryable<TodoTask> Query();

    Task<TodoTask?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
    
    Task<TodoTask?> GetByIdForUpdateAsync(Guid userId, Guid id, CancellationToken cancellationToken);

    Task AddAsync(TodoTask entity, CancellationToken cancellationToken);

    Task UpdateAsync(TodoTask entity, CancellationToken cancellationToken);

    Task DeleteAsync(TodoTask entity, CancellationToken cancellationToken);
}