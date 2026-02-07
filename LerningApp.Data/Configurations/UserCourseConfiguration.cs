using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LerningApp.Data.Configurations;

public class UserCourseConfiguration : IEntityTypeConfiguration<UserCourse>
{
    public void Configure(EntityTypeBuilder<UserCourse> builder)
    {
        builder.HasKey(x => new { x.UserId, x.CourseId });

        builder.HasOne(x => x.User)
            .WithMany(u => u.UserCourses)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Course)
            .WithMany(l => l.CourseParticipants)
            .HasForeignKey(x => x.CourseId);

    }
}