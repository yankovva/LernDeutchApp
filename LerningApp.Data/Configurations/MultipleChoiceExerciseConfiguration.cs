using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static LerningApp.Common.EntityValidationConstants.MultipleChoiceExercise;
namespace LerningApp.Data.Configurations;

public class MultipleChoiceExerciseConfiguration : IEntityTypeConfiguration<MultipleChoiceExercise>
{
    public void Configure(EntityTypeBuilder<MultipleChoiceExercise> builder)
    {
        builder
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.Question)
            .IsRequired()
            .HasMaxLength(QuestionMaxLength);
        
        builder
            .Property(x => x.CorrectAnswer)
            .IsRequired()
            .HasMaxLength(AnswerMaxLength);
        
        builder
            .Property(x => x.FirstWrongAnswer)
            .IsRequired()
            .HasMaxLength(AnswerMaxLength);
        
        builder.
            Property(x => x.SecondWrongAnswer)
            .IsRequired()
            .HasMaxLength(AnswerMaxLength);
        
        builder
            .Property(x => x.ThirdWrongAnswer)
            .HasMaxLength(AnswerMaxLength);
        
        builder
            .HasQueryFilter(x => x.IsDeleted == false);
        
        builder.HasOne(x => x.Lesson)
            .WithMany(x => x.MultipleChoiceExercises)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => new { x.LessonId, x.OrderIndex });
    }
}