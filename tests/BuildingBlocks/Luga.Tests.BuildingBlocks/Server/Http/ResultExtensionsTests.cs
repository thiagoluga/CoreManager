using Luga.BuildingBlocks.Domain.Common;
using Luga.BuildingBlocks.Domain.Errors;
using Luga.BuildingBlocks.Server.Http;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Luga.Tests.BuildingBlocks.Server.Http;

public sealed class ResultExtensionsTests
{
    [Fact]
    public void Success_TypedResult_MapsTo200Ok()
    {
        Result<int> result = Result.Success(42);

        IActionResult action = result.ToActionResult();

        action.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(42);
    }

    [Fact]
    public void Success_NonGenericResult_MapsTo204NoContent()
    {
        Result result = Result.Success();

        IActionResult action = result.ToActionResult();

        action.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void CreatedActionResult_OnSuccess_Returns201WithLocation()
    {
        Result<string> result = Result.Success("created-value");

        IActionResult action = result.ToCreatedActionResult("/api/customers/abc");

        var created = action.Should().BeOfType<CreatedResult>().Subject;
        created.Location.Should().Be("/api/customers/abc");
        created.Value.Should().Be("created-value");
    }

    [Theory]
    [InlineData("Customer.NotFound", StatusCodes.Status404NotFound)]
    [InlineData("General.Unauthorized", StatusCodes.Status401Unauthorized)]
    [InlineData("General.Forbidden", StatusCodes.Status403Forbidden)]
    [InlineData("General.Conflict", StatusCodes.Status409Conflict)]
    [InlineData("General.Validation", StatusCodes.Status422UnprocessableEntity)]
    [InlineData("General.Concurrency", StatusCodes.Status412PreconditionFailed)]
    [InlineData("General.Unexpected", StatusCodes.Status500InternalServerError)]
    public void Failure_MapsErrorSuffixToHttpStatus(string code, int expected)
    {
        Result<int> result = Result.Failure<int>(new Error(code, "msg"));

        IActionResult action = result.ToActionResult();

        var problem = action.Should().BeOfType<ObjectResult>().Subject;
        problem.StatusCode.Should().Be(expected);
        var details = problem.Value.Should().BeOfType<ProblemDetails>().Subject;
        details.Title.Should().Be(code);
        details.Detail.Should().Be("msg");
        details.Status.Should().Be(expected);
    }

    [Fact]
    public void Failure_WithUnknownSuffix_DefaultsTo400BadRequest()
    {
        Result result = Result.Failure(GeneralErrors.Validation("dummy"));

        IActionResult action = result.ToActionResult();

        // .Validation suffix is mapped to 422 — sanity check using a known code first.
        action.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);

        // Now an entirely unknown shape:
        Result unknown = Result.Failure(new Error("Something.Weird", "msg"));
        IActionResult unknownAction = unknown.ToActionResult();
        unknownAction.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}
