using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static LerningApp.Common.EntityValidationConstants.Teacher;
namespace LerningApp.Data.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.Biography)
            .HasMaxLength(BiographyMaxLength);
        
        builder
            .Property(x => x.Qualification)
            .HasMaxLength(QualificationMaxLength);
        
        builder
            .Property(x => x.TeacherSince)
            .HasColumnType("datetime2");
        
        builder.Property(t => t.PendingFirstName)
            .HasMaxLength(FirstNameMaxLength)
            .IsRequired(false);

        builder.Property(t => t.PendingLastName)
            .HasMaxLength(LastNameMaxLength)
            .IsRequired(false);

        builder.Property(t => t.PendingPhoneNumber)
            .HasMaxLength(PhoneNumberMaxLength)
            .IsRequired(false);

        builder.Property(t => t.PendingProfileImage)
            .HasMaxLength(ProfileImageMaxLength)
            .IsRequired(false);

        builder.Property(t => t.PendingBiography)
            .HasMaxLength(BiographyMaxLength)
            .IsRequired(false);

        builder.Property(t => t.PendingQualification)
            .HasMaxLength(2000)
            .IsRequired(false);

        
        builder
            .HasOne( x => x.User)
            .WithOne(u => u.Teacher)
            .HasForeignKey<Teacher>( x => x.UserId);
    }
}