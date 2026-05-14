using Luga.Modules.Core.Server.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luga.Modules.Core.Server.Infrastructure.Persistence.Configurations;

public sealed class TenantUserConfiguration : IEntityTypeConfiguration<TenantUser>
{
    public void Configure(EntityTypeBuilder<TenantUser> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("tenant_users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username).IsRequired().HasMaxLength(256);
        builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(120);
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(32);
        builder.Property(u => u.PreferredCulture).IsRequired().HasMaxLength(16);

        builder.Property(u => u.CreatedByUsername).HasMaxLength(256);
        builder.Property(u => u.UpdatedByUsername).HasMaxLength(256);
        builder.Property(u => u.DeletedByUsername).HasMaxLength(256);
        builder.Property(u => u.DeletionReason).HasMaxLength(512);

        // Unique username per tenant — same email may exist in multiple tenants as separate users.
        builder.HasIndex(u => new { u.TenantId, u.Username }).IsUnique();
        builder.Ignore(u => u.DomainEvents);
    }
}
