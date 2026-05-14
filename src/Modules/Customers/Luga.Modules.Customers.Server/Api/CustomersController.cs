using Luga.BuildingBlocks.Domain.Common;
using Luga.BuildingBlocks.Server.Http;
using Luga.Modules.Customers.Server.Application.Features.CreateCustomer;
using Luga.Modules.Customers.Server.Application.Features.DeleteCustomer;
using Luga.Modules.Customers.Server.Application.Features.GetCustomer;
using Luga.Modules.Customers.Server.Application.Features.ListCustomers;
using Luga.Modules.Customers.Server.Application.Features.UpdateCustomer;
using Luga.Modules.Customers.Shared.DTOs;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Luga.Modules.Customers.Server.Api;

[ApiController]
[Authorize]
[Route("api/customers")]
public sealed class CustomersController(ISender mediator) : ControllerBase
{
    private readonly ISender _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(PagedCustomersResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        Result<PagedCustomersResponse> result = await _mediator
            .Send(new ListCustomersQuery(page, pageSize, search), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        Result<CustomerDto> result = await _mediator
            .Send(new GetCustomerQuery(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult();
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        CreateCustomerCommand command = new(
            request.DisplayName,
            request.Email,
            request.Phone,
            request.Document,
            request.Notes,
            request.CustomFields);
        Result<CustomerDto> result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        UpdateCustomerCommand command = new(
            id,
            request.DisplayName,
            request.Email,
            request.Phone,
            request.Document,
            request.Notes,
            request.IsActive,
            request.CustomFields);
        Result<CustomerDto> result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        Result result = await _mediator.Send(new DeleteCustomerCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult();
    }
}
