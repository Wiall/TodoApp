namespace TodoApp.BLL.Dtos.Queries;

public record TodoTaskQuery(
    string? Search = null,
    Guid? CategoryId = null,
    bool? IsDone = null,
    DateTime? DueToStart = null,
    DateTime? DueToEnd = null,
    string? SortBy = null,
    bool SortDescending = false,
    int Page = 1,
    int PageSize = 10,
    bool WithoutCategory = false)
{
    public int NormalizedPage => Math.Max(Page, 1);
    public int NormalizedPageSize => Math.Clamp(PageSize, 1, 100);
}