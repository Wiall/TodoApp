using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.DAL.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(category => category.Id);

        builder.Property(category => category.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(category => category.Description)
            .HasMaxLength(500);

        builder.HasOne(category => category.User)
            .WithMany(user => user.Categories)
            .HasForeignKey(category => category.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(category => new
            {
                category.UserId,
                category.Name
            })
            .IsUnique();
    }
}