using FluentValidation;
using TodoApp.BLL.Dtos.Categories;

namespace TodoApp.BLL.Validators.Categories;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");

        RuleFor(request => request.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters");
    }
}