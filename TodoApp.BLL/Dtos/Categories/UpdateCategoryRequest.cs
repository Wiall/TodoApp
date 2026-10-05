namespace TodoApp.BLL.Dtos.Categories;

public record UpdateCategoryRequest(
    string Name,
    string? Description);