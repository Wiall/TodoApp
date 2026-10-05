namespace TodoApp.BLL.Dtos.Tasks;

public record UpdateTodoTaskRequest(
    string Title,
    string? Description,
    bool IsDone,
    DateTime? DueTo,
    Guid? CategoryId);