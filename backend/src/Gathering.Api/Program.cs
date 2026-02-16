using Asp.Versioning;
using Asp.Versioning.Builder;
using Gathering.Api;
using Gathering.Api.Extensions;
using Gathering.Application;
using Gathering.Infrastructure;
using Gathering.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

const int API_VERSION = 1;

var builder = WebApplication.CreateBuilder(args);


builder.Services
  .AddPresentationServices(API_VERSION)
  .AddApplicationServices()
  .AddInfrastructureServices(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

// Configure OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Seed database
await SeedDatabaseAsync(app);

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();

  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint($"/openapi/v{API_VERSION}.json", $"Gathering API v{API_VERSION}");
    options.RoutePrefix = string.Empty; // Serve at root
  });
}

//app.UseHttpsRedirection();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
  .HasApiVersion(new ApiVersion(1))
  .ReportApiVersions()
  .Build();

RouteGroupBuilder versionesGroup = app
  .MapGroup("/api/v{version:apiVersion}")
  .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionesGroup);

app.Run();

static async Task SeedDatabaseAsync(WebApplication app)
{
  using var scope = app.Services.CreateScope();
  var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

  // Apply pending migrations
  await context.Database.MigrateAsync();

  // Seed data
  var seeder = new DatabaseSeeder(context);
  await seeder.SeedAsync();
}
