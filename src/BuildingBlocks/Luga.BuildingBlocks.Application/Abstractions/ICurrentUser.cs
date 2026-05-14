namespace Luga.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Ambient context for the authenticated user issuing the current request.
/// Populated from the JWT by <c>CurrentUserAccessor</c>.
/// </summary>
public interface ICurrentUser
{
    /// <summary>True when a user has been authenticated for the current scope.</summary>
    bool IsAuthenticated { get; }

    /// <summary>User id from the JWT subject claim.</summary>
    Guid UserId { get; }

    /// <summary>Username (snapshot stored in audit fields when this user mutates entities).</summary>
    string Username { get; }

    /// <summary>Preferred UI culture (falls back to the tenant default then the configured fallback).</summary>
    string PreferredCulture { get; }

    /// <summary>Permissions granted to the user for the current tenant scope.</summary>
    IReadOnlySet<string> Permissions { get; }

    /// <summary>True when the user has the given permission code.</summary>
    bool HasPermission(string permission);
}
