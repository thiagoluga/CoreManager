using System.Net.Http.Headers;

using Microsoft.Extensions.Logging;

namespace Luga.BuildingBlocks.Infrastructure.Auth;

/// <summary>
/// <see cref="DelegatingHandler"/> that forwards the caller's bearer token on
/// outgoing HTTP requests so downstream services (post-extraction microservices)
/// see the same authenticated principal (CLAUDE.md §3.4 perigo 4).
/// </summary>
/// <remarks>
/// Source of the token is pluggable via <see cref="ITokenProvider"/> — in the
/// monolith this typically wraps <c>IHttpContextAccessor.HttpContext.GetTokenAsync</c>,
/// in background jobs it can hand off a service-credential token.
/// </remarks>
public sealed class AuthPropagationHandler(
    ITokenProvider tokenProvider,
    ILogger<AuthPropagationHandler> logger) : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly ILogger<AuthPropagationHandler> _logger = logger;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Headers.Authorization is null)
        {
            string? token = await _tokenProvider.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogDebug("AuthPropagationHandler: no token available for outbound request to {Uri}", request.RequestUri);
            }
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
