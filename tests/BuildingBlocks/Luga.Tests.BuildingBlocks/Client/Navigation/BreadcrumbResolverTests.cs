using Luga.BuildingBlocks.Client.Manifests;
using Luga.BuildingBlocks.Client.Navigation;

namespace Luga.Tests.BuildingBlocks.Client.Navigation;

public sealed class BreadcrumbResolverTests
{
    [Fact]
    public void NoMatch_ReturnsEmpty()
    {
        BreadcrumbResolver resolver = new([CustomersManifest()]);

        IReadOnlyList<BreadcrumbSegment> segments = resolver.Resolve("/unknown", null);

        segments.Should().BeEmpty();
    }

    [Fact]
    public void ExactMatch_ReturnsConfiguredSegments()
    {
        BreadcrumbResolver resolver = new([CustomersManifest()]);

        IReadOnlyList<BreadcrumbSegment> segments = resolver.Resolve("/customers", null);

        segments.Select(s => s.LabelKey).Should().Equal("common.home", "customers.list");
    }

    [Fact]
    public void ParameterizedMatch_FillsDynamicLeaf()
    {
        BreadcrumbResolver resolver = new([CustomersManifest()]);

        IReadOnlyList<BreadcrumbSegment> segments = resolver.Resolve(
            "/customers/3b7e69df-2a25-4d2f-8b25-93f33b1aaaaa",
            "Customer John Smith");

        segments.Select(s => s.LabelKey).Should().Equal(
            "common.home",
            "customers.list",
            "Customer John Smith");
    }

    [Fact]
    public void DisabledRoutes_AreIgnored()
    {
        BreadcrumbResolver resolver = new(
        [
            new TestManifest("test", "Test", 0, breadcrumbs:
            [
                new BreadcrumbRoute(
                    "/disabled",
                    [new BreadcrumbSegment("disabled.label")],
                    IsEnabled: false),
            ]),
        ]);

        resolver.Resolve("/disabled", null).Should().BeEmpty();
    }

    [Theory]
    [InlineData("/customers/123/edit", true)]
    [InlineData("/customers/123", false)]
    public void SegmentCount_MustMatchPattern(string route, bool shouldMatchEdit)
    {
        BreadcrumbResolver resolver = new(
        [
            new TestManifest("customers", "customers.title", 0, breadcrumbs:
            [
                new BreadcrumbRoute(
                    "/customers/{id:guid}/edit",
                    [new BreadcrumbSegment("customers.edit")]),
            ]),
        ]);

        IReadOnlyList<BreadcrumbSegment> result = resolver.Resolve(route, null);

        if (shouldMatchEdit)
        {
            result.Should().NotBeEmpty();
        }
        else
        {
            result.Should().BeEmpty();
        }
    }

    private static IModuleManifest CustomersManifest() =>
        new TestManifest(
            moduleCode: "customers",
            displayNameKey: "customers.title",
            order: 0,
            breadcrumbs:
            [
                new BreadcrumbRoute(
                    "/customers",
                    [
                        new BreadcrumbSegment("common.home", Href: "/"),
                        new BreadcrumbSegment("customers.list"),
                    ]),
                new BreadcrumbRoute(
                    "/customers/{id:guid}",
                    [
                        new BreadcrumbSegment("common.home", Href: "/"),
                        new BreadcrumbSegment("customers.list", Href: "/customers"),
                        new BreadcrumbSegment("customers.detail", Source: BreadcrumbSegmentSource.Dynamic),
                    ]),
            ]);

    private sealed class TestManifest(
        string moduleCode,
        string displayNameKey,
        int order,
        IReadOnlyList<BreadcrumbRoute>? breadcrumbs = null) : IModuleManifest
    {
        public string ModuleCode => moduleCode;

        public string DisplayNameKey => displayNameKey;

        public string IconName => string.Empty;

        public int Order => order;

        public string? RequiredSubscriptionModule => null;

        public IReadOnlyList<MenuItem> MenuItems => [];

        public IReadOnlyList<DashboardWidget> Widgets => [];

        public IReadOnlyList<BreadcrumbRoute> BreadcrumbRoutes => breadcrumbs ?? [];

        public IReadOnlyList<EmbeddableComponent> EmbeddableComponents => [];
    }
}
