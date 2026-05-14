using Luga.BuildingBlocks.Domain.Errors;

namespace Luga.Tests.BuildingBlocks.Domain.Errors;

public sealed class GeneralErrorsTests
{
    [Fact]
    public void NotFound_BuildsCodeWithEntityNamePrefix()
    {
        Guid key = Guid.NewGuid();

        var error = GeneralErrors.NotFound("Customer", key);

        error.Code.Should().Be("Customer.NotFound");
        error.Message.Should().Contain(key.ToString());
    }

    [Fact]
    public void Unauthorized_HasStableCode()
    {
        GeneralErrors.Unauthorized().Code.Should().Be("General.Unauthorized");
    }

    [Fact]
    public void Forbidden_HasStableCode()
    {
        GeneralErrors.Forbidden().Code.Should().Be("General.Forbidden");
    }

    [Fact]
    public void Conflict_PreservesProvidedMessage()
    {
        var error = GeneralErrors.Conflict("Email já está em uso.");

        error.Code.Should().Be("General.Conflict");
        error.Message.Should().Be("Email já está em uso.");
    }

    [Fact]
    public void Concurrency_HasStableCode()
    {
        GeneralErrors.Concurrency().Code.Should().Be("General.Concurrency");
    }
}
