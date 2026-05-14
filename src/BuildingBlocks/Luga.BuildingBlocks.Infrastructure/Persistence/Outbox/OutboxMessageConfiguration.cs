using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luga.BuildingBlocks.Infrastructure.Persistence.Outbox;

/// <summary>
/// EF configuration for <see cref="OutboxMessage"/>. Each module applies this
/// configuration on its own <c>DbContext</c> so the outbox lives in the module's
/// schema (CLAUDE.md §7.17).
/// </summary>
public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("outbox_messages");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.EventType).IsRequired().HasMaxLength(512);
        builder.Property(m => m.Payload).IsRequired();
        builder.Property(m => m.OccurredOn).IsRequired();
        builder.Property(m => m.Error).HasMaxLength(2048);

        // Index for the processor poll query: WHERE ProcessedOn IS NULL ORDER BY OccurredOn.
        builder.HasIndex(m => new { m.ProcessedOn, m.OccurredOn });

        // Index for idempotency lookups by event id.
        builder.HasIndex(m => m.EventId).IsUnique(false);
    }
}
