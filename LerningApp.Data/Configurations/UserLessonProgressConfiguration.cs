using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LerningApp.Data.Configurations;

public class UserLessonProgressConfiguration : IEntityTypeConfiguration<UserLessonProgress>
{
    public void Configure(EntityTypeBuilder<UserLessonProgress> builder)
    {
        builder
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.CompletedAt)
            .HasColumnType("datetime2");
        
        builder
            .HasQueryFilter(x => !x.Lesson.IsDeleted);
        
        builder
            .HasOne(x => x.User)
            .WithMany(u => u.UserLessonsProgresses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Lesson)
            .WithMany(l => l.UsersLessonProgresses)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasIndex(x => new { x.UserId, x.LessonId }).IsUnique();
    }
}