namespace TodoApp.BLL.Dtos.Tasks;

public record CreateTodoTaskRequest(
    string Title,
    string? Description,
    DateTime? DueTo,
    Guid? CategoryId );