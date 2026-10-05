using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Contracts.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyCollection<Category>> GetAllAsync(Guid userId, CancellationToken cancellationToken);

    Task<Category?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(Guid userId, string name, Guid? excludedId, CancellationToken cancellationToken);

    Task AddAsync(Category entity, CancellationToken cancellationToken);

    Task UpdateAsync(Category entity, CancellationToken cancellationToken);

    Task DeleteAsync(Category entity, CancellationToken cancellationToken);
}