using LerningApp.Services.Data.Interfaces;
using LerningApp.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Web.API.Controllers;

[ApiController]
[Route("api/lessons")]
[Authorize]
public class LessonApiController(ILessonService lessonService, 
    ITeacherService teacherService) : ControllerBase
{
    [HttpGet("available-indexes")]
    public async Task<ActionResult<List<int>>> GetAvailableLessonIndexes([FromQuery]string courseId)
    {
        var userId = User.GetUserId()!;
        var isTeacher = await teacherService.IsUserTeacherAsync(userId);
        if (!isTeacher)
        {
            return Forbid();
        }
        
        var indexes = await lessonService.GetAvailableOrderIndexes(courseId);

        if (indexes == null)
        {
            return BadRequest("Invalid course id.");
        }

        return Ok(indexes);
    }
    
    [AllowAnonymous]
    [HttpGet("whoami")]
    public IActionResult WhoAmI()
    {
        return Ok(new
        {
            isAuth = User.Identity?.IsAuthenticated,
            name = User.Identity?.Name
        });
    }
}