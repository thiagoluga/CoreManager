using System.Globalization;

using Luga.BuildingBlocks.Client;
using Luga.Client.Host;
using Luga.Modules.Core.Client;
using Luga.Modules.Customers.Client;
using Luga.Modules.Marketing.Client;
using Luga.Modules.Payments.Client;
using Luga.Modules.Personalization.Client;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

// Luga.Client.Host — Blazor WASM bootstrap (CLAUDE.md §5.9).
WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ---------- Core HttpClient (same origin as the API host) ----------
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

// ---------- MSAL authentication against Entra External ID ----------
// Configuration lives in wwwroot/appsettings.json under the "EntraExternalId" section.
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("EntraExternalId", options.ProviderOptions.Authentication);
    options.ProviderOptions.LoginMode = "redirect";
});

// ---------- BuildingBlocks.Client (MudBlazor, i18n, tenant/user/permission state) ----------
builder.Services.AddLugaBuildingBlocksClient(localization =>
{
    localization.SupportedCultures.Clear();
    localization.SupportedCultures.Add(new CultureInfo("pt-BR"));
    localization.SupportedCultures.Add(new CultureInfo("en-US"));
    localization.SupportedCultures.Add(new CultureInfo("es-ES"));
    localization.FallbackCulture = new CultureInfo("pt-BR");
});

// ---------- Modules ----------
builder.Services.AddCoreClientModule();
builder.Services.AddMarketingClientModule();
builder.Services.AddPersonalizationClientModule();
builder.Services.AddCustomersClientModule();
builder.Services.AddPaymentsClientModule();

// ---------- Default UI culture ----------
// User > tenant > browser cascading happens in the ILugaCultureProvider after login;
// here we just lock the thread cultures to a sensible default before the first render.
CultureInfo defaultCulture = new("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

await builder.Build().RunAsync().ConfigureAwait(false);
