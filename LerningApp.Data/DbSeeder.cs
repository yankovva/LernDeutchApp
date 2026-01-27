using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LerningApp.Data;

public class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LerningAppContext>();

        await db.Database.MigrateAsync();

        if (await db.Levels.AnyAsync())
            return;

        var a1 = new Level
        {
            Id = Guid.NewGuid(),
            Name = "A1",
            Description = "Beginner"
        };

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "German Basics (A1)",
            Description = "First steps in German",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            Level = a1
        };

        var lesson1 = new Lesson
        {
            Id = Guid.NewGuid(),
            Name = "At home",
            Content = "Basic home vocabulary",
            OrderIndex = 1,
            CreatedAt = DateTime.UtcNow,
            Course = course
        };
        
        var item1 = new VocabularyItem
        {
            Id = Guid.NewGuid(),
            Lesson = lesson1,
            Terms = new List<VocabularyTerm>
            {
                new VocabularyTerm
                {
                    Id = Guid.NewGuid(),
                    Side = "bg",
                    Text = "къща",
                    IsPrimary = true
                },
                new VocabularyTerm
                {
                    Id = Guid.NewGuid(),
                    Side = "bg",
                    Text = "дом",
                    IsPrimary = false
                },
                new VocabularyTerm
                {
                    Id = Guid.NewGuid(),
                    Side = "de",
                    Text = "Haus",
                    IsPrimary = true
                }
            }
        };

        var item2 = new VocabularyItem
        {
            Id = Guid.NewGuid(),
            Lesson = lesson1,
            Terms = new List<VocabularyTerm>
            {
                new VocabularyTerm
                {
                    Id = Guid.NewGuid(),
                    Side = "bg",
                    Text = "врата",
                    IsPrimary = true
                },
                new VocabularyTerm
                {
                    Id = Guid.NewGuid(),
                    Side = "de",
                    Text = "Tür",
                    IsPrimary = true
                }
            }
        };

        await db.Levels.AddAsync(a1);
        await db.Courses.AddAsync(course);
        await db.Lessons.AddAsync(lesson1);
        await db.VocabularyItems.AddRangeAsync(item1, item2);

        await db.SaveChangesAsync();
    }
}