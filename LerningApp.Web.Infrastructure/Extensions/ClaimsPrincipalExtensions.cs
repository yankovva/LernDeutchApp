using System.Security.Claims;

namespace LerningApp.Web.Infrastructure.Extensions;

public class ClaimsPrincipalExtensions
{
    public static string? GetUserId(ClaimsPrincipal? userClaimsPrincipal)
    {
        return userClaimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? null;
    }
}