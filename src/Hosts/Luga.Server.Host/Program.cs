using System.Reflection;

using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Application.Behaviors;
using Luga.BuildingBlocks.Infrastructure.Observability;
using Luga.BuildingBlocks.Infrastructure.Persistence;
using Luga.BuildingBlocks.Infrastructure.Persistence.Migrations;
using Luga.BuildingBlocks.Server.Auth;
using Luga.BuildingBlocks.Server.BackgroundJobs;
using Luga.BuildingBlocks.Server.Idempotency;
using Luga.BuildingBlocks.Server.Modules;
using Luga.BuildingBlocks.Server.Observability;
using Luga.BuildingBlocks.Server.RateLimiting;
using Luga.BuildingBlocks.Server.Tenancy;
using Luga.Modules.Core.Server;
using Luga.Modules.Core.Server.Infrastructure.Persistence;
using Luga.Modules.Customers.Server;
using Luga.Modules.Customers.Server.Infrastructure.Persistence;
using Luga.Modules.Marketing.Server;
using Luga.Modules.Marketing.Server.Infrastructure.Persistence;
using Luga.Modules.Payments.Server;
using Luga.Modules.Payments.Server.Infrastructure.Persistence;
using Luga.Modules.Personalization.Server;
using Luga.Modules.Personalization.Server.Infrastructure.Persistence;

using MediatR;

using Scalar.AspNetCore;

// Luga.Server.Host — API + serves the Blazor WASM bundle.
// CLAUDE.md §5.8.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ---------- Database bootstrap (CLAUDE.md §7.9) ----------
// Ensures the SQL Server database exists before Hangfire's SqlServerStorage tries
// to open a connection during DI build. Gated by the same flag as EF migrations:
// default true in Development, opt-in elsewhere. Production provisions the DB
// via IaC and disables this.
bool shouldBootstrapDatabase = builder.Configuration.GetValue<bool>(
    "ApplyMigrationsOnStartup",
    defaultValue: builder.Environment.IsDevelopment());

if (shouldBootstrapDatabase)
{
    string? defaultConnection = builder.Configuration.GetConnectionString("Default");
    if (!string.IsNullOrWhiteSpace(defaultConnection))
    {
        Console.WriteLine($"[bootstrap] Ensuring database exists for connection: {defaultConnection}");
        await DatabaseBootstrapper
            .EnsureDatabaseExistsAsync(defaultConnection)
            .ConfigureAwait(false);
        Console.WriteLine("[bootstrap] Database bootstrap complete.");
    }
    else
    {
        Console.WriteLine("[bootstrap] ConnectionStrings:Default missing — skipping bootstrap.");
    }
}

// ---------- Observability (Serilog replaces the default logger early) ----------
builder.Host.UseLugaSerilog();
builder.Services.AddLugaOpenTelemetry(builder.Configuration);
builder.Services.AddLugaHealthChecks();
builder.Services.AddLugaRateLimiting();

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
    MarketingServerModule.Assembly,
    PersonalizationServerModule.Assembly,
    CustomersServerModule.Assembly,
    PaymentsServerModule.Assembly,
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
builder.Services.AddMarketingServerModule(builder.Configuration);
builder.Services.AddPersonalizationServerModule(builder.Configuration);
builder.Services.AddCustomersServerModule(builder.Configuration);
builder.Services.AddPaymentsServerModule(builder.Configuration);

// ---------- Startup-time runners (migrations + seed data) ----------
builder.Services.AddSingleton<ModuleInitializerRunner>();

// ---------- MVC + module application parts ----------
builder.Services.AddControllers()
    .AddApplicationPart(CoreServerModule.Assembly)
    .AddApplicationPart(MarketingServerModule.Assembly)
    .AddApplicationPart(PersonalizationServerModule.Assembly)
    .AddApplicationPart(CustomersServerModule.Assembly)
    .AddApplicationPart(PaymentsServerModule.Assembly);

// ---------- OpenAPI ----------
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// ---------- Auto-apply migrations + seed (CLAUDE.md §7.9) ----------
// Default: enabled in Development, disabled elsewhere. Production uses the
// dedicated `deploy-migrations.yml` workflow before the app rolls. Reuses the
// same flag evaluated above for the database bootstrap so both steps stay in
// sync.
if (shouldBootstrapDatabase)
{
    await using AsyncServiceScope startupScope = app.Services.CreateAsyncScope();
    ILogger<Program> startupLogger = startupScope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    Type[] moduleDbContextTypes =
    [
        typeof(CoreDbContext),
        typeof(CustomersDbContext),
        typeof(PaymentsDbContext),
        typeof(MarketingDbContext),
        typeof(PersonalizationDbContext),
    ];

    try
    {
        startupLogger.LogInformation("ApplyMigrationsOnStartup=true — running module migrations.");
        ModuleMigrationRunner migrationRunner = startupScope.ServiceProvider.GetRequiredService<ModuleMigrationRunner>();
        await migrationRunner.RunAsync(moduleDbContextTypes).ConfigureAwait(false);

        startupLogger.LogInformation("Running module initializers (versioned DML seeds).");
        ModuleInitializerRunner initializerRunner = startupScope.ServiceProvider.GetRequiredService<ModuleInitializerRunner>();
        await initializerRunner.RunAsync(typeof(CoreDbContext)).ConfigureAwait(false);

        startupLogger.LogInformation("Startup database routines completed.");
    }
    catch (Exception ex)
    {
        startupLogger.LogCritical(ex, "Startup migration / initialization failed. The host will exit.");
        throw;
    }
}

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

app.UseRateLimiter();
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
