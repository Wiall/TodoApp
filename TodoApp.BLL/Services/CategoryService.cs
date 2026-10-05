using TodoApp.BLL.Dtos.Categories;
using TodoApp.BLL.Exceptions;
using TodoApp.BLL.Interfaces;
using TodoApp.Domain.Contracts.Interfaces.Repositories;
using TodoApp.Domain.Entities;

namespace TodoApp.BLL.Services;

public class CategoryService(
    ICategoryRepository categoryRepository,
    IValidationService validationService) : ICategoryService
{
    public async Task<IReadOnlyCollection<CategoryResponse>> GetAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(userId, cancellationToken);

        return categories
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<CategoryResponse?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(userId, id, cancellationToken);

        return category is null ? null : MapToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(Guid userId, CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        await validationService.ValidateAsync(request, cancellationToken);

        var nameExists = await categoryRepository.ExistsByNameAsync(userId, request.Name, null, cancellationToken);

        if (nameExists)
        {
            throw new ConflictException("A category with the same name already exists.");
        }

        var entity = new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            Description = request.Description
        };

        await categoryRepository.AddAsync(entity, cancellationToken);

        return MapToResponse(entity);
    }

    public async Task<CategoryResponse?> UpdateAsync(Guid userId, Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        await validationService.ValidateAsync(request, cancellationToken);

        var entity = await categoryRepository.GetByIdAsync(userId, id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        var nameExists = await categoryRepository.ExistsByNameAsync(userId, request.Name, id, cancellationToken);

        if (nameExists)
        {
            throw new ConflictException("A category with the same name already exists.");
        }

        entity.Name = request.Name;
        entity.Description = request.Description;

        await categoryRepository.UpdateAsync(entity, cancellationToken);

        return MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var entity = await categoryRepository.GetByIdAsync(userId, id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        await categoryRepository.DeleteAsync(entity, cancellationToken);

        return true;
    }

    private static CategoryResponse MapToResponse(Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.Name,
            category.Description);
    }
}