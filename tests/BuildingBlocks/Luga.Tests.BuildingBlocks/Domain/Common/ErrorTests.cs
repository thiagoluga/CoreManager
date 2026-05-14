using Luga.BuildingBlocks.Domain.Common;

namespace Luga.Tests.BuildingBlocks.Domain.Common;

public sealed class ErrorTests
{
    [Fact]
    public void None_HasEmptyCodeAndMessage()
    {
        Error.None.Code.Should().BeEmpty();
        Error.None.Message.Should().BeEmpty();
    }

    [Fact]
    public void NullValue_IsDistinctFromNone()
    {
        Error.NullValue.Should().NotBe(Error.None);
        Error.NullValue.Code.Should().Be("Error.NullValue");
    }

    [Fact]
    public void Records_WithSameCodeAndMessage_AreEqual()
    {
        Error a = new("Customer.NotFound", "Cliente não encontrado");
        Error b = new("Customer.NotFound", "Cliente não encontrado");

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Records_WithDifferentCodes_AreNotEqual()
    {
        Error a = new("A", "msg");
        Error b = new("B", "msg");

        a.Should().NotBe(b);
    }
}
