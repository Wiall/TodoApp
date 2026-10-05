using TodoApp.BLL.Dtos;
using TodoApp.BLL.Dtos.Queries;
using TodoApp.Domain.Entities;

namespace TodoApp.BLL.Pipelines.Steps;

public interface ITodoTaskQueryPipelineStep
{
    TodoTaskQueryPipelineStepType Type { get; }

    IQueryable<TodoTask> Apply(IQueryable<TodoTask> query, TodoTaskQuery queryObject);
}