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

- [ ] (S) `IDomainEvent.cs`
- [ ] (S) `IIntegrationEvent.cs` (em IntegrationEvents)
- [ ] (M) Interfaces marker:
  - [ ] `IAuditable.cs`
  - [ ] `ISoftDeletable.cs`
  - [ ] `IMultiTenant.cs`
  - [ ] `IConcurrencyAware.cs`
  - [ ] `IActivatable.cs`
  - [ ] `IHasDomainEvents.cs`
- [ ] (M) Hierarquia de Entity:
  - [ ] `EntityBase.cs`
  - [ ] `AuditableEntity.cs`
  - [ ] `FullAuditableEntity.cs`
  - [ ] `TenantEntity.cs`
- [ ] (M) Result<T> pattern + Error + GeneralErrors
- [ ] (S) Unit tests para Result e GeneralErrors

### 5.4 BuildingBlocks.Application

- [ ] (S) `ITenantContext.cs`
- [ ] (S) `ICurrentUser.cs` (UserId + Username + PreferredCulture)
- [ ] (S) `IUnitOfWork.cs`
- [ ] (M) `IRepository<T>.cs` base interface
- [ ] (S) `PagedList<T>.cs` + `PagedRequest.cs`
- [ ] (M) MediatR pipeline behaviors:
  - [ ] `LoggingBehavior`
  - [ ] `ValidationBehavior`
  - [ ] `IdempotencyBehavior`
  - [ ] `PerformanceBehavior`
- [ ] (S) `ResultExtensions.cs` (ToActionResult)

### 5.5 BuildingBlocks.Infrastructure

- [ ] (M) `LugaDbContextBase.cs` (query filters globais, concurrency tokens)
- [ ] (L) Interceptors:
  - [ ] `AuditableEntityInterceptor`
  - [ ] `TenantIdInterceptor`
  - [ ] `SoftDeleteInterceptor`
  - [ ] `ActivationTrackingInterceptor`
  - [ ] `DomainEventToOutboxInterceptor`
- [ ] (M) Outbox pattern:
  - [ ] `OutboxMessage.cs`
  - [ ] `OutboxMessageConfiguration.cs` (base)
  - [ ] `ProcessedIntegrationEvent.cs`
  - [ ] `ProcessedIntegrationEventConfiguration.cs`
  - [ ] `IOutboxProcessor.cs`
  - [ ] `OutboxProcessor.cs` (Hangfire job)
- [ ] (M) Repository base:
  - [ ] `Repository<T>.cs` implementação genérica
  - [ ] `SpecificationEvaluator.cs` (Ardalis.Specification)
- [ ] (M) Migrations infrastructure:
  - [ ] `ModuleMigrationRunner.cs`
  - [ ] `ModuleInitializerRunner.cs`
  - [ ] `IModuleInitializer.cs` (em BuildingBlocks.Server)
  - [ ] `InitializationContext.cs`
  - [ ] `ModuleInitializationEntity.cs` (rastreia versões aplicadas)
- [ ] (M) Tenancy:
  - [ ] `TenantContext.cs`
  - [ ] `TenantContextMiddleware.cs`
  - [ ] `TenantClaimsExtractor.cs`
- [ ] (M) Auth:
  - [ ] `EntraExternalIdConfiguration.cs`
  - [ ] `JwtBearerSetup.cs`
  - [ ] `CurrentUserAccessor.cs`
  - [ ] `AuthPropagationHandler.cs` (DelegatingHandler para cross-service futuro)
- [ ] (M) Idempotency:
  - [ ] `IdempotencyKey.cs` entity
  - [ ] `IdempotencyStore.cs`
  - [ ] `IdempotencyMiddleware.cs`
- [ ] (S) Hangfire setup (`HangfireSetup.cs`, dashboard auth filter)
- [ ] (M) Observability:
  - [ ] `SerilogSetup.cs`
  - [ ] `OpenTelemetrySetup.cs` (com correlation IDs)
  - [ ] `HealthChecksSetup.cs`
- [ ] (M) Events:
  - [ ] `InProcessIntegrationEventBus.cs`
  - [ ] `IProcessedEventStore.cs` + impl
  - [ ] `DomainEventDispatcher.cs`
  - [ ] `IIntegrationEventHandler<T>.cs`
