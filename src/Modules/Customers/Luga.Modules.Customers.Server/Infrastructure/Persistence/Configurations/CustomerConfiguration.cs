using System.Text.Json;

using Luga.Modules.Customers.Server.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luga.Modules.Customers.Server.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("customers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.DisplayName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(256);
        builder.Property(c => c.Phone).HasMaxLength(32);
        builder.Property(c => c.Document).HasMaxLength(20);
        builder.Property(c => c.Notes).HasMaxLength(4000);

        builder.Property(c => c.CreatedByUsername).HasMaxLength(256);
        builder.Property(c => c.UpdatedByUsername).HasMaxLength(256);
        builder.Property(c => c.DeletedByUsername).HasMaxLength(256);
        builder.Property(c => c.DeletionReason).HasMaxLength(2000);

        // Custom fields persisted as a single JSON column (ADR 030).
        ValueComparer<IDictionary<string, string>> comparer = new(
            (a, b) => ReferenceEquals(a, b) || (a != null && b != null && a.SequenceEqual(b)),
            v => v.Aggregate(0, (acc, kv) => HashCode.Combine(acc, kv.Key, kv.Value)),
            v => new Dictionary<string, string>(v, StringComparer.Ordinal));

        builder.Property(c => c.CustomFields)
            .HasColumnName("custom_fields")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrEmpty(v)
                    ? new Dictionary<string, string>(StringComparer.Ordinal)
                    : JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null)
                        ?? new Dictionary<string, string>(StringComparer.Ordinal))
            .Metadata.SetValueComparer(comparer);

        builder.HasIndex(c => new { c.TenantId, c.Email });
        builder.HasIndex(c => new { c.TenantId, c.IsActive });
    }
}
