using FluentValidation;
using FluentValidation.Results;

using Luga.BuildingBlocks.Application.Behaviors;
using Luga.BuildingBlocks.Domain.Common;

using MediatR;

using Moq;

namespace Luga.Tests.BuildingBlocks.Application.Behaviors;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task NoValidators_PassesThroughToHandler()
    {
        ValidationBehavior<DummyRequest, Result<int>> behavior = new(validators: []);

        Result<int> response = await behavior.Handle(
            new DummyRequest("ok"),
            () => Task.FromResult(Result.Success(7)),
            CancellationToken.None);

        response.IsSuccess.Should().BeTrue();
        response.Value.Should().Be(7);
    }

    [Fact]
    public async Task AllValidatorsPass_HandlerIsInvoked()
    {
        Mock<IValidator<DummyRequest>> validator = new();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DummyRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        ValidationBehavior<DummyRequest, Result<int>> behavior = new([validator.Object]);

        Result<int> response = await behavior.Handle(
            new DummyRequest("ok"),
            () => Task.FromResult(Result.Success(42)),
            CancellationToken.None);

        response.IsSuccess.Should().BeTrue();
        response.Value.Should().Be(42);
    }

    [Fact]
    public async Task ValidationFails_AndResponseIsResult_ReturnsFailureResult()
    {
        Mock<IValidator<DummyRequest>> validator = new();
        ValidationFailure failure = new("Name", "is required");
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DummyRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([failure]));

        ValidationBehavior<DummyRequest, Result<int>> behavior = new([validator.Object]);

        bool handlerInvoked = false;

        Result<int> response = await behavior.Handle(
            new DummyRequest(string.Empty),
            () =>
            {
                handlerInvoked = true;
                return Task.FromResult(Result.Success(42));
            },
            CancellationToken.None);

        response.IsFailure.Should().BeTrue();
        response.Error.Code.Should().Be("General.Validation");
        response.Error.Message.Should().Contain("Name");
        handlerInvoked.Should().BeFalse();
    }

    [Fact]
    public async Task ValidationFails_AndResponseIsNonGenericResult_ReturnsFailureResult()
    {
        Mock<IValidator<DummyRequest>> validator = new();
        ValidationFailure failure = new("Name", "is required");
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DummyRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([failure]));

        ValidationBehavior<DummyRequest, Result> behavior = new([validator.Object]);

        Result response = await behavior.Handle(
            new DummyRequest(string.Empty),
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        response.IsFailure.Should().BeTrue();
        response.Error.Code.Should().Be("General.Validation");
    }

    [Fact]
    public async Task ValidationFails_AndResponseIsNotResult_ThrowsValidationException()
    {
        Mock<IValidator<DummyRequest>> validator = new();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DummyRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("X", "Y")]));

        ValidationBehavior<DummyRequest, int> behavior = new([validator.Object]);

        Func<Task> act = () => behavior.Handle(
            new DummyRequest(string.Empty),
            () => Task.FromResult(1),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    public sealed record DummyRequest(string Name) : IRequest<Result<int>>;
}
