using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static LerningApp.Common.EntityValidationConstants.TranslationExercise;

namespace LerningApp.Data.Configurations;

public class TranslationExerciseConfiguration : IEntityTypeConfiguration<TranslationExercise>
{
    public void Configure(EntityTypeBuilder<TranslationExercise> builder)
    {
        builder
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.GermanSentence)
            .IsRequired()
            .HasMaxLength(SentenceMaxLength);

        builder
            .Property(x => x.BulgarianSentence)
            .IsRequired()
            .HasMaxLength(SentenceMaxLength);

        builder
            .Property(x => x.EnglishSentence)
            .IsRequired()
            .HasMaxLength(SentenceMaxLength);
        
        builder
            .Property(x => x.DifficultyLevel)
            .IsRequired();

        builder.HasOne(x => x.Lesson)
            .WithMany(x => x.TranslationExercises)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(c => c.Publisher)
            .WithMany(u => u.CreatedTranslationExercises)
            .HasForeignKey(c => c.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasQueryFilter(x => x.IsDeleted == false);
    }
}