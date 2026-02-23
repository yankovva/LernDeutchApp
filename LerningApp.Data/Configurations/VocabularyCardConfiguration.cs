using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LerningApp.Data.Configurations;

public class VocabularyCardConfiguration: IEntityTypeConfiguration<VocabularyCard>
{
    public void Configure(EntityTypeBuilder<VocabularyCard> builder)
    {
        builder
            .HasKey(vc => vc.Id);
        
        builder.Property(vc => vc.ImagePath)
            .HasMaxLength(500)
            .HasDefaultValue("/images/VocabularyCardsImages/defaultcardimage.png");
        
        builder.HasQueryFilter(vc => vc.IsDeleted == false);

        builder.HasOne(x => x.Lesson)
            .WithMany(l => l.VocabularyCards)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.PartOfSpeech)
            .WithMany(p => p.VocabularyCards)
            .HasForeignKey(x => x.PartOfSpeechId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.LessonId);
        builder.HasIndex(x => x.PartOfSpeechId);
    }
}