- [ ] (S) `PersistenceServiceCollectionExtensions.cs` (registra interceptors)

### 5.6 BuildingBlocks.Client (Blazor)

- [ ] (M) `IModuleManifest.cs`
- [ ] (M) `MenuItem.cs`, `DashboardWidget.cs`, `BreadcrumbRoute.cs`, `BreadcrumbSegment.cs`
- [ ] (M) `EmbeddableComponent.cs` (vazio, reservado V2+)
- [ ] (M) `LugaPageBase.cs` (cascading parameters)
- [ ] (M) Componentes compartilhados:
  - [ ] `MainMenu.razor`
  - [ ] `MenuSection.razor`
  - [ ] `PageBreadcrumb.razor`
  - [ ] `Breadcrumb.razor` (wrapper MudBreadcrumbs)
  - [ ] `PermissionGate.razor`
  - [ ] `NotFoundPage.razor`
- [ ] (M) Layouts:
  - [ ] `MainLayout.razor` (com áreas marketing/dashboard/admin)
  - [ ] `AuthLayout.razor` (login/signup)
- [ ] (M) Services:
  - [ ] `IBreadcrumbResolver.cs` + impl
  - [ ] `IPermissionService.cs` + impl
- [ ] (M) i18n setup:
  - [ ] Configurar `My.Extensions.Localization.Json`
  - [ ] `Resources/SharedStrings.pt-BR.json`
  - [ ] `IStringLocalizerFactory` configurado
  - [ ] CultureProvider em cascata (user > tenant > browser > fallback configurado)

### 5.7 Módulo Core (mínimo para autenticar)

- [ ] (M) Criar 4 projetos Core (Server, Client, Shared, Contracts)
- [ ] (M) Domain (Server):
  - [ ] `Tenant.cs` (FullAuditableEntity, NÃO IMultiTenant)
  - [ ] `TenantUser.cs` (TenantEntity, com PreferredCulture)
  - [ ] `TenantStatus.cs` enum
  - [ ] `TenantUserRole.cs` enum
  - [ ] Domain events
  - [ ] Errors
- [ ] (M) Contracts:
  - [ ] `ITenantsService.cs`
  - [ ] `IUsersService.cs`
  - [ ] DTOs (com batch methods desde dia 1)
  - [ ] Integration events V1 (`TenantCreatedIntegrationEventV1`)
- [ ] (M) Shared:
  - [ ] DTOs HTTP
  - [ ] `ICoreApi.cs` (Refit interface)
  - [ ] Validators
  - [ ] Resources JSON
- [ ] (M) Application — features mínimas:
  - [ ] `RegisterTenantCommand` + Handler + Validator
  - [ ] `GetCurrentTenantQuery` + Handler
  - [ ] `GetMyProfileQuery` + Handler
  - [ ] Mappers (Mapperly)
  - [ ] Repositórios (`ITenantRepository`, `ITenantUserRepository`)
- [ ] (M) Infrastructure:
  - [ ] `CoreDbContext` (extends LugaDbContextBase, schema "core")
  - [ ] Configurations EF Core
  - [ ] Repositórios concretos
  - [ ] `TenantsService` (impl ITenantsService)
  - [ ] `UsersService` (impl IUsersService)
  - [ ] Migration inicial
  - [ ] `CoreServerModule.cs` (composition root)
  - [ ] `CoreModuleInitializer.cs` (seeds básicos)
- [ ] (M) Api:
  - [ ] `TenantsController`
  - [ ] `UsersController`
  - [ ] Custom claims provider endpoint (POST /api/auth/enrich-claims)
- [ ] (M) Client:
  - [ ] `CoreManifest.cs` (IModuleManifest)
  - [ ] `CoreClientModule.cs`
  - [ ] Pages básicas: profile, settings
  - [ ] Resources JSON pt-BR (en-US e es-ES vazios/placeholder)

### 5.8 Bootstrapper (Luga.Server.Host)

- [ ] (M) `Program.cs` com:
  - [ ] BuildingBlocks setup
  - [ ] Entra External ID auth
  - [ ] Tenancy middleware
  - [ ] Observability (Serilog + OpenTelemetry + correlation)
  - [ ] MediatR + behaviors
  - [ ] CoreServerModule
  - [ ] Hangfire dashboard (`/jobs`)
  - [ ] Health checks (`/health/live`, `/health/ready`)
  - [ ] OpenAPI (Scalar UI)
  - [ ] Serve Blazor WASM (static files + fallback)
