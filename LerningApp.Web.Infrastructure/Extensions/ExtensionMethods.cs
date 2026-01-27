using LerningApp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LerningApp.Web.Infrastructure.Extensions;

public static class ExtensionMethods
{
    public static async Task<IApplicationBuilder> ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope serviceScope = app.ApplicationServices.CreateScope(); 
        
        LerningAppContext dbContext = serviceScope.ServiceProvider.GetRequiredService<LerningAppContext>()!;
        await dbContext.Database.MigrateAsync();
        
        return app;
    }
}