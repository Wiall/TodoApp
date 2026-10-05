using TodoApp.BLL.Dtos.Categories;

namespace TodoApp.BLL.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyCollection<CategoryResponse>> GetAllAsync(Guid userId, CancellationToken cancellationToken);

    Task<CategoryResponse?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);

    Task<CategoryResponse> CreateAsync(Guid userId, CreateCategoryRequest request, CancellationToken cancellationToken);

    Task<CategoryResponse?> UpdateAsync(Guid userId, Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken);
}