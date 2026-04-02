using LerningApp.Common;
using LerningApp.Data;
using LerningApp.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LerningApp.Web.Infrastructure.Extensions;

public static class ExtensionMethods
{
    public static async Task<IApplicationBuilder> ApplyMigrations(this IApplicationBuilder builder)
    {
        using IServiceScope serviceScope = builder.ApplicationServices.CreateScope(); 
        
        LerningAppContext dbContext = serviceScope.ServiceProvider.GetRequiredService<LerningAppContext>()!;
        await dbContext.Database.MigrateAsync();
        
        return builder;
    }

    public static async Task<IApplicationBuilder> SeedRolesAndAdminAsync(this IApplicationBuilder builder)
    {
        using IServiceScope serviceScope = builder.ApplicationServices.CreateScope(); 

        RoleManager<ApplicationRole> roleManager = serviceScope.ServiceProvider
            .GetRequiredService<RoleManager<ApplicationRole>>();
        UserManager<ApplicationUser> userManager = serviceScope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();
        
        IConfiguration configuration = serviceScope.ServiceProvider
            .GetRequiredService<IConfiguration>();
        
        var dbContext = serviceScope.ServiceProvider
            .GetRequiredService<LerningAppContext>();
        

        var email = configuration["ApplicationAdmin:Email"] 
                       ?? throw new InvalidOperationException("Email is missing.");
        var password = configuration["ApplicationAdmin:Password"] 
                       ?? throw new InvalidOperationException("Password is missing.");
        var userName = configuration["ApplicationAdmin:UserName"] 
                       ?? throw new InvalidOperationException("Username is missing.");

        string[] roles = ["Admin", "Teacher"];

        foreach (var role in roles)
        {
            bool roleExists = await roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
               var isCreated = await roleManager.CreateAsync(new ApplicationRole(role));
               if (!isCreated.Succeeded)
               {
                   throw new InvalidOperationException($"Failed to create role '{role}'.");
               }
            }
        }
        
        ApplicationUser adminUser = await CreateAdminAsync(email, password, userName, userManager);
        bool isInAdminRole = await userManager.IsInRoleAsync(adminUser, "Admin");
        bool isInRoleTeacher = await userManager.IsInRoleAsync(adminUser, "Teacher");
      
        if (!isInAdminRole)
        {
            var result = await userManager.AddToRoleAsync(adminUser, "Admin");
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to assign Admin role.");
            }
        }
        
        if (!isInRoleTeacher)
        {
            var teacherResult = await userManager.AddToRoleAsync(adminUser, "Teacher");
            if (!teacherResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to assign Teacher role.");
            }
        }
        
        bool teacherExists = await dbContext.Teachers
            .AnyAsync(t => t.UserId == adminUser.Id);
            
        if (!teacherExists)
        {
            Teacher teacher = new()
            {
                Status = Enums.TeacherStatus.Approved,
                TeacherSince = DateTime.UtcNow,
                UserId = adminUser.Id
            };

            dbContext.Teachers.Add(teacher);
            await dbContext.SaveChangesAsync();
        }
        
        return builder;
    }

    private static async Task<ApplicationUser> CreateAdminAsync(string email, string password, string userName,
        UserManager<ApplicationUser> userManager)
    {
        ApplicationUser? existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return existingUser;
        }
        ApplicationUser user = new()
        {
            Email = email,
            UserName = userName,
        };
        
        IdentityResult result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to register user '{userName}'.");
        }
        
        return user;
    }
}