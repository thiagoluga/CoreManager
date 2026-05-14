# PLAN.md — Luga CoreManager

> Roadmap executável. Fases, checklists, decisões registradas, riscos.
> Atualizado conforme entregas avançam. Itens pequenos o suficiente
> para virar PR isolado.

---

## 1. Visão Geral

Construir o **Luga CoreManager** — plataforma SaaS B2B brasileira multi-produto para SMBs — em fases incrementais, começando por fundação sólida e terminando o MVP com 5 módulos funcionais (Marketing + Core + Customers + Payments + Personalization).

**Princípio guia**: cada fase tem entregável testável, deployável e demonstrável. Mesmo a Fase 0 termina com algo em staging.

---

## 2. Decisões Tomadas (ADRs Resumidos)

| # | Decisão | Justificativa | Status |
|---|---|---|---|
| 001 | Modular Monolith Extractable | Suite com simplicidade do monolito + opção microsserviços | ✅ |
| 002 | Suite Integrada (não Federation) | ICP SMB exige UX coesa; entidades core compartilhadas | ✅ |
| 003 | .NET 10 + ASP.NET Core 10 + EF Core 10 | LTS mais recente, performance, ferramental Microsoft | ✅ |
| 004 | Azure SQL Database Serverless | Stack Microsoft, auto-pause economiza | ✅ |
| 005 | Microsoft Entra External ID (single tenant + claim) | Stack Microsoft, escala SMBs, custo baixo | ✅ |
| 006 | Azure Container Apps + GHCR público | Scale-to-zero, custo baixo, alinhado extração futura | ✅ |
| 007 | GitHub Actions com OIDC | Sem secrets, padrão moderno | ✅ |
| 008 | Bicep para IaC | Nativo Azure, simples, sem state | ✅ |
| 009 | Controllers (não Minimal APIs) | Decisão do dev — mais convencional | ✅ |
| 010 | MediatR (não Mediator source-gen) | Comunidade enorme, melhor para Visual Studio | ✅ |
| 011 | Mapperly (não AutoMapper) | Source-gen, sem reflection, explícito | ✅ |
| 012 | Repository Pattern com base genérica | Decisão do dev — wrapper sobre EF | ✅ |
| 013 | EF Core Interceptors (não override) | Composable, testável, padrão moderno | ✅ |
| 014 | LugaDbContextBase em BuildingBlocks.Infrastructure | Reuso sem violar arquitetura cross-module | ✅ |
| 015 | Schema por módulo no SQL Server | Isolamento físico, prepara extração | ✅ |
| 016 | Migration history table por schema | Cada módulo independente | ✅ |
| 017 | Outbox por módulo + handlers idempotentes | Garantia de delivery, sem perda em crashes/deploys | ✅ |
| 018 | Result<T> próprio | Sem exception para fluxo de negócio | ✅ |
| 019 | TimeProvider para tempo testável | Padrão .NET 8+ | ✅ |
| 020 | Customer no módulo Customers (não core) | Entidade rica, evolui muito, isola para extração | ✅ |
| 021 | Visual Studio 2022 17.12+ | Decisão do dev | ✅ |
| 022 | Mailtrap para email transacional | Free tier generoso | ✅ |
| 023 | WhatsApp Manual via wa.me no MVP | Gratuito, sem onboarding | ✅ |
| 024 | Asaas como gateway primário (V2 cobertura) | Foco BR, Pix imediato, taxas competitivas | ✅ |
| 025 | Frontend: Blazor WebAssembly + MudBlazor + Tailwind v4 | Stack único .NET, dev produtivo dia 1 | ✅ |
| 026 | 3 projetos por módulo (Server + Client + Shared) + Contracts | Modular full-stack inspirado em Oqtane | ✅ |
| 027 | Single Blazor WASM app com áreas (marketing/dashboard/admin) | UX integrada, simples para MVP | ✅ |
| 028 | Mobile via MAUI Blazor Hybrid em V1.1+ | ~95% reuso de código com web | ✅ |
| 029 | PWA habilitado desde Fase 0 | Mobile básico sem custo adicional | ✅ |
| 030 | Custom fields como JSON column | Performático no SQL Server, queries via JSON_VALUE | ✅ |
| 031 | Convenção de nomes: Luga.Modules.X.{Server\|Client\|Shared\|Contracts} | Plural, padrão indústria | ✅ |
| 032 | Sistema de páginas: D++ (manifest + gestão de menu, sem CMS) | Equilíbrio funcionalidade/complexidade | ✅ |
| 033 | IModuleManifest declara menu/widgets/breadcrumbs | Discovery via DI, host não conhece módulos | ✅ |
| 034 | IModuleInitializer para seeds versionados | Separado de migrations (DDL vs DML) | ✅ |
| 035 | Breadcrumb no MVP: apenas código; UI/URL overrides em V2+ | Reduz escopo MVP | ✅ |
| 036 | i18n Full-ready desde MVP (Opção 1) com Abordagem C | Refactor depois é doloroso | ✅ |
| 037 | i18n: pt-BR + en-US + es-ES preparados; pt-BR ativo no MVP | Lança em BR, expansão futura | ✅ |
| 038 | i18n fallback configurado via UI no super admin | Flexibilidade operacional | ✅ |
| 039 | Hangfire OSS (não Pro) | Free, suficiente para MVP | ✅ |
| 040 | .NET Aspire 13 para dev local | Dashboard nativo, melhor que docker-compose para .NET | ✅ |
| 041 | Mocking: Moq (não NSubstitute) | Padrão histórico .NET, comunidade maior | ✅ |
| 042 | Marketing como módulo Blazor WASM no MVP | Mais simples; revisita se SEO virar canal crítico | ✅ |
| 043 | Módulo Personalization para overrides de UI | Centraliza customizações | ✅ |
| 044 | Integration Events versionados (V1, V2) explicitamente | Suporta evolução pós-extração | ✅ |
| 045 | APIs cross-module SEMPRE têm batch desde dia 1 | Evita N+1 quando virar HTTP | ✅ |
| 046 | Migrations backwards-compatible obrigatórias | Zero downtime deploys | ✅ |

---

## 3. TBDs Explícitos

| Tópico | Quando decidir |
|---|---|
| **Domínio definitivo** (luga.com indisponível) | Antes de comprar Azure resources com custom domain |
| Pricing exato dos planos (R$ Starter / Pro / Business) | Próximo do lançamento, baseado em validação |
| Gateway escolhido para cobrar próprios tenants | Início da Fase 1 (Stripe / Asaas / Mercado Pago) |
| Ferramenta de feature flags | V2, quando justificar |
| Política de backup e DR | Antes de produção (Fase 1 final) |
| LGPD compliance detalhado (DPO, fluxo de exclusão) | Antes do lançamento público |
| Suporte ao cliente (Intercom, Crisp, custom) | Antes do lançamento público |
| Analytics de produto (PostHog, Mixpanel) | V1.1+ |
| Status page (Statuspage.io, Instatus) | V1.1+ |
| Termos de uso e política de privacidade | Antes do lançamento público |
| WhatsApp automatizado (Z-API vs Cloud API) | V2 |
| Marketing como Blazor Server separado (vs módulo WASM) | Se SEO virar canal crítico em V1.1+ |

---

