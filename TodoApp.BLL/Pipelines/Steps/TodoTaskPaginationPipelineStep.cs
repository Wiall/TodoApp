using TodoApp.BLL.Dtos.Queries;
using TodoApp.Domain.Entities;

namespace TodoApp.BLL.Pipelines.Steps;

public class TodoTaskPaginationPipelineStep : ITodoTaskQueryPipelineStep
{
    public TodoTaskQueryPipelineStepType Type => TodoTaskQueryPipelineStepType.Pagination;

    public IQueryable<TodoTask> Apply(IQueryable<TodoTask> query, TodoTaskQuery queryObject)
    {
        return query.Skip((queryObject.NormalizedPage - 1) *
                          queryObject.NormalizedPageSize)
                    .Take(queryObject.NormalizedPageSize);
    }
}