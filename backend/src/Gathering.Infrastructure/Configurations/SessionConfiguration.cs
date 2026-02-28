using Gathering.Domain.Communities;
using Gathering.Domain.Sessions;
using Microsoft.EntityFrameworkCore;

namespace Gathering.Infrastructure.Configurations;

internal sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Session> builder)
  {
    builder.ToTable("Sessions");

    builder.HasKey(s => s.Id);

    builder.Property(s => s.Title)
      .IsRequired()
      .HasMaxLength(200);

    builder.Property(s => s.Description)
      .HasMaxLength(1000);

    builder.Property(s => s.Image)
      .HasMaxLength(1000);

    builder.Property(s => s.Speaker)
      .IsRequired()
      .HasMaxLength(200);

    builder.Property(s => s.ScheduledAt)
      .IsRequired();

    builder.Property(s => s.Status)
      .IsRequired();

    builder.Property(s => s.CreatedAt)
      .IsRequired();

    builder.Property(s => s.UpdatedAt);

    builder.HasIndex(s => s.CommunityId);

    builder.Navigation(s => s.Resources)
      .HasField("_resources")
      .UsePropertyAccessMode(PropertyAccessMode.Field);
  }
}
