using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Luga.Tests.Architecture;

/// <summary>
/// Enforces the dependency arrows from CLAUDE.md §7.2. Build fails if a forbidden
/// reference is introduced.
/// </summary>
public sealed class DependencyRulesTests
{
    [Fact]
    public void Domain_DoesNotDependOnEntityFrameworkCore()
    {
        IArchRule rule = Types()
            .That().ResideInAssembly(ArchitectureFixture.DomainAssembly)
            .Should().NotDependOnAny(Types().That().ResideInNamespace(@"^Microsoft\.EntityFrameworkCore.*"))
            .Because("Domain must stay infrastructure-free (CLAUDE.md §7.2).");

        rule.Check(ArchitectureFixture.Architecture);
    }

    [Fact]
    public void Domain_DoesNotDependOnMediatR()
    {
        IArchRule rule = Types()
            .That().ResideInAssembly(ArchitectureFixture.DomainAssembly)
            .Should().NotDependOnAny(Types().That().ResideInNamespace(@"^MediatR.*"))
            .Because("Domain must not couple to mediator/application-layer frameworks.");

        rule.Check(ArchitectureFixture.Architecture);
    }

    [Fact]
    public void Application_DoesNotDependOnInfrastructure()
    {
        IArchRule rule = Types()
            .That().ResideInAssembly(ArchitectureFixture.ApplicationAssembly)
            .Should().NotDependOnAny(Types().That().ResideInAssembly(ArchitectureFixture.InfrastructureAssembly))
            .Because("Application depends on abstractions only (CLAUDE.md §7.2).");

        rule.Check(ArchitectureFixture.Architecture);
    }

    [Fact]
    public void Application_DoesNotDependOnEntityFrameworkCore()
    {
        IArchRule rule = Types()
            .That().ResideInAssembly(ArchitectureFixture.ApplicationAssembly)
            .Should().NotDependOnAny(Types().That().ResideInNamespace(@"^Microsoft\.EntityFrameworkCore.*"))
            .Because("Application stays free of EF Core types (CLAUDE.md §7.2).");

        rule.Check(ArchitectureFixture.Architecture);
    }

    [Fact]
    public void Application_DoesNotDependOnAspNetCoreMvc()
    {
        IArchRule rule = Types()
            .That().ResideInAssembly(ArchitectureFixture.ApplicationAssembly)
            .Should().NotDependOnAny(Types().That().ResideInNamespace(@"^Microsoft\.AspNetCore\.Mvc.*"))
            .Because("Application must not reference HTTP / MVC types (CLAUDE.md §7.2).");

        rule.Check(ArchitectureFixture.Architecture);
    }

    [Fact]
    public void Infrastructure_DoesNotDependOnServerHost()
    {
        IArchRule rule = Types()
            .That().ResideInAssembly(ArchitectureFixture.InfrastructureAssembly)
            .Should().NotDependOnAny(Types().That().ResideInAssembly(ArchitectureFixture.ServerAssembly))
            .Because("Server depends on Infrastructure, never the other way round.");

        rule.Check(ArchitectureFixture.Architecture);
    }

    [Fact]
    public void Client_DoesNotDependOnInfrastructure()
    {
        IArchRule rule = Types()
            .That().ResideInAssembly(ArchitectureFixture.ClientAssembly)
            .Should().NotDependOnAny(Types().That().ResideInAssembly(ArchitectureFixture.InfrastructureAssembly))
            .Because("BuildingBlocks.Client is browser-targeted; no server infra leaks (CLAUDE.md §7.2).");

        rule.Check(ArchitectureFixture.Architecture);
    }

    [Fact]
    public void IntegrationEvents_HasOnlyMinimalDependencies()
    {
        // The Contracts surface must stay tiny so consumers can add the project without
        // dragging the whole back-end stack.
        IArchRule rule = Types()
            .That().ResideInAssembly(ArchitectureFixture.IntegrationEventsAssembly)
            .Should().NotDependOnAny(Types()
                .That().ResideInAssembly(ArchitectureFixture.InfrastructureAssembly)
                .Or().ResideInAssembly(ArchitectureFixture.ApplicationAssembly)
                .Or().ResideInAssembly(ArchitectureFixture.ServerAssembly)
                .Or().ResideInAssembly(ArchitectureFixture.ClientAssembly))
            .Because("IntegrationEvents stays at the bottom of the dependency graph.");

        rule.Check(ArchitectureFixture.Architecture);
    }

    [Fact]
    public void CoreContracts_DoesNotDependOnCoreServer()
    {
        IArchRule rule = Types()
            .That().ResideInAssembly(ArchitectureFixture.CoreContractsAssembly)
            .Should().NotDependOnAny(Types().That().ResideInAssembly(ArchitectureFixture.CoreServerAssembly))
            .Because("Contracts is the stable cross-module surface; it can never know about Server internals.");

        rule.Check(ArchitectureFixture.Architecture);
    }
}
