using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace LerningApp.Controllers;

public class BaseController : Controller
{
    protected bool IsGuidValid(string id, ref Guid courseId)
    {
        if (String.IsNullOrEmpty(id))
        {
            return false;
        } 
        bool isIdValid = Guid.TryParse(id, out  courseId);
        if (!isIdValid)
        {
            return false;
        }
       
        return isIdValid;
    }

    protected Guid? GetCurrentUserIdOrNull()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var userId) ? userId : null;
    }


}