using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LerningApp.Data.Configurations;

public class MultipleChoiceOptionConfiguration : IEntityTypeConfiguration<MultipleChoiceExerciseOption>
{
    public void Configure(EntityTypeBuilder<MultipleChoiceExerciseOption> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Answer)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(x => x.IsCorrect)
            .IsRequired();

        builder
            .Property(x => x.OrderIndex)
            .IsRequired();

        builder
            .HasOne(x => x.MultipleChoiceQuestion)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.MultipleChoiceQuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasQueryFilter(o => !o.MultipleChoiceQuestion.IsDeleted);

        builder.HasIndex(x => x.MultipleChoiceQuestionId);
        builder.HasIndex(x => new { x.MultipleChoiceQuestionId, x.OrderIndex }).IsUnique();

    }
}