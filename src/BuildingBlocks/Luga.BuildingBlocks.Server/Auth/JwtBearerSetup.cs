using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Luga.BuildingBlocks.Server.Auth;

/// <summary>
/// Wires JWT Bearer authentication for the Luga API. Reads
/// <see cref="EntraExternalIdOptions"/> from <c>EntraExternalId</c> section.
/// </summary>
public static class JwtBearerSetup
{
    /// <summary>Adds JWT Bearer authentication backed by Entra External ID.</summary>
    public static IServiceCollection AddLugaJwtBearer(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<EntraExternalIdOptions>(configuration.GetSection(EntraExternalIdOptions.SectionName));

        EntraExternalIdOptions options = new();
        configuration.GetSection(EntraExternalIdOptions.SectionName).Bind(options);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwt =>
            {
                jwt.Authority = options.Authority;
                jwt.Audience = options.Audience;
                if (!string.IsNullOrWhiteSpace(options.MetadataAddress))
                {
                    jwt.MetadataAddress = options.MetadataAddress;
                }

                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = options.ClockSkew,
                };
            });

        services.AddAuthorization();
        return services;
    }
}
