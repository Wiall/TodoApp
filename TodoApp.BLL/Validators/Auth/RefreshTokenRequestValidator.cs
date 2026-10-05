using FluentValidation;
using TodoApp.BLL.Dtos.Auth;

namespace TodoApp.BLL.Validators.Auth;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(request => request.RefreshToken)
            .NotEmpty();
    }
}
