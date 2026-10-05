using FluentValidation;
using TodoApp.BLL.Dtos.Auth;

namespace TodoApp.BLL.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is required.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must contain at least 8 characters.");

        RuleFor(request => request.Password)
            .Must(password => password?.Any(char.IsDigit) == true)
            .WithMessage("Password must contain a digit.");

        RuleFor(request => request.Password)
            .Must(password => password?.Any(char.IsUpper) == true)
            .WithMessage("Password must contain an uppercase letter.");

        RuleFor(request => request.Password)
            .Must(password => password?.Any(char.IsLower) == true)
            .WithMessage("Password must contain a lowercase letter.");

        RuleFor(request => request.Password)
            .Must(password => password?.Any(character => !char.IsLetterOrDigit(character)) == true)
            .WithMessage("Password must contain a non-alphanumeric character.");
    }
}
