using Luga.Modules.Payments.Server.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luga.Modules.Payments.Server.Infrastructure.Persistence.Configurations;

public sealed class TenantPlanConfiguration : IEntityTypeConfiguration<TenantPlan>
{
    public void Configure(EntityTypeBuilder<TenantPlan> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("tenant_plans");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.Amount).HasPrecision(10, 2);
        builder.Property(p => p.CreatedByUsername).HasMaxLength(256);
        builder.Property(p => p.UpdatedByUsername).HasMaxLength(256);
        builder.Property(p => p.DeletedByUsername).HasMaxLength(256);
        builder.Property(p => p.DeletionReason).HasMaxLength(2000);
        builder.HasIndex(p => new { p.TenantId, p.IsActive });
    }
}
