using LerningApp.Services.Data.Interfaces;
using LerningApp.Contracts.ListeningExerciseDtos;
using LerningApp.Web.API.DTO.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LerningApp.Web.Infrastructure.Extensions;

namespace LerningApp.Web.API.Controllers;

[Authorize]
[ApiController]
[Route("api/listening-exercise")]
public class ListeningExerciseApiController(IListeningExerciseService listeningExerciseService) : ControllerBase
{
    [HttpPost("check-answer")]
    [ProducesResponseType(typeof(ListeningQuestionCheckResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckListeningExercise(CheckListeningExerciseInputDto dto)
    {
        var userId = User.GetUserId()!;

        var serviceResult = await listeningExerciseService
            .CheckListeningExerciseAnswer(dto, userId);

        if (serviceResult.Result == false)
        {
            return BadRequest("Invalid operation");
        }

        var results = serviceResult.Data;
        
        return Ok(results);
    }
    [HttpPost("soft-delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SoftDelete([FromBody] SoftDeleteExerciseInputDto dto)
    {
        var userId = User.GetUserId()!;
        var serviceResult = await listeningExerciseService
            .SoftDeleteExerciseAsync(dto.ExerciseId, userId);
        
        if (serviceResult.Result == false)
        {
            return BadRequest("Invalid operation.");
        }
        
        return Ok("Successfully deleted exercise.");
    }
}