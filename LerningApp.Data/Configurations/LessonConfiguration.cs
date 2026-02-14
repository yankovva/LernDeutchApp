using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LerningApp.Data.Configurations;
using LerningApp.Data.Models;
using static Common.EntityValidationConstants.Lesson;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        // Fluent API
         builder
             .HasKey(l => l.Id);
         builder
             .Property(l => l.CourseId);
         
         builder
             .Property(l => l.Name)
             .HasMaxLength(NameMaxLength)
             .IsRequired();
         
         builder
             .Property(l => l.Content)
             .HasMaxLength(ContentMaxLength)
             .IsRequired();
         
         builder
             .Property(l => l.Target)
             .HasMaxLength(TargetMaxLength)
             .IsRequired();
         
         builder
             .HasOne(e => e.Course)
             .WithMany(g => g.LessonsForCourse)
             .HasForeignKey(e => e.CourseId)
             .OnDelete(DeleteBehavior.Cascade);

    }
}