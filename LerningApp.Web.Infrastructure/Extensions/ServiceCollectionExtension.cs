using System.Reflection;
using LerningApp.Data;
using LerningApp.Data.Repository;
using LerningApp.Data.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LerningApp.Web.Infrastructure.Extensions;

public static class ServiceCollectionExtension
{
    public static void RegisterRepositories(this IServiceCollection services)
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
    }
    public static void RegisterUserDefinedServices(this IServiceCollection services, Assembly serviceAssembly)
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
    }

}