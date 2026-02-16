# Copilot Instructions for Gathering API

You are an expert AI software architect assistant. **Always plan before coding**. When asked to implement a feature or make changes, first analyze the requirements, propose an architecture or approach, and then proceed with implementation after the plan is clear.

## Project Overview

**Gathering** is a Community of Practice session management platform. It handles communities and their sessions (talks/presentations).

### Tech Stack

- **Backend**: ASP.NET Core Web API (.NET 10, C# 14)
- **Frontend**: Next.js 15+ with TypeScript (monorepo structure)
- **Database**: SQL Server via Entity Framework Core 10
- **Validation**: FluentValidation 12
- **API Versioning**: Asp.Versioning 9
- **Documentation**: OpenAPI/Swagger (via `Microsoft.AspNetCore.OpenApi`)

### Domain Model

- **Community**: Id (Guid), Name, Description, Image (optional), CreatedAt, UpdatedAt, List of Sessions
- **Session**: Id (Guid), Title, Description (optional), Speaker, ScheduledAt (DateTimeOffset), Status (Scheduled, Completed, Canceled), CommunityId
- **SessionResource**: Id (Guid), Title, Url, Type, SessionId

---

## Architecture Guidelines

### Clean Architecture Implementation

The project follows Clean Architecture with these strictly separated layers:

```
Gathering.SharedKernel   → Result<T>, Error, ErrorType, Base Entities (shared primitives)
Gathering.Domain         → Entities, Value Objects, Domain Errors, Repository Interfaces
Gathering.Application    → Commands/Queries (CQRS), Handlers, Validators, DTOs, Abstractions
Gathering.Infrastructure → EF Core DbContext, Repositories, Persistence, Time Provider
Gathering.Api            → Minimal API Endpoints, Extensions, Middleware, Configuration
```

**Dependency Flow**:

```
Api → Application → Domain
Api → Infrastructure → {Application, Domain}
Infrastructure → {Application, Domain}
Application → Domain
Domain → SharedKernel
```

---

## Coding Standards

### C# Best Practices

1. **Use C# 14 features**:
   - File-scoped namespaces
   - Primary constructors for dependency injection
   - Collection expressions `[]` instead of `new List<>()`
   - Required members for DTOs
   - `sealed` classes by default

2. **Record types** for:
   - Commands/Queries
   - DTOs (request/response)
   - Value Objects (when immutable)

3. **Minimal APIs** structure:

   ```csharp
   public class MyEndpoint : IEndpoint
   {
       public void MapEndpoint(IEndpointRouteBuilder app)
       {
           app.MapPost("/resource", async (Request request, ISender sender, CancellationToken cancellationToken) =>
           {
               var command = new MyCommand(request.Data);
               await sender.Send(command, cancellationToken);
               return Results.Ok();
           })
           .WithTags(ApiTags.TagName);
       }
   }
   ```

4. **Nullable reference types**: Always enabled, respect nullability

5. **Async/await**: Use `Task<T>` for async operations, always accept `CancellationToken`

### SOLID & DDD Principles

- **Entities**: Rich domain models with factory methods (`Create()`), private setters
- **Value Objects**: Immutable, equality by value, validation in constructor
- **Domain Errors**: Static readonly `Error` instances per aggregate (e.g., `CommunityError.NotFound`)
- **Repositories**: Interface in Domain, implementation in Infrastructure
- **Validators**: FluentValidation classes in Application layer
- **Result Pattern**: Use `Result<T>` from SharedKernel for error handling

### Database Conventions

| Item         | Convention         | Example                      |
| ------------ | ------------------ | ---------------------------- |
| Tables       | PascalCase         | `Communities`, `Sessions`    |
| Columns      | PascalCase         | `CommunityId`, `ScheduledAt` |
| Foreign Keys | `{Entity}Id`       | `CommunityId`                |
| Indexes      | `IX_{Table}_{Col}` | `IX_Sessions_CommunityId`    |
| Audit fields | PascalCase         | `CreatedAt`, `UpdatedAt`     |

---

## Key Project Files

### Program.cs Structure

```csharp
const int API_VERSION = 1;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddPresentationServices(API_VERSION)
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());
builder.Services.AddOpenApi();

var app = builder.Build();

await SeedDatabaseAsync(app); // Development only

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint($"/openapi/v{API_VERSION}.json", $"Gathering API v{API_VERSION}");
        options.RoutePrefix = string.Empty;
    });
}

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(API_VERSION))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("/api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);

app.Run();
```

### ApplicationDbContext

```csharp
public class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IDateTimeProvider? _dateTimeProvider;

    public DbSet<Community> Communities { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<SessionResource> SessionResources { get; set; }

    public ApplicationDbContext(DbContextOptions options, IDateTimeProvider? dateTimeProvider = null)
        : base(options)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }
}
```

### Entity Configuration Example

```csharp
internal sealed class CommunityConfiguration : IEntityTypeConfiguration<Community>
{
    public void Configure(EntityTypeBuilder<Community> builder)
    {
        builder.ToTable("Communities");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Description).IsRequired().HasMaxLength(1000);
        builder.HasMany(c => c.Sessions)
               .WithOne()
               .HasForeignKey(s => s.CommunityId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.Name);
    }
}
```

---

# English Copy Policy

- All user-facing strings produced by backend responses, seed data, or error messages must be in English. Matching the frontend copy (see `frontend/src/components/sessions/session-resource-list.tsx`), prefer phrases like “Supporting materials”, “Available resources”, and “shared”.

## When Implementing Features

1. **Domain First**:
   - Add/update entities in `Gathering.Domain/{Aggregate}/`
   - Define domain errors as static `Error` instances
   - Create repository interfaces if needed

2. **Application Layer**:
   - Create Command/Query record in `Gathering.Application/{Feature}/{ActionCommand|Query}.cs`
   - Implement handler (`IRequestHandler<TCommand, Result<TResponse>>`)
   - Add FluentValidation validator if needed

3. **Infrastructure**:
   - Implement repository methods
   - Add EF Core configurations if new entity
   - Create/update migrations

4. **API Layer**:
   - Create endpoint class in `Gathering.Api/Endpoints/{Feature}/{Action}.cs`
   - Map Result errors to HTTP status codes
   - Add XML docs for OpenAPI

---

## Patterns to Follow

### Command/Query Structure

```csharp
// Command
public sealed record CreateCommunityCommand(string Name, string Description, string Image)
    : ICommand;

// Handler
internal sealed class CreateCommunityCommandHandler(ICommunityRepository repository)
    : ICommandHandler<CreateCommunityCommand>
{
    public async Task<Result> Handle(CreateCommunityCommand request, CancellationToken cancellationToken)
    {
        Result<Community> communityResult = Community.Create(request.Name, request.Description, request.Image);

        if (communityResult.IsFailure)
        {
            return Result.Failure(communityResult.Error);
        }

        repository.Add(communityResult.Value);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

// Validator
public sealed class CreateCommunityCommandValidator : AbstractValidator<CreateCommunityCommand>
{
    public CreateCommunityCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}
```

### Endpoint Pattern

```csharp
public class Create : IEndpoint
{
    public sealed record Request(string Name, string Description, string Image);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/communities", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateCommunityCommand(request.Name, request.Description, request.Image);

            Result result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? Results.Ok()
                : Results.BadRequest(result.Error);
        })
        .WithTags(ApiTags.Community);
    }
}
```

### Entity Factory Method Pattern

```csharp
public class Community : AuditableEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Image { get; private set; }
    public List<Session> Sessions { get; private set; } = [];

    private Community() { }

    public static Result<Community> Create(string name, string description, string? image = null)
    {
        if (string.IsNullOrWhiteSpace(name.Trim()))
        {
            return Result.Failure<Community>(CommunityError.NameEmpty);
        }

        if (name.Length > 200)
        {
            return Result.Failure<Community>(CommunityError.NameTooLong);
        }

        var community = new Community
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Image = image
        };

        return Result.Success(community);
    }

    public Result Update(string name, string description, string? image)
    {
        // Validation and update logic
        Name = name;
        Description = description;
        Image = image;

        return Result.Success();
    }
}
```

---

## Avoid

- ❌ Business logic in endpoints (keep them thin)
- ❌ Throwing exceptions for expected failures (use `Result<T>`)
- ❌ Anemic domain models (entities should have behavior)
- ❌ Direct `DbContext` usage outside repositories
- ❌ Ignoring `CancellationToken` parameters
- ❌ Mutable DTOs (use `record` with immutable properties)
- ❌ Magic strings (use constants/enums)
- ❌ Large classes (keep under 150 lines, extract collaborators)

---

## Common Questions

**Q: How to add a new endpoint?**  
A: Create a class implementing `IEndpoint` in `Gathering.Api/Endpoints/{Feature}/`, implement `MapEndpoint`, it will be auto-registered.

**Q: How to handle validation errors?**  
A: FluentValidation is integrated with MediatR pipeline. Validation failures return `Result.Failure()` with validation errors.

**Q: How to version APIs?**  
A: Use API versioning via route template `/api/v{version:apiVersion}/...` with `ApiVersionSet`.

**Q: How to seed data?**  
A: Use `SeedDatabaseAsync()` in `Program.cs` (development only).

**Q: How to add a migration?**  
A: From project root:

```bash
cd backend
dotnet ef migrations add MigrationName --project src/Gathering.Infrastructure --startup-project src/Gathering.Api
dotnet ef database update --project src/Gathering.Infrastructure --startup-project src/Gathering.Api
```
