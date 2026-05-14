using Luga.BuildingBlocks.Domain.Common;
using Luga.BuildingBlocks.Server.Http;
using Luga.Modules.Marketing.Server.Application;
using Luga.Modules.Marketing.Shared.DTOs;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Luga.Modules.Marketing.Server.Api;

/// <summary>
/// Anonymous endpoints powering the public site (landing, pricing, contact).
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/marketing")]
public sealed class MarketingController(ISender mediator) : ControllerBase
{
    private readonly ISender _mediator = mediator;

    [HttpGet("plans")]
    [ProducesResponseType(typeof(IReadOnlyList<PublicPlanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans(CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<PublicPlanDto>> result =
            await _mediator.Send(new GetPublicPlansQuery(), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult();
    }

    [HttpPost("contact")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Contact(
        [FromBody] ContactRequestDto request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        SubmitContactCommand command = new(request.Name, request.Email, request.Company, request.Message);
        Result result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return result.ToActionResult();
    }
}
