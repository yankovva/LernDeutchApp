using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LerningApp.Data.Configurations;

public class MultipleChoiceQuestionConfiguration : IEntityTypeConfiguration<MultipleChoiceQuestion>
{
    public void Configure(EntityTypeBuilder<MultipleChoiceQuestion> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Question)
            .IsRequired()
            .HasMaxLength(250);

        builder
            .HasOne(x => x.MultipleChoiceExercise)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.MultipleChoiceExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.Options)
            .WithOne(x => x.MultipleChoiceQuestion)
            .HasForeignKey(x => x.MultipleChoiceQuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(x => x.Publisher)
            .WithMany(x => x.MultipleChoiceQuestions)
            .HasForeignKey(x => x.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasQueryFilter(x => !x.MultipleChoiceExercise.IsDeleted);

        builder.HasIndex(x => x.MultipleChoiceExerciseId);
    }
}