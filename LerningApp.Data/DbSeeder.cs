using LerningApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LerningApp.Data;

public class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<LerningAppContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await db.Database.MigrateAsync();

        if (await db.Levels.AnyAsync())
            return;

        var seedEmail = "seed.user@gmail.com";
        var seedUser = await userManager.FindByEmailAsync(seedEmail);

        if (seedUser == null)
        {
            seedUser = new ApplicationUser
            {
                UserName = seedEmail,
                Email = seedEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(seedUser, "User123!");
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to create seed user: " +
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }

        var teacher = new Teacher
        {
            Id = Guid.Parse("662d6cc2-90ea-4e1b-bf31-2f43088233e4"),
            FirstName = "Teacher",
            LastName = "Teacher",
            Biography =
                "I'm a teacher! The Best ever! You can learn a lot from me and be the best student ever to exist! Join my Courses now!",
            Qualification = "I have studied something with computers!",
            UserId = seedUser.Id,
            IsApproved = true
        };

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
            Level = a1,
            PublisherId = teacher.Id,
            Price = 20
        };
        
        var lesson1 = new Lesson
        {
            Id = Guid.NewGuid(),
            Name = "At home",
            Content = "Das Haus ist groß. Die Tür ist offen. Das Fenster ist klein.",
            OrderIndex = 1,
            CreatedAt = DateTime.UtcNow,
            Course = course,
            PublisherId = teacher.Id,
            Target = "Learn basic home vocabulary",
           
        };
        var tEx1 = new TranslationExercise
        {
            Id = Guid.NewGuid(),
            Lesson = lesson1,
            GermanSentence = "Die Tür ist offen.",
            BulgarianSentence = "Вратата е отворена.",
            EnglishSentence = "The door is open.",
            DifficultyLevel = 1,
            PublisherId = teacher.Id
        };

        var tEx2 = new TranslationExercise
        {
            Id = Guid.NewGuid(),
            Lesson = lesson1,
            GermanSentence = "Das Haus ist groß.",
            BulgarianSentence = "Къщата е голяма.",
            EnglishSentence = "The house is big.",
            DifficultyLevel = 1,
            PublisherId = teacher.Id
        };
        
        var mc1 = new MultipleChoiceExercise
        {
            Id = Guid.NewGuid(),
            Lesson = lesson1,
            Question = "Was bedeutet „Haus“?",
            CorrectAnswer = "къща",
            FirstWrongAnswer = "врата",
            SecondWrongAnswer = "куче",
            ThirdWrongAnswer = "прозорец",
            DifficultyLevel = 1,
            PublisherId = teacher.Id
        };

        var mc2 = new MultipleChoiceExercise
        {
            Id = Guid.NewGuid(),
            Lesson = lesson1,
            Question = "Was bedeutet „Hund“?",
            CorrectAnswer = "куче",
            FirstWrongAnswer = "дом",
            SecondWrongAnswer = "врата",
            ThirdWrongAnswer = "прозорец",
            DifficultyLevel = 1,
            PublisherId = teacher.Id
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
                    Side = "en",
                    Word = "house",
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
                    Side = "en",
                    Word = "dog",
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
                },
                new VocabularyTerm
                {
                    Id = Guid.NewGuid(),
                    Side = "en",
                    Word = "Door",
                    IsPrimary = true
                 }
            }
        };

        await db.Levels.AddRangeAsync(a1, a2);
        await db.Courses.AddAsync(course);
        await db.Lessons.AddAsync(lesson1);
        await db.PartsOfSpeech.AddAsync(noun);
        await db.VocabularyCards.AddRangeAsync(item1, item2, item3);
        await db.Teachers.AddAsync(teacher);
        await db.TranslationExercises.AddRangeAsync(tEx1, tEx2);
        await db.MultipleChoiceExercises.AddRangeAsync(mc1, mc2);

        await db.SaveChangesAsync();
    }
}