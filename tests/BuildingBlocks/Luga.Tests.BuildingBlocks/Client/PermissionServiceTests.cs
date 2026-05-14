using Luga.BuildingBlocks.Client.Auth;

namespace Luga.Tests.BuildingBlocks.Client;

public sealed class PermissionServiceTests
{
    [Fact]
    public void Empty_ByDefault()
    {
        PermissionService service = new();

        service.Permissions.Should().BeEmpty();
        service.HasPermission("anything").Should().BeFalse();
    }

    [Fact]
    public void SetPermissions_ReplacesPreviousSet()
    {
        PermissionService service = new();
        service.SetPermissions(["customers.read", "customers.write"]);
        service.SetPermissions(["customers.read"]);

        service.Permissions.Should().BeEquivalentTo(new[] { "customers.read" });
        service.HasPermission("customers.write").Should().BeFalse();
    }

    [Fact]
    public void HasPermission_IsCaseSensitive()
    {
        PermissionService service = new();
        service.SetPermissions(["customers.read"]);

        service.HasPermission("customers.read").Should().BeTrue();
        service.HasPermission("Customers.Read").Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void HasPermission_NullOrWhitespace_ReturnsFalse(string? input)
    {
        PermissionService service = new();
        service.SetPermissions(["customers.read"]);

        service.HasPermission(input!).Should().BeFalse();
    }
}
