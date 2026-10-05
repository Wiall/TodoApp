namespace TodoApp.BLL.Dtos.Tasks;

public record TodoTaskResponse(
    Guid Id,
    string Title,
    string? Description,
    bool IsDone,
    DateTime? DueTo,
    DateTime CreatedAt,
    Guid? CategoryId,
    string? CategoryName );