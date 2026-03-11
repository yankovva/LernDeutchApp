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
            .HasOne(x => x.ListeningQuestion)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.ListeningQuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasQueryFilter(o => !o.ListeningQuestion.IsDeleted);

        builder.HasIndex(x => x.ListeningQuestionId);
        builder.HasIndex(x => new { x.ListeningQuestionId, x.OrderIndex }).IsUnique();
    }
}