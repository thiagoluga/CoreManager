using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luga.BuildingBlocks.Infrastructure.Persistence.Migrations;

public sealed class ModuleInitializationConfiguration : IEntityTypeConfiguration<ModuleInitialization>
{
    public void Configure(EntityTypeBuilder<ModuleInitialization> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("module_initializations", schema: "core");
        builder.HasKey(m => new { m.ModuleCode, m.Version });

        builder.Property(m => m.ModuleCode).IsRequired().HasMaxLength(64);
        builder.Property(m => m.AppliedAt).IsRequired();
        builder.Property(m => m.AppliedBy).IsRequired().HasMaxLength(256);
    }
}
