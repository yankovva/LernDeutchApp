using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LerningApp.Data;

public class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();
        var db = scope.ServiceProvider
            .GetRequiredService<LerningAppContext>();

        await db.Database.MigrateAsync();

        if (await db.Levels.AnyAsync())
            return;

        var a1 = new Level
        {
            Id = Guid.NewGuid(),
            Name = "A1",
            Description = "Beginner"
        };
        
        var a2 = new Level
        {
            Id = Guid.NewGuid(),
            Name = "A2",
            Description = "Medium"
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
            Content = " Das Haus ist groß.Die Tür ist offen.Das Fenster ist klein.",
            OrderIndex = 1,
            CreatedAt = DateTime.UtcNow,
            Course = course,
            Target = "Lern basic home vocabulary",
            Gramatic = "German has more endings for verbs in the present tense than English. You\ntake the stem of a verb and then add the required ending. The stem is the\nform of the infinitive without -en or -n"
        };
        
        var noun = new PartOfSpeech
        {
            Id = Guid.NewGuid(),
            Name = "noun"
        };

        var item1 = new VocabularyCard
        {
            Id = Guid.NewGuid(),
            Lesson = lesson1,
            PartOfSpeech = noun,
            Terms = new List<VocabularyTerm>
            {
                new VocabularyTerm
                {
                    Id = Guid.NewGuid(),
                    Side = "bg",
                    Word = "къща",
                    IsPrimary = true,
                },
                new VocabularyTerm
                {
                    Id = Guid.NewGuid(),
                    Side = "bg",
                    Word = "дом",
                    IsPrimary = false
                },
                new VocabularyTerm
                {
                    Id = Guid.NewGuid(),
                    Side = "de",
                    Word = "Haus",
                    IsPrimary = true,
                    Gender = "das",
                    ExampleSentence = "Das Haus ist groß."
                }

            }
        };

        var item3 = new VocabularyCard
        {
            Id = Guid.NewGuid(),
            Lesson = lesson1,
            PartOfSpeech = noun,
            Terms = new List<VocabularyTerm>
            {
                new VocabularyTerm
                {
                    Id = Guid.NewGuid(),
                    Side = "bg",
                    Word = "куче",
                    IsPrimary = true
                },
                new VocabularyTerm
                {
                    Id = Guid.NewGuid(),
                    Side = "de",
                    Word = "Hund",
                    IsPrimary = true,
                    Gender = "der",
                    ExampleSentence = "Der Hund ist freundlich."
                }

            }
        };


        var item2 = new VocabularyCard
        {
            Id = Guid.NewGuid(),
            Lesson = lesson1,
            PartOfSpeech = noun,
            Terms = new List<VocabularyTerm>
            {
                new VocabularyTerm
                {
                    Id = Guid.NewGuid(),
                    Side = "bg",
                    Word = "врата",
                    IsPrimary = true
                },
                new VocabularyTerm
                {
                    Id = Guid.NewGuid(),
                    Side = "de",
                    Word = "Tür",
                    IsPrimary = true,
                    Gender = "die",
                    ExampleSentence = "Die Tür ist offen."
                }

            }
        };

        await db.Levels.AddRangeAsync(a1,a2);
        await db.Courses.AddAsync(course);
        await db.Lessons.AddAsync(lesson1);
        await db.PartsOfSpeech.AddAsync(noun);
        await db.VocabularyCards.AddRangeAsync(item1, item2, item3);

        await db.SaveChangesAsync();
    }
}