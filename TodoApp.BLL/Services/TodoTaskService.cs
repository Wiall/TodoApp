using Microsoft.EntityFrameworkCore;
using TodoApp.BLL.Dtos;
using TodoApp.BLL.Dtos.Queries;
using TodoApp.BLL.Dtos.Tasks;
using TodoApp.BLL.Exceptions;
using TodoApp.BLL.Interfaces;
using TodoApp.BLL.Pipelines;
using TodoApp.Domain.Contracts.Interfaces.Repositories;
using TodoApp.Domain.Entities;

namespace TodoApp.BLL.Services;

public class TodoTaskService(
    ITodoTaskRepository todoTaskRepository,
    ICategoryRepository categoryRepository,
    TodoTaskQueryPipeline queryPipeline,
    IValidationService validationService,
    TimeProvider timeProvider) : ITodoTaskService
{
    public async Task<PagedResponse<TodoTaskResponse>> GetAllAsync(
        Guid userId, TodoTaskQuery query, CancellationToken cancellationToken)
    {
        var taskQuery = todoTaskRepository
            .Query()
            .Where(task => task.UserId == userId);

        taskQuery = queryPipeline.ApplyFiltering(taskQuery, query);

        var totalCount = await taskQuery.CountAsync(cancellationToken);

        taskQuery = queryPipeline.ApplySorting(taskQuery, query);
        taskQuery = queryPipeline.ApplyPagination(taskQuery, query);

        var items = await taskQuery
            .Select(task => new TodoTaskResponse(
                task.Id,
                task.Title,
                task.Description,
                task.IsDone,
                task.DueTo,
                task.CreatedAt,
                task.CategoryId,
                task.Category != null ? task.Category.Name : null))
            .ToListAsync(cancellationToken);

        var page = query.NormalizedPage;
        var pageSize = query.NormalizedPageSize;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResponse<TodoTaskResponse>(
            items, page, pageSize, totalCount, totalPages);
    }

    public async Task<TodoTaskResponse?> GetByIdAsync(
        Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var entity = await todoTaskRepository.GetByIdAsync(
            userId, id, cancellationToken);

        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<TodoTaskResponse> CreateAsync(
        Guid userId, CreateTodoTaskRequest request, CancellationToken cancellationToken)
    {
        await validationService.ValidateAsync(request, cancellationToken);

        string? categoryName = null;

        if (request.CategoryId.HasValue)
        {
            var category = await categoryRepository.GetByIdAsync(
                userId, request.CategoryId.Value, cancellationToken);

            if (category is null)
            {
                throw new EntityNotFoundException(
                    "The specified category does not exist.");
            }

            categoryName = category.Name;
        }

        var entity = new TodoTask
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            IsDone = false,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
            DueTo = request.DueTo,
        };

        await todoTaskRepository.AddAsync(entity, cancellationToken);

        return MapToResponse(entity, categoryName);
    }

    public async Task<TodoTaskResponse?> UpdateAsync(
        Guid userId, Guid id, UpdateTodoTaskRequest request, CancellationToken cancellationToken)
    {
        await validationService.ValidateAsync(request, cancellationToken);

        var entity = await todoTaskRepository.GetByIdForUpdateAsync(userId, id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        string? categoryName = null;

        if (request.CategoryId.HasValue)
        {
            var category = await categoryRepository.GetByIdAsync(userId, request.CategoryId.Value, cancellationToken);

            if (category is null)
            {
                throw new EntityNotFoundException("The specified category does not exist.");
            }

            categoryName = category.Name;
        }

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.IsDone = request.IsDone;
        entity.DueTo = request.DueTo;
        entity.CategoryId = request.CategoryId;

        await todoTaskRepository.UpdateAsync(entity, cancellationToken);

        return MapToResponse(entity, categoryName);
    }

    public async Task<bool> DeleteAsync(
        Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var entity = await todoTaskRepository.GetByIdAsync(userId, id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        await todoTaskRepository.DeleteAsync(entity, cancellationToken);

        return true;
    }

    private static TodoTaskResponse MapToResponse(TodoTask task)
    {
        return new TodoTaskResponse(
            task.Id,
            task.Title,
            task.Description,
            task.IsDone,
            task.DueTo,
            task.CreatedAt,
            task.CategoryId,
            task.Category?.Name);
    }

    private static TodoTaskResponse MapToResponse(TodoTask task, string? categoryName)
    {
        return new TodoTaskResponse(
            task.Id,
            task.Title,
            task.Description,
            task.IsDone,
            task.DueTo,
            task.CreatedAt,
            task.CategoryId,
            categoryName);
    }
}