- [ ] (S) `appsettings.json` + `appsettings.Development.json`
- [ ] (S) `Dockerfile`
- [ ] (S) `.dockerignore`

### 5.9 Luga.Client.Host (Blazor WASM bootstrap)

- [ ] (M) `Program.cs` configurando:
  - [ ] MudBlazor
  - [ ] Tailwind v4
  - [ ] MSAL authentication
  - [ ] HttpClient + Refit clients
  - [ ] Localization
  - [ ] PWA registration
- [ ] (M) `App.razor` com Router e AdditionalAssemblies
- [ ] (M) `wwwroot/index.html`
- [ ] (M) PWA: manifest.json, service-worker.js, ícones
- [ ] (S) Login page funcional integrada com API

### 5.10 Aspire (Luga.AppHost)

- [ ] (M) `Luga.AppHost` projeto Aspire
- [ ] (M) Compose: SQL Server (container) + Server.Host + Client.Host
- [ ] (S) Documentar no README: `dotnet run --project src/AppHost/Luga.AppHost`
- [ ] (S) Verificar dashboard em http://localhost:18888

### 5.11 Testes

- [ ] (M) `Luga.Tests.Architecture`:
  - [ ] ArchUnitNET para regras de dependência
  - [ ] Domain não depende de EF Core
  - [ ] Módulos não referenciam internals de outros módulos
  - [ ] Application não depende de Infrastructure
  - [ ] Contracts não depende de Domain de outros módulos
  - [ ] Naming conventions (Handler, Command, Query suffixes)
  - [ ] Integration Events sempre com sufixo V{N}
  - [ ] Strings literais em Razor → falha (i18n)
- [ ] (M) Test base classes:
  - [ ] `IntegrationTestBase` com Testcontainers (SQL Server)
  - [ ] `WebApplicationFactoryFixture` customizada
  - [ ] FakeTimeProvider helper
- [ ] (S) Smoke test: POST /api/tenants/register cria tenant e retorna 201

### 5.12 Infra: Bicep

- [ ] (L) `infra/main.bicep` orquestrador
- [ ] (M) Modules:
  - [ ] `containerapp.bicep`
  - [ ] `sqldatabase.bicep` (Serverless)
  - [ ] `keyvault.bicep`
  - [ ] `storage.bicep`
  - [ ] `appinsights.bicep`
- [ ] (S) `parameters/staging.bicepparam`
- [ ] (S) `parameters/production.bicepparam`
- [ ] (M) Deploy manual inicial (RG, recursos base)
- [ ] (M) Configurar Managed Identity para Container App acessar Key Vault e SQL

### 5.13 CI/CD

- [ ] (M) `.github/workflows/ci-backend.yml`:
  - [ ] dotnet restore, build, test (unit + integration Testcontainers)
  - [ ] Architecture tests
  - [ ] dotnet format --verify-no-changes
- [ ] (M) `.github/workflows/ci-frontend.yml`:
  - [ ] bUnit tests
  - [ ] Playwright smoke (login)
  - [ ] build Blazor WASM
- [ ] (M) `.github/workflows/ci-infra.yml`:
  - [ ] az deployment what-if
- [ ] (M) `.github/workflows/deploy-migrations.yml`:
  - [ ] Gera scripts SQL idempotent por módulo
  - [ ] Arquiva como artifact
  - [ ] Aplica via sqlcmd em staging/prod
- [ ] (M) `.github/workflows/deploy-staging.yml`:
  - [ ] Push em main: build Docker, push GHCR, deploy Container App
  - [ ] Migrations rodam ANTES do deploy da app
- [ ] (M) `.github/workflows/deploy-production.yml`:
  - [ ] workflow_dispatch com input de versão
  - [ ] Deploy manual com aprovação
- [ ] (M) Configurar OIDC entre GitHub Actions e Azure
- [ ] (S) Documentar deploy no README

### 5.14 Scripts auxiliares

