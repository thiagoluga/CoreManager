using Microsoft.Extensions.DependencyInjection;

namespace Luga.BuildingBlocks.Server.Modules;

/// <summary>
/// Scoped helper passed to <see cref="IModuleInitializer.InitializeAsync"/>.
/// Provides the per-run DI scope so initializers can resolve repositories and
/// the module's <c>DbContext</c>.
/// </summary>
public sealed class InitializationContext(IServiceProvider services)
{
    /// <summary>Scoped service provider for resolving repositories / DbContexts.</summary>
    public IServiceProvider Services { get; } = services;

    /// <summary>Resolves a required service from the scope.</summary>
    public T GetRequiredService<T>()
        where T : notnull => Services.GetRequiredService<T>();
}
