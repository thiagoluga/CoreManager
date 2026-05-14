using System.Reflection;

using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Application.Behaviors;
using Luga.BuildingBlocks.Infrastructure.Observability;
using Luga.BuildingBlocks.Infrastructure.Persistence;
using Luga.BuildingBlocks.Server.Auth;
using Luga.BuildingBlocks.Server.BackgroundJobs;
using Luga.BuildingBlocks.Server.Idempotency;
using Luga.BuildingBlocks.Server.Observability;
using Luga.BuildingBlocks.Server.Tenancy;
using Luga.Modules.Core.Server;

using MediatR;

using Scalar.AspNetCore;

// Luga.Server.Host — API + serves the Blazor WASM bundle.
// CLAUDE.md §5.8.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ---------- Observability (Serilog replaces the default logger early) ----------
builder.Host.UseLugaSerilog();
builder.Services.AddLugaOpenTelemetry(builder.Configuration);
builder.Services.AddLugaHealthChecks();

// ---------- Cross-cutting infrastructure ----------
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddLugaPersistence();

// HTTP-bound implementations of the application abstractions.
builder.Services.AddScoped<ITenantContext, HttpTenantContext>();
builder.Services.AddScoped<ICurrentUser, CurrentUserAccessor>();

// ---------- Auth ----------
builder.Services.AddLugaJwtBearer(builder.Configuration);

// ---------- Background jobs ----------
builder.Services.AddLugaHangfire(builder.Configuration);

// ---------- MediatR (global registration — modules contribute assemblies) ----------
Assembly[] moduleAssemblies =
[
    CoreServerModule.Assembly,
];

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(moduleAssemblies);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
});

// ---------- Modules ----------
builder.Services.AddCoreServerModule(builder.Configuration);

// ---------- MVC + module application parts ----------
builder.Services.AddControllers()
    .AddApplicationPart(CoreServerModule.Assembly);

// ---------- OpenAPI ----------
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// ---------- Request pipeline ----------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

// Serve the Blazor WASM bundle (publishes wwwroot/_framework/* etc.).
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Tenant context middleware enriches the log scope with TenantId.
app.UseMiddleware<TenantContextMiddleware>();

// Idempotency middleware short-circuits retried POST/PUT/PATCH/DELETE.
app.UseMiddleware<IdempotencyMiddleware>();

app.MapControllers();
app.MapLugaHangfireDashboard();
app.MapLugaHealthChecks();

// Anything not handled by API / dashboard / health falls back to the Blazor SPA.
app.MapFallbackToFile("index.html");

await app.RunAsync().ConfigureAwait(false);