## 4. Fora de Escopo do MVP

Para evitar scope creep, EXPLICITAMENTE não entram no MVP:

- ❌ Módulo Documents (DMS) — V2
- ❌ Módulo Signatures — V2
- ❌ Módulo CRM dedicado — V3
- ❌ Módulo NF (NFS-e) — V3
- ❌ Portal do customer final — V4
- ❌ Mensageria unificada (Talk) — V4
- ❌ Agendamento — V5
- ❌ Financeiro/DRE — V5
- ❌ App mobile nativo — V1.1+
- ❌ Actions engine (motor de automação genérico) — V2
- ❌ WhatsApp automatizado via API — V2
- ❌ Stripe Connect (cobrança no exterior) — V2
- ❌ Multi-gateway no Payments (só Manual + Asaas no MVP) — V2 adiciona Pagar.me/MP
- ❌ White-label completo (cores/marca por tenant) — V3
- ❌ Webhooks públicos do Luga para sistemas externos — V2 (com Actions)
- ❌ API pública para devs externos — V3
- ❌ Marketplace de extensões — V3+
- ❌ Page composition completa (CMS-style com custom router) — V2+ se demanda
- ❌ Breadcrumb overrides via UI e URL — V2+ se demanda
- ❌ Templates de notificação multi-idioma — V3+
- ❌ Custom fields multi-idioma — V3+
- ❌ Traduções en-US e es-ES populadas — V2+ conforme demanda (arquitetura preparada)

---

## 5. FASE 0 — Fundação

**Objetivo**: criar fundação técnica completa que suporte todos os módulos futuros, com infraestrutura deployada em staging. Sem features de negócio ainda — apenas plumbing.

**Critério de pronto**: API base sobe em Container App de staging, autentica via Entra External ID, salva em Azure SQL, frontend Blazor WASM roda em localhost mostrando login funcional, Aspire orquestra dev local.

**Estimativa**: 4-6 semanas (dev sozinho) / 2-3 semanas (com pair).

### 5.1 Setup do Repositório

- [x] (S) Criar repo no GitHub (privado inicialmente)
- [x] (S) `.gitignore` para .NET + Visual Studio + Rider
- [x] (S) `.editorconfig` configurado para Visual Studio + StyleCop
- [x] (S) Initial `README.md`
- [x] (S) Copiar `CLAUDE.md` e `PLAN.md` para raiz do repo
- [ ] (S) Branch protection rules em `main` (require PR, require CI) — pendente (ação no GitHub)

### 5.2 Solution e Projetos Base

- [x] (S) Criar `src/Luga.CoreManager.slnx` (formato `.slnx` — .NET 10 default; movido para `src/`)
- [x] (S) `Directory.Build.props` (Nullable, ImplicitUsings, LangVersion, TreatWarningsAsErrors)
- [x] (S) `Directory.Build.targets` (overrides aplicados após csproj — IsTestProject)
- [x] (S) `Directory.Packages.props` (Central Package Management)
- [x] (S) `global.json` (SDK pinado em 10.0.203)
- [x] (M) Criar projetos BuildingBlocks:
  - [x] `Luga.BuildingBlocks.Domain`
  - [x] `Luga.BuildingBlocks.Application`
  - [x] `Luga.BuildingBlocks.Infrastructure`
  - [x] `Luga.BuildingBlocks.IntegrationEvents`
  - [x] `Luga.BuildingBlocks.Server`
  - [x] `Luga.BuildingBlocks.Client`
- [x] (M) Criar projetos Hosts:
  - [x] `Luga.Server.Host` (API)
  - [x] `Luga.Client.Host` (Blazor WASM)
- [x] (S) Criar `Luga.AppHost` (Aspire 13.3.0)
- [x] (S) Criar projetos de testes (Architecture, BuildingBlocks)
- [x] (S) Configurar references entre projetos conforme regras

### 5.3 BuildingBlocks.Domain

- [x] (S) `IDomainEvent.cs`
- [x] (S) `IIntegrationEvent.cs` (em IntegrationEvents)
- [x] (M) Interfaces marker:
  - [x] `IAuditable.cs`
  - [x] `ISoftDeletable.cs`
  - [x] `IMultiTenant.cs`
  - [x] `IConcurrencyAware.cs`
  - [x] `IActivatable.cs`
  - [x] `IHasDomainEvents.cs`
- [x] (M) Hierarquia de Entity:
  - [x] `EntityBase.cs`
  - [x] `AuditableEntity.cs`
  - [x] `FullAuditableEntity.cs`
  - [x] `TenantEntity.cs`
- [x] (M) Result<T> pattern + Error + GeneralErrors
- [x] (S) Unit tests para Result e GeneralErrors (26 testes, todos passando)

### 5.4 BuildingBlocks.Application

- [x] (S) `ITenantContext.cs`
- [x] (S) `ICurrentUser.cs` (UserId + Username + PreferredCulture + Permissions)
- [x] (S) `IUnitOfWork.cs`
- [x] (S) `IIdempotencyStore.cs` (abstraction; impl em 5.5)
- [x] (M) `IRepository<T>.cs` base interface (com `ISpecification` + `PagedList`)
- [x] (S) `PagedList<T>.cs` + `PagedRequest.cs`
- [x] (M) MediatR pipeline behaviors:
  - [x] `LoggingBehavior`
  - [x] `ValidationBehavior` (retorna `Result.Failure` quando `TResponse` é `Result`; senão lança `ValidationException`)
  - [x] `IdempotencyBehavior` + `IIdempotentRequest` marker
  - [x] `PerformanceBehavior` (threshold 500ms)
- [x] (S) `ResultExtensions.cs` (`ToActionResult` → RFC 7807 ProblemDetails) — **movido para `BuildingBlocks.Server.Http`** porque Application não pode referenciar ASP.NET Core (CLAUDE.md §7.2)
- [x] (S) Unit tests (33 novos: PagedRequest, PagedList, ValidationBehavior, ResultExtensions; 59 totais passando)

### 5.5 BuildingBlocks.Infrastructure

- [x] (M) `LugaDbContextBase.cs` (global query filters for tenancy + soft-delete, rowversion concurrency tokens, `IUnitOfWork`)
- [x] (L) Interceptors (registered scoped in `AddLugaPersistence`):
  - [x] `AuditableEntityInterceptor`
  - [x] `TenantIdInterceptor`
  - [x] `SoftDeleteInterceptor`
  - [x] `ActivationTrackingInterceptor`
  - [x] `DomainEventToOutboxInterceptor`
- [x] (M) Outbox pattern:
  - [x] `OutboxMessage.cs` + `OutboxMessageConfiguration.cs`
  - [x] `ProcessedIntegrationEvent.cs` + `ProcessedIntegrationEventConfiguration.cs`
  - [x] `IOutboxProcessor.cs`
  - [x] `OutboxProcessor<TContext>.cs` (generic; each module registers its own)
- [x] (M) Repository base:
  - [x] `Repository<T>.cs` (Ardalis.Specification + paging)
  - [x] (skipped) `SpecificationEvaluator.cs` — using `Ardalis.Specification.EntityFrameworkCore.SpecificationEvaluator.Default` directly
