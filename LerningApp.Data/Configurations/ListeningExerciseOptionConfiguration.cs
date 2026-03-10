using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LerningApp.Data.Configurations;

public class ListeningExerciseOptionConfiguration : IEntityTypeConfiguration<ListeningExerciseOption>
{
    public void Configure(EntityTypeBuilder<ListeningExerciseOption> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Answer)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(x => x.isCorrect)
            .IsRequired();

        builder
            .Property(x => x.OrderIndex)
            .IsRequired();

        builder
            .HasOne(x => x.ListeningExercise)
            .WithMany(x => x.AnswerOptions)
            .HasForeignKey(x => x.ListeningExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasQueryFilter(o => !o.ListeningExercise.IsDeleted);

        builder.HasIndex(x => x.ListeningExerciseId);
        builder.HasIndex(x => new { x.ListeningExerciseId, x.OrderIndex }).IsUnique();
    }
}