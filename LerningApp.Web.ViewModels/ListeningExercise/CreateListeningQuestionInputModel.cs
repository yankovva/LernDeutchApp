using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LerningApp.Web.ViewModels.ListeningExercise;

public class CreateListeningQuestionInputModel
{
    [StringLength(250, MinimumLength = 5)]
    public string? QuestionText { get; set; } 
    
    public List<CreateListeningQuestionOptionInputModel> Options { get; set; } = new();
}