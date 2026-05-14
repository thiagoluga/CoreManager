namespace Luga.BuildingBlocks.Infrastructure.Auth;

/// <summary>
/// Resolves the bearer token to attach to outbound HTTP calls. Implementations
/// live in <c>BuildingBlocks.Server</c> (HTTP-aware) or in the background job
/// host (service credentials).
/// </summary>
public interface ITokenProvider
{
    /// <summary>Returns the access token to attach, or null when none is available.</summary>
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
