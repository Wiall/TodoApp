namespace TodoApp.BLL.Dtos.Categories;

public record CreateCategoryRequest(
    string Name,
    string? Description);