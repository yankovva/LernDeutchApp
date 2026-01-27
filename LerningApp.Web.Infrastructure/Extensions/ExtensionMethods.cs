using LerningApp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LerningApp.Web.Infrastructure.Extensions;

public static class ExtensionMethods
{
    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope serviceScope = app.ApplicationServices.CreateScope(); 
        
        LerningAppContext dbContext = serviceScope.ServiceProvider.GetRequiredService<LerningAppContext>()!;
        dbContext.Database.Migrate();
        
        return app;
    }
}