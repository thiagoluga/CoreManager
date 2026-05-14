using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Infrastructure.Events;
using Luga.BuildingBlocks.Infrastructure.Idempotency;
using Luga.BuildingBlocks.Infrastructure.Persistence.DbContextInterceptors;
using Luga.BuildingBlocks.Infrastructure.Persistence.Migrations;
using Luga.BuildingBlocks.IntegrationEvents;

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Luga.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Wires the cross-cutting persistence pieces shared by every module:
/// EF Core interceptors, integration-event bus, processed-event store, idempotency
/// store, migration / initializer runners. Each module then registers its own
/// <c>DbContext</c> and repositories via its <c>X.ServerModule</c> entry point.
/// </summary>
public static class PersistenceServiceCollectionExtensions
{
    /// <summary>
    /// Registers the interceptors and shared persistence services. Idempotent.
    /// </summary>
    public static IServiceCollection AddLugaPersistence(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton(TimeProvider.System);

        // Interceptors are scoped — they pull from ICurrentUser / ITenantContext.
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, TenantIdInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, ActivationTrackingInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DomainEventToOutboxInterceptor>();

        // Cross-cutting infrastructure.
        services.AddSingleton<IIntegrationEventBus, InProcessIntegrationEventBus>();
        services.AddScoped<DomainEventDispatcher>();
        services.AddScoped<IProcessedEventStore, ProcessedEventStore>();
        services.AddScoped<IIdempotencyStore, IdempotencyStore>();

        services.AddSingleton<ModuleMigrationRunner>();

        return services;
    }
}
