namespace TodoApp.BLL.Dtos.Auth;

public record RegisterRequest(
    string Email,
    string Password);