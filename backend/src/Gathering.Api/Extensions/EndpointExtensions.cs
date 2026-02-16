using Gathering.Api.Endpoints;
using System.Reflection;

namespace Gathering.Api.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var endpointTypes = assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IEndpoint)) && t.IsClass);

        foreach (var type in endpointTypes)
        {
            services.AddTransient(typeof(IEndpoint), type);
        }

        return services;

        //services.AddTransient<IEndpoint, Endpoints.Communities.Create>();
        //services.AddTransient<IEndpoint, Endpoints.Communities.GetAll>();
        //services.AddTransient<IEndpoint, Endpoints.Sessions.Create>();
        //services.AddTransient<IEndpoint, Endpoints.Sessions.GetByCommunity>();

        return services;
    }

    public static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }


        return app;
    }
}