- [x] (M) Migrations infrastructure:
  - [x] `ModuleMigrationRunner.cs` (Infrastructure)
  - [x] `ModuleInitializerRunner.cs` (**moved to Server** — depends on `IModuleInitializer`)
  - [x] `IModuleInitializer.cs` (Server)
  - [x] `InitializationContext.cs` (Server)
  - [x] `ModuleInitialization.cs` entity + config (`core.module_initializations`)
- [x] (M) Tenancy:
  - [x] `HttpTenantContext.cs` (Server — implements `ITenantContext` from claims)
  - [x] `TenantContextMiddleware.cs` (Server — adds TenantId to log scope)
  - [x] `TenantClaimsExtractor.cs` (Server — claim constants and helpers)
- [x] (M) Auth:
  - [x] `EntraExternalIdOptions.cs` (Server — config record)
  - [x] `JwtBearerSetup.cs` (Server)
  - [x] `CurrentUserAccessor.cs` (Server — implements `ICurrentUser` from claims)
  - [x] `AuthPropagationHandler.cs` + `ITokenProvider.cs` (Infrastructure — `DelegatingHandler`)
- [x] (M) Idempotency:
  - [x] `IdempotencyKey.cs` entity + config (Infrastructure)
  - [x] `IdempotencyStore.cs` (Infrastructure — implements `IIdempotencyStore`)
  - [x] `IdempotencyMiddleware.cs` (Server — POST/PUT/PATCH/DELETE + buffered response capture)
- [x] (S) `HangfireSetup.cs` (Server — SQL Server storage + dashboard auth policy)
- [x] (M) Observability:
  - [x] `SerilogSetup.cs` (Infrastructure)
  - [x] `OpenTelemetrySetup.cs` (Infrastructure — AspNetCore + Http + Runtime instrumentation)
  - [x] `HealthChecksSetup.cs` (Server — `/health/live`, `/health/ready`)
- [x] (M) Events:
  - [x] `InProcessIntegrationEventBus.cs` (Infrastructure)
  - [x] `IProcessedEventStore.cs` (IntegrationEvents) + `ProcessedEventStore.cs` (Infrastructure)
  - [x] `DomainEventDispatcher.cs` (Infrastructure — skips `IIntegrationEvent` already outboxed)
  - [x] `IIntegrationEventHandler<T>.cs` (IntegrationEvents)
- [x] (S) `PersistenceServiceCollectionExtensions.cs` (registers interceptors + cross-cutting infra)

### 5.6 BuildingBlocks.Client (Blazor)

- [x] (M) `IModuleManifest.cs`
- [x] (M) `MenuItem.cs`, `DashboardWidget.cs` (+ `DashboardWidgetSize` enum), `BreadcrumbRoute.cs`, `BreadcrumbSegment.cs` (+ `BreadcrumbSegmentSource` enum)
- [x] (M) `EmbeddableComponent.cs` (reserved for V2+)
- [x] (M) `LugaPageBase.cs` (cascading parameters: `TenantContext`, `CurrentUser`, `IPermissionService`)
- [x] (M) Tenancy + Auth client-side context: `TenantContext.cs` (with `ActiveModules`/`HasModuleActive`), `CurrentUser.cs`
- [x] (M) Shared components:
  - [x] `MainMenu.razor` (filters by `RequiredSubscriptionModule`)
  - [x] `MenuSection.razor` (filters by `RequiredPermission`, localizes labels)
  - [x] `PageBreadcrumb.razor` (reactive to `NavigationManager.LocationChanged`)
  - [x] `Breadcrumb.razor` (wraps `MudBreadcrumbs`)
  - [x] `PermissionGate.razor` (with `FallbackContent`)
  - [x] `NotFoundPage.razor`
- [x] (M) Layouts:
  - [x] `MainLayout.razor` (MudLayout + MudAppBar + MudDrawer + MainMenu)
  - [x] `AuthLayout.razor` (centered card for login/signup)
- [x] (M) Services:
  - [x] `IBreadcrumbResolver.cs` + `BreadcrumbResolver.cs` (route matching with `{...}` parameter wildcards + dynamic-leaf substitution)
  - [x] `IPermissionService.cs` + `PermissionService.cs`
- [x] (M) i18n setup:
  - [x] `My.Extensions.Localization.Json` wired via `AddLugaLocalization`
  - [x] `Resources/SharedStrings.pt-BR.json` + en-US + es-ES (placeholders pre-translated)
  - [x] `Resources/NotFoundPage.{pt-BR,en-US,es-ES}.json`
  - [x] `IStringLocalizerFactory` resolves via `My.Extensions.Localization.Json`
  - [x] `ILugaCultureProvider` cascade (user > tenant > browser > fallback)
  - [x] JSON files marked as `EmbeddedResource` so they reach the WASM runtime
- [x] (S) `BuildingBlocksClientServiceCollectionExtensions.AddLugaBuildingBlocksClient` (registers MudBlazor + localization + context records + permission/breadcrumb services)
- [x] (S) Unit tests: BreadcrumbResolver (5), PermissionService (7) → 12 new, **71 total passing**

### 5.7 Módulo Core (mínimo para autenticar)

- [x] (M) 4 Core projects created (Server, Client, Shared, Contracts) and added to slnx
- [x] (M) Domain (Server):
  - [x] `Tenant.cs` (`FullAuditableEntity`, NOT `IMultiTenant`) with `Register` factory that raises `TenantRegisteredDomainEvent` + `TenantCreatedIntegrationEventV1`
  - [x] `TenantUser.cs` (`TenantEntity` with `PreferredCulture`, `Role`, factory + lifecycle methods)
  - [x] `TenantStatus.cs` enum (`Active` / `Suspended` / `Cancelled`)
  - [x] `TenantUserRole.cs` enum (`Owner` / `Admin` / `Manager` / `Operator` / `Viewer`)
  - [x] Domain events (`TenantRegisteredDomainEvent`)
  - [x] Errors (`CoreErrors`)
- [x] (M) Contracts:
  - [x] `ITenantsService.cs` + `IUsersService.cs` (with batch `GetByIdsAsync` from day 1)
  - [x] DTOs: `TenantContractDto`, `TenantUserContractDto`
  - [x] Integration events V1: `TenantCreatedIntegrationEventV1`, `TenantUserRegisteredIntegrationEventV1`
- [x] (M) Shared:
  - [x] HTTP DTOs: `TenantDto`, `TenantUserDto`, `RegisterTenantRequest`/`Response`, `EnrichClaimsRequest`/`Response`
  - [x] `ICoreApi.cs` (Refit) — `RegisterTenant`, `GetCurrentTenant`, `GetMyProfile`
  - [x] Validators: `RegisterTenantRequestValidator`
- [x] (M) Application — minimal feature slice:
  - [x] `RegisterTenantCommand` + handler + validator
  - [x] `GetCurrentTenantQuery` + handler
  - [x] `GetMyProfileQuery` + handler
  - [x] Mappers (Mapperly with `RequiredMappingStrategy.Target`)
  - [x] Repositories (`ITenantRepository`, `ITenantUserRepository` with batch + cross-tenant lookup for the claims provider)
