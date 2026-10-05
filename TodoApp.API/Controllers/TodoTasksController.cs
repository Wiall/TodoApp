using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.BLL.Dtos.Queries;
using TodoApp.BLL.Dtos.Tasks;
using TodoApp.BLL.Interfaces;

namespace TodoApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TodoTasksController(
    ITodoTaskService todoTaskService,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TodoTaskQuery query, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();

        var response = await todoTaskService.GetAllAsync(userId, query, cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();

        var task = await todoTaskService.GetByIdAsync(userId, id, cancellationToken);

        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTodoTaskRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();

        try
        {
            var created = await todoTaskService.CreateAsync(userId, request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateTodoTaskRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();

        try
        {
            var updated = await todoTaskService.UpdateAsync(userId, id, request, cancellationToken);

            if (updated is null)
            {
                return NotFound();
            }

            return Ok(updated);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();

        var deleted = await todoTaskService.DeleteAsync(userId, id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}