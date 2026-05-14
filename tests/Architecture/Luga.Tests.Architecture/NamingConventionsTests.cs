using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

using Luga.BuildingBlocks.IntegrationEvents;

using MediatR;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Luga.Tests.Architecture;

/// <summary>
/// Enforces the naming conventions from CLAUDE.md §12.1 across module assemblies.
/// </summary>
public sealed class NamingConventionsTests
{
    [Fact]
    public void IntegrationEvents_AreVersioned()
    {
        // CLAUDE.md §7.17: every IIntegrationEvent type ends in V{N}.
        IArchRule rule = Classes()
            .That().ImplementInterface(typeof(IIntegrationEvent))
            .Should().HaveNameMatching(@"^[A-Z][A-Za-z0-9]+V\d+$")
            .Because("Integration events must carry an explicit V{N} suffix (CLAUDE.md §3.4).");

        rule.Check(ArchitectureFixture.Architecture);
    }

    [Fact]
    public void RequestHandlers_HaveHandlerSuffix()
    {
        // Anything implementing MediatR's IRequestHandler<,> must end with "Handler".
        IObjectProvider<Class> handlers = Classes()
            .That().ImplementInterface(typeof(IRequestHandler<,>))
            .Or().ImplementInterface(typeof(IRequestHandler<>))
            .As("MediatR request handlers");

        IArchRule rule = Classes()
            .That().Are(handlers)
            .Should().HaveNameEndingWith("Handler")
            .Because("MediatR handlers end with 'Handler' (CLAUDE.md §12.1).");

        rule.Check(ArchitectureFixture.Architecture);
    }
}
