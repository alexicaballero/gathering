using Gathering.Domain.Communities;
using Microsoft.EntityFrameworkCore;

namespace Gathering.Infrastructure.Configurations;

public class CommunityConfiguration : IEntityTypeConfiguration<Community>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Community> builder)
  {
    builder.ToTable("Communities");

    builder.HasKey(c => c.Id);

    builder.Property(c => c.Name)
      .IsRequired()
      .HasMaxLength(200);

    builder.Property(c => c.Description)
      .IsRequired()
      .HasMaxLength(1000);

    builder.Property(c => c.Image)
      .HasMaxLength(1000);

    builder.Property(c => c.CreatedAt)
      .IsRequired();

    builder.Property(c => c.UpdatedAt);

    builder.HasMany(c => c.Sessions);
  }
}
