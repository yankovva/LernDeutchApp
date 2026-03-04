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
            .Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(FirstNameMaxLength);
        
        builder
            .Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(LastNameMaxLength);
        
        builder
            .Property(x => x.ProfileImage)
            .HasMaxLength(ProfileImageMaxLength);
        
        builder
            .Property(x => x.Biography)
            .HasMaxLength(BiographyMaxLength);
        
        builder
            .Property(x => x.Qualification)
            .IsRequired()
            .HasMaxLength(QualificationMaxLength);
        
        builder
            .HasOne( x => x.User)
            .WithOne(u => u.Teacher)
            .HasForeignKey<Teacher>( x => x.UserId);
    }
}