using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static LerningApp.Common.EntityValidationConstants.VocabularyTerm;

namespace LerningApp.Data.Configurations;

public class VocabularyTermConfiguration
    :IEntityTypeConfiguration<VocabularyTerm>
{
    public void Configure(EntityTypeBuilder<VocabularyTerm> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Word)
            .IsRequired()
            .HasMaxLength(WordMaxLength);

        builder.Property(x => x.Side)
            .IsRequired()
            .HasMaxLength(SideMaxLength);
        
        builder.Property(x => x.Gender)
            .HasMaxLength(GenderMaxLength)
            .IsFixedLength();
        
        builder.Property(x => x.ExampleSentence)
            .HasMaxLength(ExampleSentenceMaxLenght);
        
        builder.HasOne(x => x.VocabularyCard)
            .WithMany(i => i.Terms)
            .HasForeignKey(x => x.VocabularyCardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => new { x.VocabularyCardId, x.Side });
    }
}