- [x] (M) Infrastructure:
  - [x] `CoreDbContext` (extends `LugaDbContextBase`, default schema `core`, owns shared `core.idempotency_keys` and `core.module_initializations`)
  - [x] EF Core configurations (unique tenant slug, unique `(TenantId, Username)` on tenant_users)
  - [x] Repository implementations
  - [x] Contract service implementations (`TenantsService`, `UsersService`)
  - [ ] **Initial migration deferred** — run `dotnet ef migrations add Initial --project src/Modules/Core/Luga.Modules.Core.Server --startup-project src/Hosts/Luga.Server.Host --context CoreDbContext --output-dir Infrastructure/Persistence/Migrations` once §5.8 wires the host. CLAUDE.md §21 requires the generated migration to be reviewed before applying.
  - [x] `CoreServerModule.cs` (composition root: DbContext + MediatR pipeline + validators + repositories + contract services + initializer)
  - [x] `CoreModuleInitializer.cs` (v1, empty hook — seeds plan-catalog data when §6.2 lands)
- [x] (M) Api:
  - [x] `TenantsController` (`POST /register` anonymous, `GET /current` authorized)
  - [x] `UsersController` (`GET /me` authorized)
  - [x] `AuthController.EnrichClaims` (anonymous; stub returns user's tenant id + preferred culture)
- [x] (M) Client:
  - [x] `CoreManifest.cs` (menu: profile + settings; breadcrumbs declared)
  - [x] `CoreClientModule.cs` (registers manifest + Refit `ICoreApi`)
  - [x] Pages: `Profile.razor`, `Settings.razor` (read-only stubs hitting the API)
  - [x] Resources JSON: `CoreManifest / Profile / Settings` × `{pt-BR, en-US, es-ES}` (pt-BR primary)

> **Architectural change in this section:** `IIntegrationEvent` now derives from `IDomainEvent`, allowing integration events to be raised on the entity's `DomainEvents` collection. The `DomainEventToOutboxInterceptor` picks them up at `SaveChanges` time; the `DomainEventDispatcher` skips them so they're not re-dispatched in-process.

### 5.8 Bootstrapper (Luga.Server.Host)

- [x] (M) `Program.cs` wired with:
  - [x] BuildingBlocks setup (`AddLugaPersistence`)
  - [x] Entra External ID auth (`AddLugaJwtBearer`)
  - [x] Tenancy: `IHttpContextAccessor` + `HttpTenantContext` + `TenantContextMiddleware`
  - [x] Auth resolver: `CurrentUserAccessor`
  - [x] Observability: Serilog (`UseLugaSerilog`) + OpenTelemetry (`AddLugaOpenTelemetry`)
  - [x] MediatR with module assemblies + 4 pipeline behaviors (Logging / Validation / Idempotency / Performance) registered **globally** (removed duplicate registration from `CoreServerModule`)
  - [x] `AddCoreServerModule` + `AddApplicationPart(CoreServerModule.Assembly)` for MVC discovery
  - [x] Hangfire dashboard (`AddLugaHangfire` + `MapLugaHangfireDashboard` at `/jobs`)
  - [x] Health checks (`AddLugaHealthChecks` + `MapLugaHealthChecks` at `/health/live` and `/health/ready`)
  - [x] OpenAPI (`AddOpenApi` + `MapScalarApiReference` in Development)
  - [x] Idempotency middleware
  - [x] Serve Blazor WASM (`UseBlazorFrameworkFiles` + `UseStaticFiles` + `MapFallbackToFile("index.html")`)
- [x] (S) `appsettings.json` + `appsettings.Development.json` (ConnectionStrings, EntraExternalId, OpenTelemetry, Serilog overrides)
- [x] (S) `Dockerfile` (multi-stage build from repo root context; publishes API + Blazor bundle on `aspnet:10.0`)
- [x] (S) `.dockerignore`

> **Note:** No initial EF migration generated yet — the host compiles and starts cleanly without it, but `dotnet ef migrations add Initial …` needs to run before the app actually hits SQL Server. CLAUDE.md §21 requires the migration to be reviewed before applying.

### 5.9 Luga.Client.Host (Blazor WASM bootstrap)

- [x] (M) `Program.cs` wired with:
  - [x] MudBlazor (via `AddLugaBuildingBlocksClient` → `AddMudServices`)
  - [ ] Tailwind v4 — **deferred**: `app.css` carries a placeholder note; npm-based Tailwind build pipeline is V1.1 scope
  - [x] MSAL authentication (`AddMsalAuthentication` reading `EntraExternalId` section)
  - [x] HttpClient (same-origin to host) — Refit clients registered by each `X.ClientModule`
  - [x] Localization (`AddLugaLocalization` with pt-BR/en-US/es-ES, pt-BR fallback)
  - [x] Default thread culture locked before first render
  - [x] PWA registration (service-worker registered in `index.html` behind a feature check)
- [x] (M) `App.razor` with `<CascadingAuthenticationState>` + `<Router>` + `AdditionalAssemblies` (explicit list, starts with `CoreClientModule.Assembly`) + `<AuthorizeRouteView>` + `<NotAuthorized>` → `RedirectToLogin` + `NotFoundPage` from BuildingBlocks
- [x] (M) `wwwroot/index.html` (Roboto + MudBlazor CSS/JS, MSAL `AuthenticationService.js`, `theme-color`, mobile viewport, pt-BR lang, dropped Bootstrap)
- [x] (M) PWA: `manifest.webmanifest` (Luga-branded, `purpose: any maskable`, scope, description, lang) + existing `service-worker.js` / `service-worker.published.js` / icons retained
- [x] (S) Auth pages in `Client.Host/Pages/`:
  - [x] `Authentication.razor` (`/authentication/{action}` → `RemoteAuthenticatorView`) under `AuthLayout`
  - [x] `Login.razor` (`/login` → `NavigateToLogin` with returnUrl, under `AuthLayout`, i18n via `Resources/Login.{pt-BR,en-US,es-ES}.json`)
  - [x] `Shared/RedirectToLogin.razor` (preserves returnUrl)
- [x] (S) `wwwroot/appsettings.json` with `EntraExternalId` MSAL config + `Api.BaseUrl`
- [x] (S) Removed Bootstrap/template `Layout/` + `Pages/` from Client.Host — layouts owned by `BuildingBlocks.Client.Layouts`

> **Note:** MSAL `ClientId` and the actual Entra External ID tenant URLs in `appsettings.json` are placeholders. Real values come from Azure setup in §5.12.

### 5.10 Aspire (Luga.AppHost)

- [x] (M) `Luga.AppHost` Aspire 13.3.0 project (created in §5.2)
- [x] (M) Compose: SQL Server container with persistent `luga-sql-data` volume → `luga-core` DB → `api` project → `web` project; `WaitFor` chained so the API only starts after the DB is healthy
- [x] (M) Connection string republished under `ConnectionStrings__Default` and `ConnectionStrings__Hangfire` so the API code reads the keys it already expects with no Aspire-specific branching
- [x] (S) README documents `dotnet run --project src/AppHost/Luga.AppHost` and the dashboard URL <http://localhost:18888>

### 5.11 Testes

- [x] (M) `Luga.Tests.Architecture` (ArchUnitNET, 11 tests):
  - [x] Domain has no Microsoft.EntityFrameworkCore dependency
  - [x] Domain has no MediatR dependency
  - [x] Application does not depend on Infrastructure
  - [x] Application has no EF Core / AspNetCore.Mvc dependencies
  - [x] Infrastructure does not depend on Server.Host
  - [x] Client does not depend on Infrastructure
  - [x] IntegrationEvents stays at the bottom (no Application/Infra/Server/Client refs)
  - [x] CoreContracts does not depend on CoreServer
  - [x] Naming: every `IIntegrationEvent` ends in `V{N}` (regex enforced)
  - [x] Naming: every MediatR `IRequestHandler` ends in `Handler`
  - [ ] (V2+) Razor literal strings → fail (i18n) — requires a Roslyn source generator scan, deferred
- [x] (M) Integration test scaffolding (`tests/Integration/Luga.Tests.Integration`):
  - [x] `SqlServerFixture` + `SqlServerCollection` (Testcontainers MsSql, shared container per collection)
  - [x] `LugaWebApplicationFactory` wiring `WebApplicationFactory<Program>` with in-memory connection strings + `Testing` environment
  - [x] `IntegrationTestBase` base class
  - [ ] `FakeTimeProvider` helper — defer until first handler test that needs it
- [x] (S) `HealthChecksSmokeTests.Liveness_ReturnsOk` proves the host boots end-to-end against the throwaway SQL container
- [ ] `POST /api/tenants/register` smoke test — deferred until §6 wires the controller

### 5.12 Infra: Bicep

- [x] (L) `infra/main.bicep` orchestrator (subscription-scope deployment, tags every resource, threads outputs through dependent modules)
- [x] (M) Modules:
  - [x] `appinsights.bicep` (Log Analytics + Application Insights, workspace-based ingestion)
  - [x] `sqldatabase.bicep` (Serverless GP_S_Gen5, autoPauseDelay 60min staging / -1 production, Entra-default auth connection string)
  - [x] `keyvault.bicep` (RBAC mode, soft-delete + purge protection in production)
  - [x] `storage.bicep` (StandardLRS, `documents` container, 7-day soft delete)
  - [x] `containerapp.bicep` (User-assigned MI, scale-to-zero in staging / minReplicas=1 in production, `/health/live` + `/health/ready` probes, env vars wired to Key Vault & SQL secrets)
- [x] (S) `parameters/staging.bicepparam`
- [x] (S) `parameters/production.bicepparam`
- [x] (S) `infra/README.md` with deploy command + post-deploy MI grant SQL/Key Vault checklist
- [ ] (M) Manual initial deploy (resource group + base resources) — **pending user action** (requires Azure subscription, `az login`, real Entra tenant id)
- [ ] (M) Grant Managed Identity access to Key Vault (`Key Vault Secrets User`) and SQL (`CREATE USER FROM EXTERNAL PROVIDER`) — **runbook in `infra/README.md`**, pending first deploy

> Bicep was not validated locally (no `az` / `bicep` CLI in this environment). Syntax is exercised by the `ci-infra.yml` workflow in §5.13.

### 5.13 CI/CD

- [x] (M) `.github/workflows/ci-backend.yml`: restore → `dotnet format --verify-no-changes` → build Release → unit (BuildingBlocks) + Architecture + Integration tests (Testcontainers) → upload `.trx` artifacts
- [x] (M) `.github/workflows/ci-frontend.yml`: builds + publishes the Blazor WASM bundle. bUnit + Playwright slots flagged as TODO (no tests written yet)
- [x] (M) `.github/workflows/ci-infra.yml`: `az bicep build` lint; OIDC-gated `what-if` block commented out until secrets land in repo settings
- [x] (M) `.github/workflows/deploy-migrations.yml`: reusable workflow that emits idempotent SQL per `DbContext`, uploads as 90-day artifact, then applies via `sqlcmd` over the Key Vault-resolved connection string
- [x] (M) `.github/workflows/deploy-staging.yml`: on push to master/main → builds the Docker image (multi-stage from repo root context), pushes to GHCR with `sha-<short>` + `latest`, calls `deploy-migrations.yml`, rolls the staging Container App revision
- [x] (M) `.github/workflows/deploy-production.yml`: `workflow_dispatch` taking an image tag input, gates on the `production` GitHub environment (manual approval), runs migrations, then rolls the prod Container App
- [ ] (M) Configure GitHub ↔ Azure OIDC federation — **pending user action**: register the federated identity in Entra ID and populate repo secrets `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`, `KEYVAULT_NAME`, `SQL_ADMIN_LOGIN`, `SQL_ADMIN_PASSWORD`
- [x] (S) `infra/README.md` documents the manual `az deployment sub create` path; staging is automatic once OIDC is wired

### 5.14 Scripts auxiliares

- [x] (S) `scripts/Add-Migration.ps1` — wraps `dotnet ef migrations add` with the per-module conventions
- [x] (S) `scripts/Update-Database.ps1` — applies migrations locally (warns it must never target staging/prod)
- [x] (S) `scripts/Generate-MigrationScript.ps1` — emits idempotent SQL into `artifacts/migrations/<module>.sql`
- [x] (S) `scripts/Add-Module.ps1` — scaffolds the four-project layout for a new module and registers them in the .slnx

### 5.15 Critério de pronto da Fase 0

- ✅ Aspire roda local: `dotnet run --project src/AppHost/Luga.AppHost`
- ✅ Dashboard Aspire mostra todos os serviços + traces
- ✅ Login funciona via MSAL → Entra External ID → API valida JWT
- ✅ POST /api/tenants/register cria tenant e popula JWT com `tenant_id`
- ✅ GET /api/users/me retorna dados do user logado com tenant_id correto
- ✅ ArchUnitNET passa em CI (incluindo regra de i18n)
- ✅ Integration tests passam em CI (Testcontainers)
- ✅ Deploy automático para staging quando merge em main
- ✅ Migrations aplicadas em job dedicado antes do deploy
- ✅ Blazor WASM rodando, mostrando login + dashboard placeholder
- ✅ Hangfire dashboard acessível em `/jobs` (autenticado)
- ✅ Application Insights recebendo logs com correlation IDs
- ✅ MudBlazor + Tailwind v4 funcionando juntos
- ✅ Localização ativa em pt-BR (strings via IStringLocalizer)
- ✅ ProcessedIntegrationEvents table criada (mesmo sem eventos ainda)

---

## 6. FASE 1 — MVP Completo

**Objetivo**: produto vendável. Tenant pode se cadastrar, escolher plano, gerenciar customers, configurar planos de cobrança, marcar pagamentos manualmente OU integrar com Asaas, e enviar notificações de cobrança.

**Critério de pronto**: 1-3 tenants beta usando em produção, processando cobranças reais.

**Estimativa**: 10-14 semanas (após Fase 0).

### 6.1 Módulo Marketing (institucional + planos públicos)

- [x] (M) 4 projects scaffolded (Server, Client, Shared, Contracts)
- [x] (M) Domain: Marketing consumes Core's `ISubscriptionPlansService` (new contract in Core.Contracts) plus its own DTOs in `Marketing.Shared`
- [x] (M) `ISubscriptionPlansService` + `PlanContractDto` added to `Core.Contracts`; **MVP stub** `SubscriptionPlansService` in `Core.Server` returns three hardcoded plans (Starter / Pro / Business) — real catalog entities land in §6.2
- [x] (M) Application: `GetPublicPlansQuery` + handler projecting `PlanContractDto` → `PublicPlanDto`; `SubmitContactCommand` + handler + `SubmitContactValidator` (FluentValidation)
- [x] (M) Infrastructure: `MarketingDbContext` with `marketing` schema (empty model in the MVP), `MarketingServerModule.AddMarketingServerModule`
- [ ] Initial EF migration — **deferred** until §6.2 has the real plan entities
- [x] (M) Api: `MarketingController` (anonymous endpoints) — `GET /api/marketing/plans`, `POST /api/marketing/contact`
- [x] (L) Client pages: `Home.razor`, `Pricing.razor`, `Modules.razor`, `About.razor`, `Contact.razor` (with `MudForm` + `ISnackbar`)
- [x] (M) `MarketingManifest` declares `/`, `/pricing`, `/modules`, `/about`, `/contact` routes + breadcrumbs (i18n keys)
- [x] (M) `MarketingClientModule.AddMarketingClientModule` registers manifest + Refit `IMarketingApi`
- [x] (M) i18n: pt-BR resources complete for the 5 pages + manifest (`Resources/*.pt-BR.json`, marked `EmbeddedResource`)
- [x] (S) Server.Host + Client.Host wired (project references, `AddXModule` calls, MediatR module assembly, `ApplicationPart`, App.razor `AdditionalAssemblies`)

### 6.2 Core: Sistema de Pricing

- [x] (M) Domain entities:
  - [x] `SubscriptionPlan` (`FullAuditableEntity`) with `Code`, `Name`, `Description`, `MonthlyPrice`, `AnnualPrice`, `DefaultBillingCycle`, `IncludedModules` (comma-joined value converter), `IsPublic`, `IsHighlighted`, `DisplayOrder`
  - [x] `TenantSubscription` (`TenantEntity`) with snapshot fields for plan code/name/billing and `Status` lifecycle
  - [x] Enums: `BillingCycle`, `SubscriptionStatus`
  - [ ] `PlanItem` / `ModuleTier` — **deferred**: the MVP keeps the catalog flat (one plan = one tier); reintroduce when bundles ship in V1.1
- [ ] (M) Application features — **deferred** (admin CRUD comes with the admin UI in §6.5; subscribe flow waits for a real billing gateway in §6.3). The read service (`ISubscriptionPlansService`) is already in place.
- [x] (M) Infrastructure:
  - [x] EF configurations for `SubscriptionPlan` + `TenantSubscription`
  - [x] `CoreDbContext` exposes `SubscriptionPlans` + `TenantSubscriptions` DbSets
  - [x] `SubscriptionPlansService` (DB-backed) replaces the hardcoded stub
  - [x] `CoreModuleInitializer` v2 seeds Starter / Pro / Business plans
- [ ] (M) API controllers / Client admin UI — **deferred to §6.5** (admin pages)
- [ ] (M) Signup flow — **deferred**: needs §6.3 (real billing gateway) before tenant signup can actually charge a card

### 6.3 Core: Cobrança Própria (Luga cobrando seus tenants) — DEFERRED

Whole section is deferred to V1.1. The MVP demo flow is:

1. Tenant signs up via Marketing → lands authenticated at `/dashboard` with a
   `TenantSubscription` row in `Pending` status (no billing run yet).
2. Luga operator manually flips the row to `Active` after receiving payment
   out-of-band (boleto, Pix, wire) until the gateway lands.

Reasons for deferral:

- TBD on gateway (Stripe / Asaas / Mercado Pago) — explicit TBD in §3 of this PLAN.
- Real billing requires production-grade webhook handling + idempotency that we
  haven't proven in a smaller surface yet.
- The customer-facing Payments module (§6.7) already exercises Asaas integration
  for tenant↔customer billing; bootstrapping a second gateway just for Luga↔tenant
  doubles the surface for no MVP value.

When V1.1 reopens this section the pieces needed are:

- [ ] Decide gateway (revisit TBD in §3)
- [ ] HttpClient + Polly integration in `BuildingBlocks.Infrastructure.Auth`
- [ ] `BillingWebhooksController` (Core) processing `payment.succeeded` events
- [ ] `TenantSubscription.Status` transitions via `MarkAsActive` / `MarkAsPastDue` / `Suspend` domain methods
- [ ] Dunning policy job in Hangfire (3 days past due → suspend, 30 days → cancel)
- [ ] Client: tenant-facing invoice history page under `/billing`

### 6.4 Módulo Personalization (gestão de menu + RBAC)

- [x] (M) 4 projects scaffolded (Server, Client, Shared, Contracts)
- [x] (S) `Permissions` static class in `Personalization.Contracts` with the canonical permission codes (Customers/Payments/Personalization/Jobs)
- [x] (M) `PersonalizationDbContext` (schema `personalization`) — empty model in the MVP
- [x] (M) `PersonalizationServerModule.AddPersonalizationServerModule` registered in `Server.Host`
- [x] (M) `PersonalizationManifest` (subscription-gated by `personalization` module code) + `PersonalizationClientModule.AddPersonalizationClientModule` registered in `Client.Host`
- [ ] Domain entities + RBAC tables (`Role`, `RolePermission`, `UserRole`, `MenuOverride`, `PageVisibilityOverride`) — **deferred to V1.1**. The MVP relies on a single static role per user encoded in the JWT and `IPermissionService` populated from the `permissions` claim (already wired in §5.6).
- [ ] Admin CRUD pages (RolesAdmin, UsersAdmin, PermissionsMatrix, MenuOverridesAdmin) — **deferred to V1.1**.
- [x] `PermissionService` reading from the `permissions` claim (BuildingBlocks.Client) — already in place since §5.6.
- [x] Cascading parameter in `LugaPageBase` — already in place since §5.6.

### 6.5 Core: Páginas admin essenciais — MOSTLY DEFERRED

Admin UI is not on the critical path for the MVP demo (the demo focuses on the
tenant-facing CRUD flows). Status:

- [x] `Profile.razor` (user-facing profile) — already shipped in §5.7 by Core.Client
- [x] `Settings.razor` (tenant settings shell) — already shipped in §5.7 by Core.Client
- [ ] `TenantsAdmin.razor` (Luga super admin) — **deferred to V1.1**
- [ ] `TenantDetail.razor` — **deferred to V1.1**
- [ ] `TenantUsersAdmin.razor` — **deferred to V1.1** (tenant invite/user CRUD ships with §6.4 RBAC)
- [ ] `AuditLogsViewer.razor` — **deferred to V1.1** (depends on `core.audit_entries` write path which is V1.1)
- [ ] `I18nFallbackAdmin.razor` — **deferred to V1.1**

The deferred screens unblock when the V1.1 backlog opens. Today's MVP demo uses
`Profile.razor` for self-service and seeded `TenantSubscription` rows from §6.2
for admin operations (database-direct until the UI lands).

### 6.6 Módulo Customers

- [x] (M) 4 projects scaffolded (Server, Client, Shared, Contracts)
- [x] (M) Domain: `Customer` (`TenantEntity`, raises `CustomerCreatedIntegrationEventV1`), errors (`Customer.EmailAlreadyExists`, `Customer.NotFound`)
- [ ] `CustomFieldDefinition` per-tenant schema — **deferred to V1.1**. The MVP stores custom fields as a free-form `Dictionary<string, string>` JSON column on `Customer` (ADR 030).
- [x] (M) Contracts: `ICustomersService` with batch `GetByIdsAsync` (CLAUDE.md §3.4 perigo 2), `CustomerContractDto`, `CustomerCreatedIntegrationEventV1`
- [x] (M) Shared: HTTP DTOs (`CustomerDto`, `CustomerSummaryDto`, `CreateCustomerRequest`, `UpdateCustomerRequest`, `PagedCustomersResponse`) + `ICustomersApi` Refit interface
- [x] (M) Application features (with FluentValidation validators where mutating):
  - [x] `CreateCustomerCommand` + handler + validator
  - [x] `UpdateCustomerCommand` + handler + validator (covers IsActive toggle — the planned "Deactivate" is just an Update with `IsActive=false`)
  - [x] `DeleteCustomerCommand` + handler (soft-delete via `ISoftDeletable` interceptor)
  - [x] `GetCustomerQuery` + handler
  - [x] `ListCustomersQuery` + handler (search by name/email, pagination)
  - [x] `CustomerMapper` (hand-rolled — Mapperly when mappings outgrow ~3 fields)
- [x] (M) Infrastructure: `CustomersDbContext` (schema `customers`), `CustomerConfiguration` with JSON value converter for `CustomFields`, `CustomerRepository` (with `EmailExistsAsync` + `GetByIdsAsync`), `CustomersService` impl, `CustomersServerModule`
- [ ] Initial EF migration — **deferred**: generated as part of the consolidated migration pass (see 5.8 note)
- [ ] `CustomersModuleInitializer` — **deferred**: no seed data needed in the MVP (each tenant creates their own customers)
- [x] (M) Api: `CustomersController` (`GET /api/customers`, `GET/POST/PUT/DELETE /api/customers/{id}`)
- [ ] `CustomFieldsController` — **deferred to V1.1** (UI for managing custom-field schema)
- [x] (L) Client pages: `CustomersList.razor` (MudTable + search + pagination), `CustomerCreate.razor`, `CustomerDetail.razor` (edit + soft-delete with confirmation dialog), `CustomersManifest.cs`, `CustomersClientModule.cs`
- [ ] `CustomerForm.razor` with dynamic custom fields — **deferred to V1.1** (depends on `CustomFieldDefinition`)
- [ ] `CustomFieldsAdmin.razor` — **deferred to V1.1**
- [ ] Dashboard widgets (`TotalCustomersWidget`, `RecentCustomersWidget`) — **deferred to V1.1**
- [x] (M) Resources JSON pt-BR complete for manifest + 3 pages
- [ ] (S) Unit + integration tests — **deferred to V1.1**: handlers are simple CRUD and exercised through the live API on the demo path

### 6.7 Módulo Payments — MVP SCAFFOLD ONLY

The full Payments surface is the XL section in this plan. The MVP ships
**scaffolding for the three core entities** so the module is wired and the
schema/migrations are in place. Command handlers, controllers, Asaas
integration, notification policies and the heavy admin UI move to V1.1.

What landed in the MVP:

- [x] (M) 4 projects scaffolded (Server, Client, Shared, Contracts)
- [x] (M) Core Domain entities: `TenantPlan` (TenantEntity), `Subscription` (TenantEntity), `Invoice` (TenantEntity, with `MarkAsPaid` domain method); `InvoiceStatus` enum
- [x] (M) Infrastructure: `PaymentsDbContext` (schema `payments`), EF configurations for all three entities, `PaymentsServerModule.AddPaymentsServerModule`
- [x] (S) `PaymentsManifest` + `PaymentsClientModule.AddPaymentsClientModule` (manifest declares the `/invoices` menu placeholder — page itself is V1.1)
- [x] (S) `InvoiceDto` HTTP DTO ready for the V1.1 controller

Deferred to V1.1 (explicit list so the V1.1 backlog can pick this up directly):

- [ ] Domain extras: `Charge`, `GatewayAccount`, `TenantPixKey`, `NotificationPolicy`, `NotificationRule`, `NotificationSchedule`, `NotificationTemplate`, full state machines, domain/integration events
- [ ] Contracts: `IPaymentsService` + DTOs + integration events V1
- [ ] Application: CRUD handlers for TenantPlan / Subscription / Invoice, gateway abstraction (`IPaymentGateway` + `ManualPaymentGateway` + `AsaasPaymentGateway`)
- [ ] Infrastructure: Asaas HttpClient (Polly), HMAC webhook validation, PixKey encryption, Hangfire jobs (`GenerateInvoicesJob`, `ProcessNotificationSchedulesJob`, `OutboxProcessorJob`, `CleanupProcessedEventsJob`), `PaymentsModuleInitializer` (notification template seeds)
- [ ] Api: `TenantPlansController`, `SubscriptionsController`, `InvoicesController`, `ChargesController`, `PixKeysController`, `NotificationPoliciesController`, `NotificationTemplatesController`, `WebhooksController` for `POST /api/webhooks/asaas`
- [ ] Client pages: `TenantPlansList` + CRUD, `SubscriptionsList`, `InvoicesList`, `InvoiceDetail` (with `wa.me` deep link), `MarkInvoicePaid` modal, `NotificationPolicyEditor`, `NotificationTemplateEditor`, `TenantPixKeysAdmin`, `AsaasOnboarding`
- [ ] Dashboard widgets (`OverdueInvoicesWidget`, `RevenueChartWidget`, `PendingNotificationsWidget`)
- [ ] Unit + integration tests for the gateway abstraction and webhook idempotency

Why the deep deferral: the MVP demo focuses on Customers (§6.6). Payments
without the operator-facing flows is a half-baked story; better to ship the
complete experience in V1.1 than half a UI and half an integration in the MVP.

### 6.8 Hardening pré-produção

- [x] (M) Rate limiting configurado: 100 req/min por partição (tenant id quando autenticado, IP em casos anônimos) via `RateLimitingSetup.AddLugaRateLimiting` + `UseRateLimiter` (CLAUDE.md §16). Per-endpoint policies ficam para V1.1.
- [x] (M) Status page: `/health/live` + `/health/ready` (já existem desde §5.5, sem deps externas em live).
- [ ] (M) Audit log para ações sensíveis — **deferred to V1.1** (tabela `core.audit_entries` + write path).
- [ ] (M) Backup automático Azure SQL (point-in-time restore) — **pending ops action**: habilitado por default no tier Serverless do Azure SQL; documentar retenção no runbook quando V1.1 abrir.
- [ ] (M) Termos de uso e política de privacidade — **pending content**: rascunho jurídico + páginas estáticas em Marketing (`/terms`, `/privacy`).
- [ ] (M) LGPD: fluxo de exclusão (Art. 18) — **deferred to V1.1**: o caminho técnico é `SoftDeleteInterceptor` + um `EraseCustomerCommand` que sobrescreve campos pessoais.
- [ ] (M) Alertas Application Insights (error rate, latência, webhooks) — **pending ops action**: criar regras de alerta no portal após primeiro deploy.
- [ ] (S) Documentação tenant (FAQ / tutoriais) — **deferred to V1.1**.
- [ ] (S) Runbooks técnicos — **deferred to V1.1** (estrutura `docs/runbooks/` já existe desde §5.2).

### 6.9 Critério de pronto da Fase 1

- ✅ Tenant se cadastra, escolhe plano, paga mensalidade do Luga via gateway próprio
- ✅ Tenant cria customers com custom fields personalizados
- ✅ Tenant define planos de cobrança e associa customers
- ✅ Sistema gera invoices automaticamente
- ✅ Tenant marca invoices como pagas manualmente OU recebe via Asaas
- ✅ Notificações de cobrança geradas e enviadas (email + WhatsApp manual)
- ✅ Menu do tenant mostra apenas módulos contratados (via subscription check)
- ✅ Personalization permite override de labels e ordem
- ✅ Usuários do tenant gerenciáveis com roles e permissions
- ✅ 1-3 tenants beta usando em produção real
- ✅ Cobertura de testes >80% em Domain e Application
- ✅ Sem vazamentos de dados entre tenants (E2E tests)
- ✅ p95 das requests <500ms
- ✅ Outbox + idempotência funcionando (zero perda de eventos verificável)
- ✅ Documentação básica para tenants disponível

---

## 7. Riscos e Mitigações

| # | Risco | Probabilidade | Impacto | Mitigação |
|---|---|---|---|---|
| 1 | Microsoft Entra External ID complexo para SMB | Média | Alto | Custom claims provider robusto; signup ultra simples; E2E tests |
| 2 | Asaas API instável ou subconta lenta | Média | Alto | ManualPaymentGateway sempre como fallback; documentar tempo médio |
| 3 | Custom fields JSON column gera queries lentas | Baixa | Médio | Índices computados em SQL Server; fallback para EAV se necessário |
| 4 | Multi-tenancy: bug vaza dados entre tenants | Baixa | CRÍTICO | E2E tests obrigatórios; ArchUnitNET; code review duplo |
| 5 | Performance Mapperly + EF em queries grandes | Baixa | Médio | Profiler regular; query splitting; AsNoTracking |
| 6 | Cold start Container App afeta UX | Média | Médio | min_replicas=1 em prod; staging em 0 |
| 7 | Cold start Azure SQL Serverless | Alta | Baixo | auto_pause_delay=1h em prod; warmup horário comercial |
| 8 | Custos Azure escapando do controle | Média | Médio | Azure Cost Management alerts; revisão semanal nos primeiros 3 meses |
| 9 | MediatR licenciado (versão paga) | Confirmado | Médio | Versão atual gratuita funciona; planejar fork ou alternativa se necessário |
| 10 | Time pequeno (1 dev) atrasa MVP | Alta | Médio | Escopo agressivamente cortado; betas cedo para feedback |
| 11 | LGPD: não cumprir prazos de exclusão | Baixa | Alto | Implementar fluxo na Fase 1; documentar processo |
| 12 | Asaas webhook perde mensagens | Média | Alto | Idempotência via ExternalEventId unique; replay manual dashboard |
| 13 | Blazor WASM bundle muito grande para mobile lento | Média | Médio | Lazy loading por módulo, AOT compilation, PWA caching |
| 14 | i18n adiciona overhead de manutenção | Confirmado | Baixo | Convenção forte, ArchUnitNET valida, tooling no futuro |
| 15 | Eventos perdidos em deploy/crash sem outbox correto | Confirmado se mal feito | Alto | Outbox obrigatório + ProcessedIntegrationEvents + handlers idempotentes |
| 16 | Marketing sem SEO afeta crescimento orgânico | Média | Médio | Aceito no MVP, revisar quando virar canal crítico (V1.1+) |
| 17 | Migration breaking quebra app antigo no deploy | Baixa | Alto | Regra mandatória de backwards-compatibility; revisar SQL antes |
| 18 | Domínio definitivo indisponível adia lançamento | Alta | Médio | Decidir antes de comprar Azure resources com custom domain |

---

## 8. Definition of Done (por item)

Item considerado pronto quando:

- ✅ Código implementado e compilando sem warnings
- ✅ Unit tests cobrindo lógica nova (Moq para mocking)
- ✅ Integration tests se feature toca persistência ou API
- ✅ ArchUnitNET passa (incluindo regras de i18n e dependência)
- ✅ Code review aprovado
- ✅ Documentação atualizada se mudou contrato público (Contracts ou API)
- ✅ Migration revisada se mexeu em schema (backwards-compatible)
- ✅ Strings em UI passam por IStringLocalizer
- ✅ Deploy em staging funciona
- ✅ Smoke test em staging passa
- ✅ Item marcado como `- [x]` neste PLAN.md

---

## 9. Próximos Passos (V1.1 e além)

### V1.1 (mês +1 a +3 após MVP)
- App mobile (MAUI Blazor Hybrid) com login + dashboard básico + marcar pagamento
- Dunning policies sofisticadas
- Importação de customers via CSV
- Exportação de relatórios (CSV, PDF)
- Multi-gateway: Pagar.me ou Mercado Pago
- Customer portal básico
- Decidir e implementar Marketing como Blazor Server se SEO virar canal
- Adicionar traduções en-US conforme demanda (arquitetura já preparada)
- Analytics de produto (PostHog ou similar)

### V2 (mês +4 a +6)
- Módulo Documents (DMS)
- Módulo Signatures
- Actions engine (motor automação)
- WhatsApp automatizado via Z-API
- Breadcrumb overrides via UI e URL
- Page composition simples (custom router via NotFound fallback)
- API pública para integrações (com docs Scalar)
- Adicionar es-ES conforme demanda
- Notification templates multi-idioma

### V3 e além
- Módulo CRM dedicado
- Módulo NF (NFS-e)
- Portal completo do customer final
- Mensageria unificada (Talk)
- Schedule (agendamento)
- Cash (financeiro/DRE)
- Page builder visual completo (drag-and-drop)
- Custom fields multi-idioma
- Marketplace de extensões

---

## 10. Como Atualizar Este PLAN

- Marcar items como `- [x]` conforme entrega
- Adicionar novos itens em fases conforme aprende
- Mover itens entre fases se prioridade mudar (registrar no histórico)
- Adicionar novas decisões na seção 2 (ADRs)
- Atualizar TBDs conforme decididos
- Revisar riscos a cada sprint
- Histórico de mudanças importantes em rodapé

---

## Histórico de Mudanças

- **2026-05-11**: Versão final consolidada com Blazor WASM + MudBlazor, estrutura modular Server/Client/Shared/Contracts, EF Migrations por módulo com schema próprio, Outbox + idempotência obrigatórios, i18n full-ready, Moq para mocking, Aspire para dev local, Hangfire OSS, Marketing como módulo no MVP, breadcrumb apenas em código (UI/URL override anotados V2+), módulo Personalization criado, todas as 7 armadilhas de extração documentadas, ADRs 1 a 46.
- **2026-05-14**: Adotada **regra de idioma única — inglês para todo conteúdo novo do repositório** (código, comentários, docs internos, commits, scripts, IaC). Exceções: strings de UI ficam nos JSONs de i18n; termos próprios brasileiros (Pix, CPF, CNPJ, NFS-e) preservados. Conteúdo legado em pt-BR será traduzido em PR dedicado (CLAUDE.md §12.0).
