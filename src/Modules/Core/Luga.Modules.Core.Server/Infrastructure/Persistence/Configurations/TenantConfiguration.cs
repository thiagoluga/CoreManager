using Luga.Modules.Core.Server.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luga.Modules.Core.Server.Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("tenants");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(120);
        builder.Property(t => t.Slug).IsRequired().HasMaxLength(60);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(t => t.DefaultCulture).IsRequired().HasMaxLength(16);

        builder.Property(t => t.CreatedByUsername).HasMaxLength(256);
        builder.Property(t => t.UpdatedByUsername).HasMaxLength(256);
        builder.Property(t => t.DeletedByUsername).HasMaxLength(256);
        builder.Property(t => t.DeletionReason).HasMaxLength(512);

        builder.HasIndex(t => t.Slug).IsUnique();
        builder.Ignore(t => t.DomainEvents);
    }
}
