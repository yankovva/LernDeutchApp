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
            .HasOne(x => x.MultipleChoiceExercise)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.MultipleChoiceExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasQueryFilter(o => !o.MultipleChoiceExercise.IsDeleted);

        builder.HasIndex(x => x.MultipleChoiceExerciseId);
        builder.HasIndex(x => new { x.MultipleChoiceExerciseId, x.OrderIndex }).IsUnique();

    }
}