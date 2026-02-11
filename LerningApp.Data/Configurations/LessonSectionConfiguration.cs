using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static LerningApp.Common.EntityValidationConstants.LessonSection;
namespace LerningApp.Data.Configurations;

public class LessonSectionConfiguration : IEntityTypeConfiguration<LessonSection>
{
    public void Configure(EntityTypeBuilder<LessonSection> builder)
    {
       builder.HasKey(x => x.Id);
       
       builder.Property(x => x.Type)
           .IsRequired()
           .HasMaxLength(TypeMaxLength);

       builder.Property(x => x.Content)
           .IsRequired()
           .HasMaxLength(ContentMaxLength);

       builder.Property(x => x.OrderIndex)
           .IsRequired();

       builder.HasOne(x => x.Lesson)
           .WithMany(l => l.LessonSections)
           .HasForeignKey(x => x.LessonId)
           .OnDelete(DeleteBehavior.Cascade);

       builder.HasIndex(x => new { x.LessonId, x.OrderIndex });
    }
}