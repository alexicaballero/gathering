using FluentValidation;
using Gathering.Application.Abstractions;
using Gathering.Application.Mediator;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Gathering.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISender, Sender>();

        var assemblies = new[] { typeof(DependencyInjection).Assembly };

        services.RegisterHandlers(assemblies);
        services.RegisterDomainEventHandlers(assemblies);

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }

    private static IServiceCollection RegisterHandlers(this IServiceCollection services, Assembly[] assemblies)
    {
        var handlerTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                !type.IsGenericTypeDefinition &&
                !type.ContainsGenericParameters &&
                type.GetInterfaces().Any(
                    i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .Select(t => new
            {
                Implementation = t,
                Interface = t.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
            });

        foreach (var handler in handlerTypes)
        {
            services.AddTransient(handler.Interface, handler.Implementation);
        }

        return services;
    }

    private static IServiceCollection RegisterDomainEventHandlers(this IServiceCollection services, Assembly[] assemblies)
    {
        var handlerTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.GetInterfaces().Any(
                    i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(SharedKernel.IDomainEventHandler<>)))
            .Select(t => new
            {
                Implementation = t,
                Interfaces = t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(SharedKernel.IDomainEventHandler<>))
            });

        foreach (var handler in handlerTypes)
        {
            foreach (var handlerInterface in handler.Interfaces)
            {
                services.AddScoped(handlerInterface, handler.Implementation);
            }
        }

        return services;
    }
}
