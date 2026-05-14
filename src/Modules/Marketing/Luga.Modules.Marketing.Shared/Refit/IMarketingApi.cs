using Luga.Modules.Marketing.Shared.DTOs;

using Refit;

namespace Luga.Modules.Marketing.Shared.Refit;

/// <summary>
/// Refit contract shared by the Marketing client pages and the API. Read-only
/// endpoints are anonymous; `Contact` requires no auth either (visitors can
/// reach out from the public site).
/// </summary>
public interface IMarketingApi
{
    [Get("/api/marketing/plans")]
    Task<IReadOnlyList<PublicPlanDto>> GetPlansAsync(CancellationToken cancellationToken = default);

    [Post("/api/marketing/contact")]
    Task SubmitContactAsync([Body] ContactRequestDto request, CancellationToken cancellationToken = default);
}
