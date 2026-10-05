using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.DAL.Configurations;

public class TodoTaskConfiguration : IEntityTypeConfiguration<TodoTask>
{
    public void Configure(EntityTypeBuilder<TodoTask> builder)
    {
        builder.HasKey(task => task.Id);

        builder.Property(task => task.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(task => task.Description)
            .HasMaxLength(2000);

        builder.Property(task => task.IsDone)
            .IsRequired();

        builder.Property(task => task.CreatedAt)
            .IsRequired();

        builder.HasOne(task => task.User)
            .WithMany(user => user.Tasks)
            .HasForeignKey(task => task.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(task => task.Category)
            .WithMany(category => category.Tasks)
            .HasForeignKey(task => task.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}