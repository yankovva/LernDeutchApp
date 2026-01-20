using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static LerningApp.Common.EntityValidationConstants.Course;

namespace LerningApp.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        // Fluent Api
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .HasMaxLength(NameMaxLength)
            .IsRequired();
        
        builder.Property(c => c.Description)
            .HasMaxLength(DescriptionMaxLength)
            .IsRequired();
        
        builder
            .HasOne(e => e.Level)
            .WithMany(g => g.CoursesForLevel)
            .HasForeignKey(e => e.LevelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}