using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.BLL.Interfaces;

namespace TodoApp.BLL.Services;

public class ValidationService(IServiceProvider serviceProvider) : IValidationService
{
    public async Task ValidateAsync<T>(T model, CancellationToken cancellationToken)
    {
        var validator = serviceProvider.GetRequiredService<IValidator<T>>();

        await validator.ValidateAndThrowAsync(model, cancellationToken);
    }
}