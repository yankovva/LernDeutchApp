using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static LerningApp.Common.EntityValidationConstants.User;

namespace LerningApp.Data.Configurations;

public class ApplicationUserConfiguration: IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName)
            .HasMaxLength(FirstNameMaxLength)
            .IsRequired(false);

        builder.Property(x => x.LastName)
            .HasMaxLength(LastNameMaxLength)
            .IsRequired(false);

        builder.Property(x => x.ProfileImage)
            .HasMaxLength(ProfileImageMaxLength)
            .IsRequired(false);
    }
}