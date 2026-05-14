using Luga.Modules.Payments.Server.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luga.Modules.Payments.Server.Infrastructure.Persistence.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("invoices");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.Amount).HasPrecision(10, 2);
        builder.Property(i => i.Notes).HasMaxLength(2000);
        builder.Property(i => i.CreatedByUsername).HasMaxLength(256);
        builder.Property(i => i.UpdatedByUsername).HasMaxLength(256);
        builder.Property(i => i.DeletedByUsername).HasMaxLength(256);
        builder.Property(i => i.DeletionReason).HasMaxLength(2000);
        builder.HasIndex(i => new { i.TenantId, i.Status });
        builder.HasIndex(i => new { i.TenantId, i.DueDate });
        builder.HasIndex(i => new { i.TenantId, i.CustomerId });
    }
}
