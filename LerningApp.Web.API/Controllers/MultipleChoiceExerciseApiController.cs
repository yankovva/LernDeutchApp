using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.API.DTO.MultipleChoiceExerciseDtos;
using LerningApp.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Web.API.Controllers;

[Authorize]
[Route("api/multiple-choice-exercise")]
[ApiController]
public class MultipleChoiceExerciseApiController(IMultipleChoiceExerciseService exerciseService) : ControllerBase
{
    [HttpPost("check-answer")]
    [ProducesResponseType(typeof(CheckCorrectAnswerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckMultipleChoiceExercise([FromBody]CheckCorrectInputDto checkInputDto)
    {
        var userId = User.GetUserId()!;
        
        var serviceResult = await exerciseService
            .CheckMultipleChoice(checkInputDto.ExerciseId, checkInputDto.SelectedAnswer,checkInputDto.LessonId, userId);

        if (serviceResult == null)
        {
           return BadRequest("Invalid operation.");
        }

        var result = new CheckCorrectAnswerDto()
        {
            IsCorrect = serviceResult.Value.isCorrect,
            CorrectAnswer = serviceResult.Value.correctAnswer,
        };
       
        return Ok(result);
    }
}