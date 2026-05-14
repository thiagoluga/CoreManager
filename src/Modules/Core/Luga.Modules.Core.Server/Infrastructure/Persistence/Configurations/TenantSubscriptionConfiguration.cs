using Luga.Modules.Core.Server.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luga.Modules.Core.Server.Infrastructure.Persistence.Configurations;

public sealed class TenantSubscriptionConfiguration : IEntityTypeConfiguration<TenantSubscription>
{
    public void Configure(EntityTypeBuilder<TenantSubscription> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("tenant_subscriptions");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.PlanCode).IsRequired().HasMaxLength(50);
        builder.Property(s => s.PlanName).IsRequired().HasMaxLength(120);
        builder.Property(s => s.CreatedByUsername).HasMaxLength(256);
        builder.Property(s => s.UpdatedByUsername).HasMaxLength(256);
        builder.Property(s => s.DeletedByUsername).HasMaxLength(256);
        builder.Property(s => s.DeletionReason).HasMaxLength(2000);

        builder.HasIndex(s => new { s.TenantId, s.Status });

        builder.Property(s => s.ActiveModules)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Metadata.SetValueComparer(
                new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IList<string>>(
                    (a, b) => a!.SequenceEqual(b!),
                    v => v.Aggregate(0, (acc, s) => HashCode.Combine(acc, s.GetHashCode(StringComparison.Ordinal))),
                    v => v.ToList()));
    }
}
