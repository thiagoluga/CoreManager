using Luga.BuildingBlocks.Domain.Common;

namespace Luga.Tests.BuildingBlocks.Domain.Common;

public sealed class ResultTests
{
    [Fact]
    public void Success_IsSuccessIsTrue_ErrorIsNone()
    {
        Result result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_IsSuccessIsFalse_CarriesError()
    {
        Error error = new("X.Bad", "ruim");

        Result result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Failure_WithErrorNone_Throws()
    {
        Action act = () => Result.Failure(Error.None);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Generic_Success_ExposesValue()
    {
        Result<int> result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Generic_Failure_AccessingValueThrows()
    {
        Result<int> result = Result.Failure<int>(new Error("X.Bad", "ruim"));

        Action act = () => _ = result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Generic_ImplicitConversionFromValue_ProducesSuccess()
    {
        Result<string> result = "ok";

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
    }

    [Fact]
    public void Generic_ImplicitConversionFromError_ProducesFailure()
    {
        Error error = new("X.Bad", "ruim");

        Result<string> result = error;

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Generic_ImplicitConversionFromNullReference_ProducesFailureWithNullValue()
    {
        string? value = null;

        Result<string> result = value;

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }
}
