namespace TodoApp.BLL.Dtos.Categories;

public record CategoryResponse(
    Guid Id,
    string Name,
    string? Description);