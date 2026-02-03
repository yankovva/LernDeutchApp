using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LerningApp.Data.Configurations;

public class VocabularyItemConfiguration: IEntityTypeConfiguration<VocabularyItem>
{
    public void Configure(EntityTypeBuilder<VocabularyItem> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder.HasOne(x => x.Lesson)
            .WithMany(l => l.VocabularyItems)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.PartOfSpeech)
            .WithMany(p => p.VocabularyItems)
            .HasForeignKey(x => x.PartOfSpeechId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.LessonId);
        builder.HasIndex(x => x.PartOfSpeechId);

    }
}