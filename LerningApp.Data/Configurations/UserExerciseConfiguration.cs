using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LerningApp.Data.Configurations;

public class UserExerciseConfiguration : IEntityTypeConfiguration<UserExerciseProgress>
{
    public void Configure(EntityTypeBuilder<UserExerciseProgress> builder)
    {
        builder
            .HasKey(x => x.Id);
        
        builder
            .HasQueryFilter(x => !x.Lesson.IsDeleted);
        
        builder
            .Property(x => x.CompletedAt)
            .HasColumnType("datetime2");
        
        builder
            .HasOne(x => x.User)
            .WithMany(u => u.UserExerciseProgresses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(x => x.Lesson)
            .WithMany(u => u.UserExerciseProgresses)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .Property(x => x.ExerciseId)
            .IsRequired();
        
        builder
            .HasIndex(x => new { x.UserId, x.ExerciseId , x.ExerciseType}).IsUnique();
    }
}