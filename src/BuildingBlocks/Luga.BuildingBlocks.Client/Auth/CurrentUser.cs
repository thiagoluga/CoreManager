namespace Luga.BuildingBlocks.Client.Auth;

/// <summary>
/// Client-side snapshot of the authenticated user. Populated by the host after login
/// (typically from <c>GET /api/users/me</c>).
/// </summary>
public sealed class CurrentUser
{
    /// <summary>Sentinel returned for anonymous / pre-auth contexts.</summary>
    public static CurrentUser Anonymous { get; } = new();

    /// <summary>User id from the JWT subject claim.</summary>
    public Guid UserId { get; init; }

    /// <summary>Login (e-mail or handle), used as the audit username snapshot.</summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>Human-friendly name shown in the app bar.</summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>User-preferred UI culture (e.g. <c>pt-BR</c>).</summary>
    public string PreferredCulture { get; init; } = "pt-BR";

    /// <summary>Whether the user has finished the authentication flow.</summary>
    public bool IsAuthenticated { get; init; }
}
