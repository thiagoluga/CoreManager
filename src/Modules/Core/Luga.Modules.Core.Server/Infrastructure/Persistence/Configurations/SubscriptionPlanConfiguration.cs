using Luga.Modules.Core.Server.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luga.Modules.Core.Server.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("subscription_plans");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(p => p.Code).IsUnique();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(120);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(2000);
        builder.Property(p => p.MonthlyPrice).HasPrecision(10, 2);
        builder.Property(p => p.AnnualPrice).HasPrecision(10, 2);
        builder.Property(p => p.CreatedByUsername).HasMaxLength(256);
        builder.Property(p => p.UpdatedByUsername).HasMaxLength(256);
        builder.Property(p => p.DeletedByUsername).HasMaxLength(256);
        builder.Property(p => p.DeletionReason).HasMaxLength(2000);

        // Comma-joined module list — readable in SQL, no separate table required for the MVP.
        builder.Property(p => p.IncludedModules)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Metadata.SetValueComparer(
                new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IList<string>>(
                    (a, b) => a!.SequenceEqual(b!),
                    v => v.Aggregate(0, (acc, s) => HashCode.Combine(acc, s.GetHashCode(StringComparison.Ordinal))),
                    v => v.ToList()));

        builder.Property(p => p.IsPublic).HasDefaultValue(true);
        builder.Property(p => p.IsHighlighted).HasDefaultValue(false);
        builder.Property(p => p.DisplayOrder).HasDefaultValue(0);
    }
}
