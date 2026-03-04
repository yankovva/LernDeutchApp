using System.Security.Claims;

namespace LerningApp.Web.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal? userClaimsPrincipal)
    {
        return userClaimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? null;
    }
}