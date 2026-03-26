using LerningApp.Contracts.MultipleChoiceExerciseDtos;
using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.API.DTO.Common;
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
    [ProducesResponseType(typeof(MultipleChoiceCheckResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckMultipleChoiceExercise([FromBody]CheckMultipleChoiceExerciseInputDto checkInputDto)
    {
        var userId = User.GetUserId()!;
        
        var serviceResult = await exerciseService
            .CheckMultipleChoice(checkInputDto, userId);

        if (serviceResult.Result == false)
        {
           return BadRequest("Invalid operation.");
        }

        var result = serviceResult.Data;
       
        return Ok(result);
    }
    
    [HttpPost("soft-delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SoftDelete([FromBody] SoftDeleteExerciseInputDto dto)
    {
        var userId = User.GetUserId()!;
        var serviceResult = await exerciseService
            .SoftDeleteExerciseAsync(dto.ExerciseId, userId);
        
        if (serviceResult.Result == false)
        {
            return BadRequest("Invalid operation.");
        }
        
        return Ok("Successfully deleted exercise.");
    }
}