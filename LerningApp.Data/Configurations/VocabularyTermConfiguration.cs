using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LerningApp.Data.Configurations;

public class VocabularyTermConfiguration
    :IEntityTypeConfiguration<VocabularyTerm>
{
    public void Configure(EntityTypeBuilder<VocabularyTerm> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Side)
            .IsRequired()
            .HasMaxLength(5);

        builder.HasOne(x => x.VocabularyItem)
            .WithMany(i => i.Terms)
            .HasForeignKey(x => x.VocabularyItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.VocabularyItemId, x.Side });
    }
}