using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luga.BuildingBlocks.Infrastructure.Persistence.Outbox;

public sealed class ProcessedIntegrationEventConfiguration : IEntityTypeConfiguration<ProcessedIntegrationEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedIntegrationEvent> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("processed_integration_events");
        builder.HasKey(p => new { p.EventId, p.HandlerName });

        builder.Property(p => p.HandlerName).IsRequired().HasMaxLength(256);
        builder.Property(p => p.ProcessedOn).IsRequired();
    }
}
