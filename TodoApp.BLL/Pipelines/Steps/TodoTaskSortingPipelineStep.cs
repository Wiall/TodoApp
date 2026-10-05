using TodoApp.BLL.Dtos;
using TodoApp.BLL.Dtos.Queries;
using TodoApp.Domain.Entities;

namespace TodoApp.BLL.Pipelines.Steps;

public class TodoTaskSortingPipelineStep : ITodoTaskQueryPipelineStep
{
    public TodoTaskQueryPipelineStepType Type => TodoTaskQueryPipelineStepType.Sorting;

    public IQueryable<TodoTask> Apply(IQueryable<TodoTask> query, TodoTaskQuery queryObject)
    {
        query = queryObject.SortBy?.ToLowerInvariant() switch
        {
            "title" => queryObject.SortDescending
                ? query.OrderByDescending(task => task.Title).ThenBy(task => task.Id)
                : query.OrderBy(task => task.Title).ThenBy(task => task.Id),

            "createdat" => queryObject.SortDescending
                ? query.OrderByDescending(task => task.CreatedAt).ThenBy(task => task.Id)
                : query.OrderBy(task => task.CreatedAt).ThenBy(task => task.Id),
            
            "dueto" => queryObject.SortDescending
                ? query.OrderByDescending(task => task.DueTo).ThenBy(task => task.Id)
                : query.OrderBy(task => task.DueTo).ThenBy(task => task.Id),

            "isdone" => queryObject.SortDescending
                ? query.OrderByDescending(task => task.IsDone).ThenBy(task => task.Id)
                : query.OrderBy(task => task.IsDone).ThenBy(task => task.Id),

            _ => query.OrderByDescending(task => task.CreatedAt).ThenByDescending(task => task.Id)
        };

        return query;
    }
}