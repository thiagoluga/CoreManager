using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luga.BuildingBlocks.Infrastructure.Idempotency;

public sealed class IdempotencyKeyConfiguration : IEntityTypeConfiguration<IdempotencyKey>
{
    public void Configure(EntityTypeBuilder<IdempotencyKey> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("idempotency_keys", schema: "core");
        builder.HasKey(k => k.Key);

        builder.Property(k => k.Key).HasMaxLength(256);
        builder.Property(k => k.ResponsePayload).IsRequired();
        builder.Property(k => k.CreatedOn).IsRequired();
        builder.Property(k => k.ExpiresOn).IsRequired();

        builder.HasIndex(k => k.ExpiresOn);
    }
}
