using System.Reflection;
using LerningApp.Data;
using LerningApp.Data.Models;
using LerningApp.Data.Repository;
using LerningApp.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is missing.");
        services
            .AddDbContext<LerningAppContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        
        return services;
    }

    public static IServiceCollection AddApplicationIdentity(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                ConfigureIdentity(options, configuration);
            })
            .AddEntityFrameworkStores<LerningAppContext>()
            .AddDefaultTokenProviders();
        
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.LogoutPath = "/Identity/Account/Logout";
            options.ExpireTimeSpan = TimeSpan.FromHours(12);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.Name = "DeutchBuddy.Auth";
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
        });
        
        services.Configure<SecurityStampValidatorOptions>(options =>
        {
            options.ValidationInterval = TimeSpan.FromHours(1);
        });
        
        return services;
    }
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        using IServiceScope scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LerningAppContext>();

        var entityTypes = dbContext.Model
            .GetEntityTypes()
            .Select(e => e.ClrType)
            .Where(t => !t.IsAbstract);

        foreach (var type in entityTypes)
        {
            var idProp = type.GetProperty("Id");
            var idType = idProp?.PropertyType ?? typeof(object);

            var repoInterface = typeof(IRepository<,>).MakeGenericType(type, idType);
            var repoImpl = typeof(Repository<,>).MakeGenericType(type, idType);

            services.AddScoped(repoInterface, repoImpl);
        }
        return services;
    }
    public static IServiceCollection RegisterUserDefinedServices(this IServiceCollection services, Assembly serviceAssembly)
    {
        var serviceInterfaceTypes = serviceAssembly
            .GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Service"))
            .ToArray();

        var serviceTypes = serviceAssembly
            .GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract && t.Name.EndsWith("Service"))
            .ToArray();

        foreach (var serviceInterfaceType in serviceInterfaceTypes)
        {
            var serviceType = serviceTypes
                .SingleOrDefault(t => "I" + t.Name == serviceInterfaceType.Name);

            if (serviceType == null)
            {
                throw new InvalidOperationException($"Service implementation not found for {serviceInterfaceType.Name}");
            }

            services.AddScoped(serviceInterfaceType, serviceType);
        }
        return services;
    }
    private static void ConfigureIdentity(IdentityOptions options, IConfiguration configuration)
    {
        options.Password.RequiredLength = configuration.GetValue<int>("Identity:Password:RequiredLength");
        options.Password.RequireNonAlphanumeric = configuration.GetValue<bool>("Identity:Password:RequireNonAlphanumeric");
        options.Password.RequireDigit = configuration.GetValue<bool>("Identity:Password:RequireDigits");
        options.Password.RequireLowercase = configuration.GetValue<bool>("Identity:Password:RequireLowercase");
        options.Password.RequireUppercase = configuration.GetValue<bool>("Identity:Password:RequireUppercase");
        options.Password.RequiredUniqueChars =configuration.GetValue<int>("Identity:Password:RequiredUniqueChars");

        options.SignIn.RequireConfirmedEmail =configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedEmail");
        options.SignIn.RequireConfirmedPhoneNumber = configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedPhoneNumber");
        options.SignIn.RequireConfirmedAccount = configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedAccount");
        
        options.User.RequireUniqueEmail = configuration.GetValue<bool>("Identity:User:RequireUniqueEmail");
    }
}