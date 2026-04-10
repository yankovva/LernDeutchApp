using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.API.DTO.Common;
using LerningApp.Web.API.DTO.MultipleChoiceExerciseDtos;
using LerningApp.Web.API.DTO.TranslationExerciseDtos;
using LerningApp.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Web.API.Controllers;

[Authorize]
[ApiController]
[Route("api/translation-exercise")]
public class TranslationExerciseApiController(ITranslationExerciseService exerciseService) :ControllerBase
{
    [HttpPost("check-answer")]
    [ProducesResponseType(typeof(CheckTranslationExerciseAnswerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckTranslationExercise([FromBody]CheckCorrectTranslationInputDto checkInputDto)
    {
        string userId = User.GetUserId()!;
        var serviceResult = await exerciseService
            .CheckTranslationAsync(checkInputDto.ExerciseId, checkInputDto.UserTranslation, checkInputDto.LessonId, userId);
        
        if (serviceResult == null)
        {
            return BadRequest("Invalid operation.");
        }
        
        var result = new CheckTranslationExerciseAnswerDto()
        {
            IsCorrect = serviceResult.Value.isCorrect,
            CorrectAnswer = serviceResult.Value.correctAnswer,
        };
       
        return Ok(result);
    }
    
    [HttpPost("soft-delete")]
    [Authorize(Roles = "Admin,Teacher")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SoftDelete([FromBody] SoftDeleteExerciseInputDto dto)
    {
        var userId = User.GetUserId()!;
        var serviceResult = await exerciseService
            .SoftDeleteAsync(dto.ExerciseId, userId);
        
        if (serviceResult.Result == false)
        {
            return BadRequest("Invalid operation.");
        }
        
        return Ok("Successfully deleted exercise.");
    }
}