- [ ] (S) `scripts/Add-Migration.ps1`
- [ ] (S) `scripts/Update-Database.ps1`
- [ ] (S) `scripts/Generate-MigrationScript.ps1`
- [ ] (S) `scripts/Add-Module.ps1` (template para criar nova estrutura de módulo)

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

- [ ] (M) Criar 4 projetos Marketing
- [ ] (M) Domain (Server):
  - [ ] Não precisa de muito; consome `ISubscriptionPlansService` (Core.Contracts)
- [ ] (M) Application:
  - [ ] `GetPublicPlansQuery` + Handler (lista planos para landing)
- [ ] (M) Infrastructure:
  - [ ] MarketingDbContext (schema "marketing") — vazio inicial, pode crescer
  - [ ] Migration inicial
  - [ ] `MarketingServerModule.cs`
- [ ] (M) Api:
  - [ ] `MarketingController` (POST /api/marketing/contact, GET /api/marketing/plans)
- [ ] (L) Client (páginas Blazor WASM):
  - [ ] `Home.razor` (landing)
  - [ ] `Pricing.razor` (tabela de planos)
  - [ ] `Modules.razor` (descrição dos módulos)
  - [ ] `About.razor`
  - [ ] `Contact.razor`
  - [ ] Manifest declarando rotas e breadcrumbs
- [ ] (M) i18n: Resources JSON em pt-BR completos

### 6.2 Core: Sistema de Pricing

- [ ] (M) Domain entities:
  - [ ] `SubscriptionPlan` (FullAuditableEntity)
  - [ ] `PlanItem`
  - [ ] `ModuleTier`
  - [ ] `TenantSubscription` (TenantEntity)
  - [ ] Enums: BillingCycle, SubscriptionStatus
- [ ] (M) Application features:
  - [ ] `CreatePlanCommand` (admin only)
  - [ ] `UpdatePlanCommand`
  - [ ] `ListPlansQuery` (público)
  - [ ] `SubscribeTenantToPlanCommand`
  - [ ] `GetCurrentSubscriptionQuery`
  - [ ] `CheckModuleAccessQuery`
- [ ] (M) Infrastructure:
  - [ ] Repositories
  - [ ] CoreModuleInitializer popula planos iniciais (Free, Starter, Pro, Business)
- [ ] (M) API Controllers
- [ ] (M) Client: UI admin de gestão de catálogo (CRUD plans/tiers/bundles)
- [ ] (M) Client: signup fluxo de escolher plano

### 6.3 Core: Cobrança Própria (Luga cobrando seus tenants)

- [ ] (S) **Decidir gateway**: Stripe / Asaas / Mercado Pago
- [ ] (M) Integração com gateway
- [ ] (M) Webhook receiver para pagamento confirmado
- [ ] (M) Atualização de `TenantSubscription.Status`
- [ ] (M) Handling inadimplência (suspende tenant após X dias)
- [ ] (S) Client: histórico de faturas do tenant

### 6.4 Módulo Personalization (gestão de menu + RBAC)

- [ ] (M) Criar 4 projetos Personalization
- [ ] (M) Domain:
  - [ ] `Role.cs` (TenantEntity)
  - [ ] `Permission.cs` (constante string, não entity)
  - [ ] `RolePermission.cs` (TenantEntity)
  - [ ] `UserRole.cs` (TenantEntity)
  - [ ] `MenuOverride.cs` (TenantEntity)
  - [ ] `PageVisibilityOverride.cs` (TenantEntity)
- [ ] (M) Application features:
  - [ ] CRUD de Roles
  - [ ] Assign roles to users
  - [ ] Permission check
  - [ ] Menu override CRUD
- [ ] (M) Infrastructure:
  - [ ] PersonalizationDbContext (schema "personalization")
  - [ ] Repositórios
  - [ ] Migration inicial
  - [ ] Initializer com roles default (Admin, Manager, Operator, Viewer)
- [ ] (M) Api Controllers
- [ ] (L) Client:
  - [ ] `RolesAdmin.razor` (CRUD roles)
  - [ ] `UsersAdmin.razor` (lista users do tenant, assign roles)
  - [ ] `PermissionsMatrix.razor` (matriz role × permission)
  - [ ] `MenuOverridesAdmin.razor` (override de labels, ordem, plans)
  - [ ] Manifest
