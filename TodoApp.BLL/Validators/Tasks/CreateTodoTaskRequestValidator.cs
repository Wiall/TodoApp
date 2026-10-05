using FluentValidation;
using TodoApp.BLL.Dtos.Tasks;

namespace TodoApp.BLL.Validators.Tasks;

public class CreateTodoTaskRequestValidator : AbstractValidator<CreateTodoTaskRequest>
{
    public CreateTodoTaskRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters");

        RuleFor(request => request.Description)
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters");
    }
}