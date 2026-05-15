using ArchUnitNET.Loader;
using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Client.Manifests;
using Luga.BuildingBlocks.Domain.Entities;
using Luga.BuildingBlocks.Infrastructure.Persistence;
using Luga.BuildingBlocks.IntegrationEvents;
using Luga.BuildingBlocks.Server.Modules;
using Luga.Modules.Core.Contracts;
using Luga.Modules.Core.Server;
using ReflectionAssembly = System.Reflection.Assembly;

namespace Luga.Tests.Architecture;

/// <summary>
/// Loaded once per test session — ArchUnitNET parses every assembly we care about
/// up front and shares the model across tests via the static <see cref="Architecture"/>.
/// </summary>
public static class ArchitectureFixture
{
    public static readonly ReflectionAssembly DomainAssembly = typeof(EntityBase).Assembly;
    public static readonly ReflectionAssembly ApplicationAssembly = typeof(IUnitOfWork).Assembly;
    public static readonly ReflectionAssembly InfrastructureAssembly = typeof(LugaDbContextBase).Assembly;
    public static readonly ReflectionAssembly IntegrationEventsAssembly = typeof(IIntegrationEvent).Assembly;
    public static readonly ReflectionAssembly ServerAssembly = typeof(IModuleInitializer).Assembly;
    public static readonly ReflectionAssembly ClientAssembly = typeof(IModuleManifest).Assembly;

    public static readonly ReflectionAssembly CoreServerAssembly = typeof(CoreServerModule).Assembly;
    public static readonly ReflectionAssembly CoreContractsAssembly = typeof(ITenantsService).Assembly;

    public static readonly ArchUnitNET.Domain.Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            DomainAssembly,
            ApplicationAssembly,
            InfrastructureAssembly,
            IntegrationEventsAssembly,
            ServerAssembly,
            ClientAssembly,
            CoreServerAssembly,
            CoreContractsAssembly)
        .Build();
}
