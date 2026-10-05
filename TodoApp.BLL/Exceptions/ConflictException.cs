namespace TodoApp.BLL.Exceptions;

public class ConflictException(string message) : Exception(message);