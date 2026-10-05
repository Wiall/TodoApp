using TodoApp.BLL.Dtos;
using TodoApp.BLL.Dtos.Queries;
using TodoApp.BLL.Pipelines.Steps;
using TodoApp.Domain.Entities;

namespace TodoApp.BLL.Pipelines;

public class TodoTaskQueryPipeline(IEnumerable<ITodoTaskQueryPipelineStep> steps)
{
    public IQueryable<TodoTask> ApplyFiltering(IQueryable<TodoTask> query, TodoTaskQuery queryObject)
        => ApplySteps(query, queryObject, TodoTaskQueryPipelineStepType.Filtering);
    

    public IQueryable<TodoTask> ApplySorting(IQueryable<TodoTask> query, TodoTaskQuery queryObject)
        =>  ApplySteps(query, queryObject, TodoTaskQueryPipelineStepType.Sorting);

    public IQueryable<TodoTask> ApplyPagination(IQueryable<TodoTask> query, TodoTaskQuery queryObject)
        =>  ApplySteps(query, queryObject, TodoTaskQueryPipelineStepType.Pagination);

    private IQueryable<TodoTask> ApplySteps(
        IQueryable<TodoTask> query, TodoTaskQuery queryObject, TodoTaskQueryPipelineStepType type)
    {
        foreach (var step in steps.Where(x => x.Type == type))
        {
            query = step.Apply(query, queryObject);
        }

        return query;
    }
}