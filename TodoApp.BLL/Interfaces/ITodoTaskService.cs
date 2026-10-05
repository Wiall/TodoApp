using TodoApp.BLL.Dtos;
using TodoApp.BLL.Dtos.Queries;
using TodoApp.BLL.Dtos.Tasks;

namespace TodoApp.BLL.Interfaces;

public interface ITodoTaskService
{
    Task<PagedResponse<TodoTaskResponse>> GetAllAsync(Guid userId, TodoTaskQuery queryObject, CancellationToken cancellationToken);

    Task<TodoTaskResponse?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);

    Task<TodoTaskResponse> CreateAsync(Guid userId, CreateTodoTaskRequest request, CancellationToken cancellationToken);

    Task<TodoTaskResponse?> UpdateAsync(Guid userId, Guid id, UpdateTodoTaskRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken);
}