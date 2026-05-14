using Luga.BuildingBlocks.Domain.Common;

using MediatR;

namespace Luga.Modules.Marketing.Server.Application;

/// <summary>
/// Public-site contact form submission. Stored / dispatched downstream (V2 wires
/// the Mailtrap delivery + audit table); the MVP just acknowledges receipt.
/// </summary>
public sealed record SubmitContactCommand(
    string Name,
    string Email,
    string? Company,
    string Message) : IRequest<Result>;