- [ ] (M) Behaviors:
  - [ ] `PermissionService` que lê role assignments + retorna permissions
  - [ ] Cascading parameter em LugaPageBase
  - [ ] `[Authorize(Permission = "...")]` attribute custom

### 6.5 Core: Páginas admin essenciais

- [ ] (M) `TenantsAdmin.razor` (Luga admin lista todos tenants)
- [ ] (M) `TenantDetail.razor` (ver/editar tenant específico)
- [ ] (M) `TenantUsersAdmin.razor` (gerenciar users de um tenant)
- [ ] (M) `MyProfile.razor` (user edita próprio perfil, idioma preferido)
- [ ] (M) `TenantSettings.razor` (configurações gerais do tenant)
- [ ] (M) `AuditLogsViewer.razor` (lê core.audit_entries)
- [ ] (S) `I18nFallbackAdmin.razor` (super admin configura fallback de idioma)

### 6.6 Módulo Customers

- [ ] (M) Criar 4 projetos Customers
- [ ] (M) Domain:
  - [ ] `Customer.cs` (TenantEntity, IHasDomainEvents)
  - [ ] `CustomFieldDefinition.cs` (TenantEntity)
  - [ ] `CustomFieldValue.cs` (Value Object)
  - [ ] Enums: CustomerStatus, CustomFieldType
  - [ ] Domain events
  - [ ] Errors
- [ ] (M) Contracts:
  - [ ] `ICustomersService.cs` com **batch methods desde dia 1** (`GetByIdsAsync`)
  - [ ] DTOs simplificados
  - [ ] Integration events V1
- [ ] (M) Shared:
  - [ ] DTOs HTTP
  - [ ] `ICustomersApi.cs` (Refit)
  - [ ] Validators
- [ ] (L) Application features:
  - [ ] `CreateCustomerCommand` + Handler + Validator
  - [ ] `UpdateCustomerCommand`
  - [ ] `DeactivateCustomerCommand`
  - [ ] `GetCustomerByIdQuery`
  - [ ] `ListCustomersQuery` (search, paginação, filtros)
  - [ ] `GetCustomersByIdsQuery` (batch)
  - [ ] `DefineCustomFieldCommand`
  - [ ] `UpdateCustomFieldCommand`
  - [ ] `DeleteCustomFieldCommand`
  - [ ] `ListCustomFieldsQuery`
  - [ ] Mappers, Repositórios
- [ ] (M) Infrastructure:
  - [ ] `CustomersDbContext`
  - [ ] Configurations (custom fields como JSON column)
  - [ ] Repositórios
  - [ ] `CustomersService` (impl Contracts)
  - [ ] Migration inicial
  - [ ] `CustomersServerModule.cs`
  - [ ] `CustomersModuleInitializer.cs`
- [ ] (M) Api:
  - [ ] `CustomersController`
  - [ ] `CustomFieldsController`
- [ ] (L) Client (Blazor):
  - [ ] `CustomersList.razor` (com MudDataGrid: filtros, ordenação, paginação)
  - [ ] `CustomerCreate.razor` (com wizard MudStepper se necessário)
  - [ ] `CustomerDetail.razor`
  - [ ] `CustomerEdit.razor`
  - [ ] `CustomFieldsAdmin.razor`
  - [ ] `CustomerForm.razor` (componente com custom fields dinâmicos)
  - [ ] `CustomersManifest.cs`
  - [ ] `CustomersClientModule.cs`
- [ ] (M) Widgets:
  - [ ] `TotalCustomersWidget.razor`
  - [ ] `RecentCustomersWidget.razor`
- [ ] (M) Resources JSON pt-BR completos
- [ ] (S) Unit + integration tests

### 6.7 Módulo Payments

- [ ] (M) Criar 4 projetos Payments
- [ ] (L) Domain:
  - [ ] `TenantPlan.cs` (TenantEntity)
  - [ ] `Subscription.cs` (TenantEntity)
  - [ ] `Invoice.cs` (TenantEntity, com snapshot de customer)
  - [ ] `Charge.cs` (TenantEntity)
  - [ ] `GatewayAccount.cs` (TenantEntity)
  - [ ] `TenantPixKey.cs` (TenantEntity, criptografada)
  - [ ] `NotificationPolicy.cs` (TenantEntity)
  - [ ] `NotificationRule.cs`
  - [ ] `NotificationSchedule.cs` (TenantEntity)
  - [ ] `NotificationTemplate.cs` (TenantEntity)
  - [ ] Enums (vários)
  - [ ] Máquinas de estado documentadas
  - [ ] Domain events + Integration events V1
  - [ ] Errors
