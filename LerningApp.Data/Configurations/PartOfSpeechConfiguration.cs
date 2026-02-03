using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static LerningApp.Common.EntityValidationConstants.PartOfSpeech;
namespace LerningApp.Data.Configurations;

public class PartOfSpeechConfiguration : IEntityTypeConfiguration<PartOfSpeech>
{
    public void Configure(EntityTypeBuilder<PartOfSpeech> builder)
    {
        builder
            .HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(PartOfSpeecheMaxLength);
    }
}