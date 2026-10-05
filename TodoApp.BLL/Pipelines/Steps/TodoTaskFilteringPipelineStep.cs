using TodoApp.BLL.Dtos;
using TodoApp.BLL.Dtos.Queries;
using TodoApp.Domain.Entities;

namespace TodoApp.BLL.Pipelines.Steps;

public class TodoTaskFilteringPipelineStep : ITodoTaskQueryPipelineStep
{
    public TodoTaskQueryPipelineStepType Type => TodoTaskQueryPipelineStepType.Filtering;

    public IQueryable<TodoTask> Apply(IQueryable<TodoTask> query, TodoTaskQuery queryObject)
    {
        query = ApplySearchFilter(query, queryObject);
        query = ApplyCategoryFilter(query, queryObject);
        query = ApplyStatusFilter(query, queryObject);
        query = ApplyDueToFilter(query, queryObject);

        return query;
    }

    private static IQueryable<TodoTask> ApplySearchFilter(IQueryable<TodoTask> query, TodoTaskQuery queryObject)
    {
        if (!string.IsNullOrWhiteSpace(queryObject.Search))
        {
            query = query.Where(task => task.Title.Contains(queryObject.Search) ||
                                        (task.Description != null && task.Description.Contains(queryObject.Search)));
        }

        return query;
    }

    private static IQueryable<TodoTask> ApplyCategoryFilter(IQueryable<TodoTask> query, TodoTaskQuery queryObject)
    {
        if (queryObject.WithoutCategory)
        {
            return query.Where(
                task => task.CategoryId == null);
        }
        
        if (queryObject.CategoryId.HasValue)
        {
            query = query.Where(task => task.CategoryId == queryObject.CategoryId.Value);
        }

        return query;
    }

    private static IQueryable<TodoTask> ApplyStatusFilter(IQueryable<TodoTask> query, TodoTaskQuery queryObject)
    {
        if (queryObject.IsDone.HasValue)
        {
            query = query.Where(task => task.IsDone == queryObject.IsDone.Value);
        }

        return query;
    }
    
    private static IQueryable<TodoTask> ApplyDueToFilter(IQueryable<TodoTask> query, TodoTaskQuery queryObject)
    {
        if (queryObject.DueToStart.HasValue)
        {
            query = query.Where(task => task.DueTo >= queryObject.DueToStart.Value);
        }
        
        if (queryObject.DueToEnd.HasValue)
        {
            query = query.Where(task => task.DueTo <= queryObject.DueToEnd.Value);
        }

        return query;
    }
}