- [ ] (M) Contracts:
  - [ ] `IPaymentsService.cs` com batch methods
  - [ ] DTOs
  - [ ] Integration events V1
- [ ] (M) Shared:
  - [ ] DTOs HTTP, Validators, Refit interface
- [ ] (XL) Application features:
  - [ ] TenantPlan CRUD
  - [ ] Subscription (Create, Cancel, Get, List)
  - [ ] Invoice (Generate via job, Get, List, MarkPaid)
  - [ ] Charge (Create, MarkPaid, ProcessWebhook)
  - [ ] Pix Keys (Register encrypted, List)
  - [ ] NotificationPolicy CRUD
  - [ ] NotificationTemplate CRUD + seed defaults
  - [ ] Gateway abstraction:
    - [ ] `IPaymentGateway.cs`
    - [ ] `ManualPaymentGateway.cs`
    - [ ] `AsaasPaymentGateway.cs`
  - [ ] Mappers, Validators, Repositórios
  - [ ] Event handlers (idempotentes, com ProcessedIntegrationEvents)
- [ ] (L) Infrastructure:
  - [ ] `PaymentsDbContext`
  - [ ] Configurations (criptografia PixKey)
  - [ ] Repositórios
  - [ ] `PaymentsService`
  - [ ] Asaas SDK integration (HttpClient + Polly)
  - [ ] Webhook signature validation HMAC
  - [ ] Migration inicial
  - [ ] Background jobs:
    - [ ] `GenerateInvoicesJob` (diário 06:00 BRT)
    - [ ] `ProcessNotificationSchedulesJob` (hourly)
    - [ ] `OutboxProcessorJob` (a cada 10s)
    - [ ] `CleanupProcessedEventsJob` (semanal)
  - [ ] `PaymentsServerModule.cs`
  - [ ] `PaymentsModuleInitializer.cs` (seed templates default)
- [ ] (M) Api:
  - [ ] `TenantPlansController`
  - [ ] `SubscriptionsController`
  - [ ] `InvoicesController`
  - [ ] `ChargesController`
  - [ ] `PixKeysController`
  - [ ] `NotificationPoliciesController`
  - [ ] `NotificationTemplatesController`
  - [ ] `WebhooksController` (POST /api/webhooks/asaas)
- [ ] (XL) Client (Blazor):
  - [ ] `TenantPlansList.razor` + CRUD
  - [ ] `SubscriptionsList.razor` + Create (assigning customer + plan)
  - [ ] `InvoicesList.razor` (com MudDataGrid)
  - [ ] `InvoiceDetail.razor` (com botão WhatsApp deep link wa.me)
  - [ ] `MarkInvoicePaid.razor` (modal)
  - [ ] `NotificationPolicyEditor.razor`
  - [ ] `NotificationTemplateEditor.razor`
  - [ ] `TenantPixKeysAdmin.razor`
  - [ ] `AsaasOnboarding.razor` (fluxo subconta)
  - [ ] `PaymentsManifest.cs`
- [ ] (M) Widgets:
  - [ ] `OverdueInvoicesWidget.razor`
  - [ ] `RevenueChartWidget.razor`
  - [ ] `PendingNotificationsWidget.razor`
- [ ] (M) Resources JSON pt-BR completos
- [ ] (M) Unit + integration tests

### 6.8 Hardening pré-produção

- [ ] (M) Rate limiting configurado por tenant + por IP
- [ ] (M) Audit log para ações sensíveis
- [ ] (M) Backup automático Azure SQL (point-in-time restore)
- [ ] (M) Termos de uso e política de privacidade publicados
- [ ] (M) LGPD: implementar exclusão de dados pessoais (Art. 18)
- [ ] (M) Status page interno (mínimo `/health/ready` público)
- [ ] (M) Alertas no Application Insights (error rate, latência, webhooks)
- [ ] (S) Documentação para tenants (FAQ, tutoriais)
- [ ] (S) Documentação técnica interna (runbooks)

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
