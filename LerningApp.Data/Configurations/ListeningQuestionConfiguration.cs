using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LerningApp.Data.Configurations;

public class ListeningQuestionConfiguration : IEntityTypeConfiguration<ListeningQuestion>
{
    public void Configure(EntityTypeBuilder<ListeningQuestion> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Question)
            .IsRequired()
            .HasMaxLength(250);

        builder
            .HasOne(x => x.ListeningExercise)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.ListeningExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.Options)
            .WithOne(x => x.ListeningQuestion)
            .HasForeignKey(x => x.ListeningQuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasQueryFilter(x => !x.ListeningExercise.IsDeleted);

        builder.HasIndex(x => x.ListeningExerciseId);
    }
}