namespace TodoApp.BLL.Dtos.Auth;

public record LoginRequest(
    string Email,
    string Password);