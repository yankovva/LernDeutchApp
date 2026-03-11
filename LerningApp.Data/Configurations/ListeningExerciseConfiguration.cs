using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LerningApp.Data.Configurations;

public class ListeningExerciseConfiguration : IEntityTypeConfiguration<ListeningExercise>
{
    public void Configure(EntityTypeBuilder<ListeningExercise> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.AudioPath)
            .IsRequired()
            .HasMaxLength(500);

        builder
            .Property(x => x.DifficultyLevel)
            .IsRequired();

        builder
            .HasQueryFilter(x => x.IsDeleted == false);

        builder
            .HasOne(x => x.Lesson)
            .WithMany(x => x.ListeningExercises)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Publisher)
            .WithMany(x => x.ListeningExercises)
            .HasForeignKey(x => x.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.LessonId);
        builder.HasIndex(x => x.PublisherId);
    }
}