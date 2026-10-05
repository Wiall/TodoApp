namespace TodoApp.Domain.Entities;

public class TodoTask
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsDone { get; set; }
    
    public DateTime? DueTo { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
}