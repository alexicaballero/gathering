using Gathering.Application.Abstractions;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Communities;
using Gathering.Domain.Sessions;
using Gathering.Infrastructure.Repositories;
using Gathering.Infrastructure.Storage;
using Gathering.Infrastructure.Time;
using Gathering.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure.Storage.Blobs;

namespace Gathering.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
        });

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ICommunityRepository, CommunityRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();

        // Azure Blob Storage
        var azureStorageConnectionString = configuration.GetSection("AzureStorage:ConnectionString").Value
            ?? throw new InvalidOperationException("AzureStorage:ConnectionString is not configured.");
        services.AddSingleton(new BlobServiceClient(azureStorageConnectionString));
        services.AddScoped<IImageStorageService, AzureBlobStorageService>();

        return services;
    }
}
