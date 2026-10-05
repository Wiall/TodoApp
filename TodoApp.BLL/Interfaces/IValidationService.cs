namespace TodoApp.BLL.Interfaces;

public interface IValidationService
{
    Task ValidateAsync<T>(T model, CancellationToken cancellationToken);
}