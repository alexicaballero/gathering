using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Gathering.Infrastructure;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
  public ApplicationDbContext CreateDbContext(string[] args)
  {
    var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
      if (OperatingSystem.IsWindows())
      {
        connectionString = "Server=(localdb)\\mssqllocaldb;Database=GatheringDb;Trusted_Connection=True;MultipleActiveResultSets=True;";
      }
      else
      {
        throw new InvalidOperationException(
          "Environment variable 'ConnectionStrings__DefaultConnection' is not set. " +
          "Set the DATABASE_CONNECTION_STRING secret in GitHub Actions.");
      }
    }

    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
    optionsBuilder.UseSqlServer(connectionString);

    return new ApplicationDbContext(optionsBuilder.Options);
  }
}
