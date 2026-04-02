using LerningApp.Common;
using LerningApp.Data.Models;
using LerningApp.Data.Repository.Interfaces;
using LerningApp.Services.Data.Interfaces.TeacherInterfaces;
using LerningApp.Web.ViewModels.Teacher;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Services.Data.TeacherServices;

public class TeacherLessonService(IRepository<Lesson, Guid> lessonRepository) : ITeacherLessonService
{
    public async Task<IEnumerable<LessonTeacherIndexViewModel>> GetAllLessonsAsync()
    {
        var lessons = await lessonRepository
            .GetAllAttached()
            .Include(c => c.Course)
            .Select(l => new LessonTeacherIndexViewModel
            {
                Id = l.Id.ToString(),
                Name = l.Name,
                CourseName  = l.Course != null ? l.Course.Name : "Not added to course.",
                IsDeleted = l.IsDeleted,
                ExercisesCount = l.ListeningExercises.Count
                                 + l.TranslationExercises.Count
                                 + l.MultipleChoiceExercises.Count
            }).ToListAsync();
        
        return lessons;
    }
}