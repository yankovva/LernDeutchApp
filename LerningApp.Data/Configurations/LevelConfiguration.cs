using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static LerningApp.Common.EntityValidationConstants.Level;

namespace LerningApp.Data.Configurations;

public class LevelConfiguration: IEntityTypeConfiguration<Level>
{
    public void Configure(EntityTypeBuilder<Level> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(NameMaxLength);
        
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(DescriptionMaxLength);
        
       
    }
}