using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Marketing.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Marketing.Server.Application;

/// <summary>
/// Returns the published plans for the public pricing page. Pulls from Core's
/// contract service so the catalog stays in one place.
/// </summary>
public sealed record GetPublicPlansQuery : IRequest<Result<IReadOnlyList<PublicPlanDto>>>;
