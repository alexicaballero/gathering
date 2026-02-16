using Gathering.Domain.Sessions;
using Microsoft.EntityFrameworkCore;

namespace Gathering.Infrastructure.Configurations;

public class SessionResourceConfiguration : IEntityTypeConfiguration<SessionResource>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<SessionResource> builder)
  {
    builder.ToTable("SessionResources");

    builder.HasKey(r => r.Id);

    builder.Property(r => r.Type)
        .IsRequired();

    builder.Property(r => r.Title)
        .HasMaxLength(200);

    builder.Property(r => r.Url)
        .HasMaxLength(2000);

    builder.Property(r => r.Notes)
        .HasMaxLength(4000);

    builder.Property(r => r.CreatedAt)
        .IsRequired();

    builder.Property(r => r.UpdatedAt);

    builder.HasIndex(r => new { r.SessionId, r.Type });

    builder.HasOne<Session>()
        .WithMany(s => s.Resources)
        .HasForeignKey(r => r.SessionId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
