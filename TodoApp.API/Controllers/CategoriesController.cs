using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.BLL.Dtos.Categories;
using TodoApp.BLL.Interfaces;

namespace TodoApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/categories")]
public class CategoriesController(
    ICategoryService categoryService,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();

        var categories = await categoryService.GetAllAsync(userId, cancellationToken);

        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();

        var category = await categoryService.GetByIdAsync(userId, id, cancellationToken);

        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();

        try
        {
            var created = await categoryService.CreateAsync(userId, request, cancellationToken);

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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();

        try
        {
            var updated = await categoryService.UpdateAsync(userId, id, request, cancellationToken);

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

        var deleted = await categoryService.DeleteAsync(userId, id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}