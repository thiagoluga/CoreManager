using Luga.Modules.Payments.Server.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luga.Modules.Payments.Server.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("subscriptions");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.CreatedByUsername).HasMaxLength(256);
        builder.Property(s => s.UpdatedByUsername).HasMaxLength(256);
        builder.Property(s => s.DeletedByUsername).HasMaxLength(256);
        builder.Property(s => s.DeletionReason).HasMaxLength(2000);
        builder.HasIndex(s => new { s.TenantId, s.CustomerId });
        builder.HasIndex(s => new { s.TenantId, s.TenantPlanId });
    }
}
