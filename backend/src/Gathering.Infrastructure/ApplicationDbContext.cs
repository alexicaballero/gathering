using Gathering.Domain.Abstractions;
using Gathering.Domain.Communities;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Gathering.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IDateTimeProvider? _dateTimeProvider;

    public DbSet<Community> Communities { get; set; }

    public DbSet<Session> Sessions { get; set; }

    public DbSet<SessionResource> SessionResources { get; set; }

    public ApplicationDbContext(DbContextOptions options, IDateTimeProvider? dateTimeProvider = null) : base(options)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        ApplyAuditInfo();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInfo()
    {
        var now = _dateTimeProvider?.UtcNow ?? DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = now;
                entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = null;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = now;
            }
        }
    }
}
