# CLAUDE.md — Luga CoreManager

> Manual permanente do projeto. Lido em toda sessão do Claude.
> Define visão, arquitetura, convenções e regras.
> Muda apenas quando decisões estruturais mudam.

---

## 1. Product Overview

**Luga CoreManager** é uma plataforma SaaS B2B brasileira **multi-produto** (Suite Integrada) para pequenos e médios negócios: academias, escolas, clubes, prestadores de serviço, profissionais liberais, clínicas pequenas.

Vendemos **gestão**, não processamento. NÃO movimentamos dinheiro dos customers finais. NÃO somos instituição de pagamento. Orquestramos assinaturas, faturas, cobrança, comunicação, documentos, e oferecemos automação configurável.

### 1.1 Posicionamento (B2B2C)

```
Luga CoreManager (você)
    │
    └── Tenant (academia/escola/clínica) — paga mensalidade ao Luga
            │
            └── Customer final (aluno/paciente/sócio) — paga ao Tenant
```

### 1.2 Receita

- Mensalidade do SaaS por Tenant (cobrada via Stripe/Asaas/Mercado Pago — TBD)
- Pricing modular: cada módulo tem preço próprio (mensal/anual) + tiers internos
- Bundles com desconto quando módulos comprados juntos
- Sem markup obrigatório sobre transações no MVP

### 1.3 ICP (Ideal Customer Profile)

Pequenos e médios negócios brasileiros:
- Volume típico: 5 a 500 cobranças recorrentes/mês
- Tipo: academias, escolas pequenas, clubes, salões, clínicas, prestadores, profissionais liberais

### 1.4 Domínio (TBD)

`luga.com` e `luga.com.br` indisponíveis. Decisão pendente entre:
- Manter "Luga" com variação (`lugamanager.com.br`, `lugacore.com.br`)
- Renomear produto para algo onde `.com.br` esteja livre
- Comprar premium (`luga.app`, `luga.io`)

**Nos arquivos de configuração e CI/CD, usar `luga.com` como placeholder até decisão final.** Substituir antes de comprar Azure resources com custom domain.

---

## 2. Platform Model: Suite Integrada

Luga CoreManager é uma **Suite**, não Federation.

- **Identidade unificada**: Tenant, User, Customer são entidades CORE compartilhadas (Customer NÃO é duplicado entre módulos)
- **Banco único** com schemas por módulo + schema `core` para entidades compartilhadas
- **Comunicação cross-module preferencialmente in-process** via Contracts ou Domain/Integration Events
- **Transações ACID atravessam módulos** quando o caso de uso exige (via outbox + eventual consistency)
- **UX é UMA aplicação** com áreas de módulos contratados
- **Pricing é por bundles/planos**, não por produto separado
- **Permissões unificadas** em RBAC com escopos por módulo

### 2.1 Módulos planejados

| Ordem | Módulo | Fase | Descrição |
|---|---|---|---|
| 1 | Core | MVP | Tenants, Users, Auth, Subscriptions ao Luga, Catálogo de Planos |
| 2 | Marketing | MVP | Site institucional, página de planos, signup público |
| 3 | Customers | MVP | Cadastro de end customers com custom fields configuráveis |
| 4 | Payments | MVP | Mensalidades, planos do tenant, cobrança Manual + Asaas |
| 5 | Personalization | MVP | Overrides de menu, RBAC, breadcrumbs (V2+), futuras customizações de UI |
| 6 | Documents (DMS) | V2 | Gestão eletrônica de documentos |
| 7 | Signatures | V2 | Assinatura eletrônica |
| 8 | CRM | V3 | CRM leve (leads, pipeline) |
| 9 | NF | V3 | Emissão NFS-e |
| 10 | Portal | V4 | Portal do customer final |
| 11 | Talk | V4 | Mensageria unificada |
| 12 | Schedule | V5 | Agendamento |
| 13 | Cash | V5 | Financeiro/DRE |

**Power-Up Rule**: cada módulo deve potencializar os existentes. Módulos isolados sem sinergia não são construídos.

---

## 3. Architecture Strategy: Modular Monolith → Extractable

Hoje é **um único deployment**. Amanhã, módulos podem virar microsserviços **sem redesign de domínio**. Exige disciplina arquitetural desde o dia 1.

### 3.1 Princípios não-negociáveis

1. **Cada módulo é dono dos seus dados.** Outros módulos NUNCA acessam tabelas alheias. Sem JOIN, sem Include, sem FK física entre módulos.
2. **Cada módulo tem seu DbContext próprio**, com seu schema próprio, com migrations independentes (history table separada).
3. **Comunicação cross-module APENAS via:**
   - **Contracts** (interfaces síncronas em `[Module].Contracts`)
   - **Integration Events** (assíncronos, versionados, contratos estáveis)
4. **Domain Events ≠ Integration Events.** Domain é interno e específico. Integration é público, estável, versionado.
5. **Outbox pattern em toda Integration Event** desde o dia 1.
6. **Transações respeitam fronteiras de módulo.** Cross-module é eventually consistent via outbox + integration events.
7. **Cada módulo tem seu composition root** (`AddXServerModule`, `AddXClientModule`).
8. **Validação arquitetural em CI** via ArchUnitNET. Build quebra se módulo referenciar internals de outro.

### 3.2 Caminho de extração futura (quando justificar)

Para extrair um módulo X como microsserviço:
1. Criar banco dedicado, dump+restore do schema X
2. Criar `X.Host` que carrega `XServerModule.AddXServerModule()`
3. Trocar registração: `IXService` de impl in-process para `RefitClient` HTTP
4. Trocar `IIntegrationEventBus` de in-memory para Azure Service Bus
5. Configurar `AuthPropagationHandler` para repassar JWT cross-service
6. Migrar dados via cutover ou dual-write
7. Decommissionar X do monolito

Nenhum passo envolve reescrever Domain, Application ou Contracts.

### 3.3 Quando NÃO extrair

Padrão é manter no monolito. Sinais válidos para extrair:
- Necessidade de escala muito diferente do resto
- Time dedicado bloqueado por deploy compartilhado
- Requisitos técnicos divergentes
- Módulo virou produto vendido separadamente

Sinais NÃO suficientes:
- "Microsserviços são modernos"
- "Quero deploy independente" (CI/CD resolve)

### 3.4 Os 7 perigos da extração (precauções obrigatórias)

Para extração ser viável quando precisar, estas regras devem ser seguidas DESDE O DIA 1:

#### Perigo 1 — Transações distribuídas
Quando extrair, transações ACID cross-module deixam de ser possíveis. **Mitigação**: modelar fluxos cross-module via Saga ou eventual consistency desde o monolito. Não escrever código que assume "tudo na mesma transação" entre módulos.

#### Perigo 2 — Performance N+1 cross-service
Loop chamando `customers.GetByIdAsync` 100x funciona em microssegundos in-process, mas 100 chamadas HTTP = 5+ segundos. **Mitigação**: `ICustomersService.GetByIdsAsync(ids)` (batch) deve existir desde o dia 1, mesmo no monolito.

#### Perigo 3 — Versionamento de contratos
Após extração, módulos evoluem em ritmos diferentes. **Mitigação**: Integration Events sempre com versão explícita no nome (`CustomerCreatedIntegrationEventV1`). Breaking change → V2 coexiste com V1 por período. Contracts versionados também.

#### Perigo 4 — Auth e tenant propagation
Chamada HTTP cross-service precisa propagar JWT do user para o serviço chamado, senão o destino não sabe quem é o tenant. **Mitigação**: `AuthPropagationHandler` (DelegatingHandler) planejado no `BuildingBlocks.Infrastructure`.

#### Perigo 5 — Webhooks e callbacks externos
Webhook do Asaas chega no Payments. Se Payments depende de atualizar customer (em outro serviço), NÃO fazer chamada HTTP direta — emitir Integration Event sempre. **Mitigação**: webhooks sempre terminam emitindo evento, nunca chamada síncrona cross-module.

#### Perigo 6 — Observability spread
Erro em produção entre serviços vira tortura sem correlation IDs. **Mitigação**: OpenTelemetry com Activity correlation desde o dia 1, mesmo no monolito.

#### Perigo 7 — Coordenação de migrations
Cada módulo aplica suas próprias migrations. **Mitigação**: history table separada por schema (`customers.__EFMigrationsHistory`, etc.). Migrations sempre backwards-compatible (não quebram app antigo durante deploy).

---

## 4. Tech Stack

### 4.1 Backend

| Categoria | Tecnologia | Justificativa |
|---|---|---|
| Runtime | .NET 10 (LTS, GA nov/2025) | Mais recente LTS, performance |
| Linguagem | C# 14 | Primary constructors, collection expressions |
| Web | ASP.NET Core 10 com **Controllers** | Conventional (decisão) |
| ORM | EF Core 10 com Migrations | Maduro, nativo SQL Server |
| Banco | Azure SQL Database Serverless | Stack Microsoft, auto-pause economiza |
| Auth | Microsoft Entra External ID (JWT) | Stack 100% Microsoft |
| CQRS / Mediator | **MediatR** (Jimmy Bogard) | Industry standard, comunidade enorme |
| Validation | FluentValidation | Padrão, integra com MediatR pipeline |
| Mapping | **Mapperly** (source-generated) | Sem reflection, performante, explícito |
| Background Jobs | **Hangfire OSS** (SQL Server storage) | Dashboard pronto, fila por módulo, free |
| Specifications | Ardalis.Specification | Para queries complexas reutilizáveis |
| Result Pattern | Próprio em BuildingBlocks | Sem dependência externa |
| Logging | Serilog | Industry standard, structured |
| Telemetry | OpenTelemetry + Application Insights | Padrão CNCF |
| Email | Mailtrap Email API | Free tier generoso |
| WhatsApp MVP | Deep link `wa.me` (manual) | Gratuito |
| HTTP Client (cross-service) | Refit + Polly | Type-safe, resilient |
| Tempo | **TimeProvider** (sempre) | Testável, .NET 8+ |

### 4.2 Frontend Web

| Categoria | Tecnologia |
|---|---|
| Framework | **Blazor WebAssembly** (.NET 10) |
| UI Library | **MudBlazor** |
| Styling | Tailwind v4 |
| API Client | Refit (interfaces compartilhadas via `.Shared` projects) |
| Auth | Microsoft.Authentication.WebAssembly.Msal |
| State | Serviços scoped + cascading parameters |
| Forms | EditForm + FluentValidation.Blazor |
| Localização | **My.Extensions.Localization.Json + IStringLocalizer** |
| Tabelas | MudBlazor DataGrid |
| Charts | MudBlazor Chart |
| Wizard/Stepper | MudStepper |
| Breadcrumb | MudBreadcrumbs |
| PWA | manifest.json + service-worker.js |
| Testing | bUnit + Playwright (E2E) |

### 4.3 Frontend Mobile (V1.1+)

| Categoria | Tecnologia |
|---|---|
| Framework | **.NET MAUI Blazor Hybrid** |
| Reuso de código | ~95% (mesmos `.Modules.X.Client` projects do Web) |
| Push notifications | Azure Notification Hubs (APNs + FCM) |
| APIs nativas | MAUI Essentials (câmera, biometria, etc.) |

### 4.4 Marketing (decisão atual: módulo Blazor WASM)

No MVP, **Marketing é um módulo como qualquer outro** dentro do Blazor WASM single-app. Tem SEO limitado (Blazor WASM não é ótimo para crawlers).

**Anotado para revisão futura**: se SEO orgânico virar canal crítico, considerar mover Marketing para:
- Blazor Server separado (`Luga.Marketing.Host`) — stack único .NET
- Astro/Next.js separado — SEO ótimo, stack adicional

Decisão pendente; manter como módulo Blazor WASM por enquanto.

### 4.5 Infra & DevOps

| Categoria | Tecnologia |
|---|---|
| Hosting | Azure Container Apps (consumption) |
| Container Registry | GitHub Container Registry (público) |
| IaC | Bicep (desde Fase 0) |
| CI/CD | GitHub Actions com OIDC (sem secrets) |
| Storage | Azure Blob Storage |
| Secrets | Azure Key Vault + Managed Identity |
| Observability | Application Insights + Log Analytics |
| Dev local | **.NET Aspire 13** (orquestração + dashboard) |

### 4.6 IDE recomendada

**Visual Studio 2022 17.12+** (decisão).

### 4.7 Testing Stack

| Tipo | Tecnologia |
|---|---|
| Unit (Domain) | xUnit + FluentAssertions |
| Unit (Application) | xUnit + **Moq** + FluentAssertions |
| Integration | Testcontainers (SQL Server) |
| Architecture | ArchUnitNET |
| Component (Blazor) | bUnit |
| E2E (front) | Playwright |

---

## 5. Repository Structure

**Monorepo full-stack** — backend + frontend + mobile + infra no mesmo repo.

```
luga-coremanager/                          # único repo no GitHub
├── README.md
├── CLAUDE.md
├── PLAN.md
├── .gitignore
├── .editorconfig
├── Directory.Build.props
├── Directory.Packages.props
├── global.json
│
├── src/
│   ├── Luga.CoreManager.slnx                # .slnx (.NET 10 default)
│   ├── BuildingBlocks/
│   │   ├── Luga.BuildingBlocks.Domain/
│   │   ├── Luga.BuildingBlocks.Application/
│   │   ├── Luga.BuildingBlocks.Infrastructure/
│   │   ├── Luga.BuildingBlocks.IntegrationEvents/
│   │   ├── Luga.BuildingBlocks.Server/         # IModuleInitializer, etc.
│   │   └── Luga.BuildingBlocks.Client/         # IModuleManifest, LugaPageBase, componentes
│   │
│   ├── Modules/
│   │   ├── Core/
│   │   │   ├── Luga.Modules.Core.Server/
│   │   │   ├── Luga.Modules.Core.Client/
│   │   │   ├── Luga.Modules.Core.Shared/
│   │   │   └── Luga.Modules.Core.Contracts/
│   │   ├── Marketing/
│   │   │   └── (mesma estrutura)
│   │   ├── Customers/
│   │   │   └── (mesma estrutura)
│   │   ├── Payments/
│   │   │   └── (mesma estrutura)
│   │   └── Personalization/
│   │       └── (mesma estrutura)
│   │
│   ├── Hosts/
│   │   ├── Luga.Server.Host/                   # API + serve Blazor WASM
│   │   ├── Luga.Client.Host/                   # Blazor WASM bootstrap
│   │   └── Luga.Mobile.Host/                   # MAUI Blazor Hybrid (V1.1+)
│   │
│   └── AppHost/
│       └── Luga.AppHost/                       # .NET Aspire dev local
│
├── tests/
│   ├── Architecture/
│   │   └── Luga.Tests.Architecture/            # ArchUnitNET
│   ├── BuildingBlocks/
│   └── Modules/
│       ├── Core/
│       ├── Customers/
│       └── Payments/
│
├── infra/
│   ├── main.bicep
│   ├── modules/
│   └── parameters/
│
├── docs/
│   ├── architecture/
│   ├── adrs/
│   └── runbooks/
│
├── scripts/
│   ├── Add-Migration.ps1
│   ├── Update-Database.ps1
│   └── Generate-MigrationScript.ps1
│
└── .github/workflows/
    ├── ci-backend.yml
    ├── ci-frontend.yml
    ├── ci-infra.yml
    ├── deploy-migrations.yml
    ├── deploy-staging.yml
    └── deploy-production.yml
```

---

## 6. Module Structure: 3 Projects per Module (+ Contracts)

**Convenção de nomes**: `Luga.Modules.{NomeDoModulo}.{Camada}` (plural).

Cada módulo tem **4 projetos**:

### `Luga.Modules.X.Server` — Backend completo

Contém Domain + Application + Infrastructure + Api juntos por simplicidade.

```
Luga.Modules.Customers.Server/
├── Domain/
│   ├── Entities/
│   │   ├── Customer.cs                      # herda TenantEntity
│   │   └── CustomFieldDefinition.cs
│   ├── Enums/
│   ├── Events/
│   ├── ValueObjects/
│   ├── Errors/
│   └── Specifications/
├── Application/
│   ├── Features/                            # vertical slice por feature
│   │   ├── CreateCustomer/
│   │   ├── UpdateCustomer/
│   │   └── ...
│   ├── Mappers/                             # Mapperly
│   ├── Repositories/                        # interfaces (renomeado de Abstractions)
│   │   ├── ICustomerRepository.cs
│   │   └── ICustomFieldDefinitionRepository.cs
│   └── EventHandlers/
├── Infrastructure/
│   ├── Persistence/
│   │   ├── CustomersDbContext.cs            # extends LugaDbContextBase
│   │   ├── Configurations/
│   │   └── Migrations/                      # privadas do módulo
│   ├── Repositories/                        # implementações
│   └── Services/                            # ICustomersService impl
├── Api/
│   └── Controllers/
├── CustomersServerModule.cs                 # composition root
└── CustomersModuleInitializer.cs            # IModuleInitializer (seed)
```

### `Luga.Modules.X.Client` — Frontend Blazor

```
Luga.Modules.Customers.Client/
├── Pages/                                   # @page Blazor padrão
│   ├── CustomersList.razor
│   ├── CustomerDetail.razor
│   └── CustomFields.razor
├── Components/                              # componentes internos
│   ├── CustomerForm.razor
│   ├── CustomerTable.razor
│   └── CustomFieldEditor.razor
├── Widgets/                                 # componentes para dashboard
│   ├── TotalCustomersWidget.razor
│   └── RecentCustomersWidget.razor
├── Services/
│   └── CustomersApiClient.cs                # Refit client
├── Resources/                               # i18n
│   ├── CustomersList.pt-BR.json
│   ├── CustomersList.en-US.json
│   ├── CustomersList.es-ES.json
│   └── ...
├── CustomersManifest.cs                     # IModuleManifest
└── CustomersClientModule.cs                 # extension method para registrar
```

### `Luga.Modules.X.Shared` — DTOs e tipos compartilhados

```
Luga.Modules.Customers.Shared/
├── DTOs/
│   ├── CustomerDto.cs
│   ├── CustomerSummaryDto.cs
│   ├── CreateCustomerRequest.cs
│   └── UpdateCustomerRequest.cs
├── Contracts/
│   └── ICustomersApi.cs                     # interface Refit (back e front usam)
└── Validators/
    └── CreateCustomerValidator.cs           # FluentValidation reusada
```

**Importante**: `.Shared` é reusado pelo `.Server` (deserializa requests, envia responses) E pelo `.Client` (envia requests via Refit, recebe responses tipadas). Zero geração de código, zero drift.

### `Luga.Modules.X.Contracts` — Comunicação cross-module

```
Luga.Modules.Customers.Contracts/
├── ICustomersService.cs                     # consumido in-process por OUTROS módulos
├── DTOs/
│   ├── CustomerContractDto.cs               # DTO simplificado para cross-module
│   └── ...
└── IntegrationEvents/
    ├── CustomerCreatedIntegrationEventV1.cs
    ├── CustomerUpdatedIntegrationEventV1.cs
    └── CustomerDeactivatedIntegrationEventV1.cs
```

**Diferença entre `.Shared` e `.Contracts`**:
- `.Shared`: fronteira HTTP entre Client e Server **do mesmo módulo**
- `.Contracts`: fronteira lógica entre módulos diferentes do back

---

## 7. Backend Architecture

### 7.1 Princípios

- **DDD** (tático e estratégico)
- **SOLID** aplicado pragmaticamente
- **Clean Architecture** (dependências apontam para dentro)
- **CQRS leve** via MediatR
- **Vertical Slice** dentro de cada módulo
- **Repository Pattern** com base genérica + repositórios específicos
- **TimeProvider** sempre (nunca `DateTime.Now` direto)

### 7.2 Regras de dependência (validadas em CI por ArchUnitNET)

```
Luga.BuildingBlocks.Domain
  └─ depende de: NADA (puro C#)

Luga.BuildingBlocks.Application
  └─ depende de: BuildingBlocks.Domain, MediatR, FluentValidation

Luga.BuildingBlocks.Infrastructure
  └─ depende de: BuildingBlocks.Application, BuildingBlocks.Domain
                 EF Core, Hangfire, Serilog, OpenTelemetry

Luga.BuildingBlocks.IntegrationEvents
  └─ depende de: BuildingBlocks.Domain (mínimo)

Luga.BuildingBlocks.Server
  └─ depende de: BuildingBlocks.Application + Infrastructure

Luga.BuildingBlocks.Client
  └─ depende de: BuildingBlocks.IntegrationEvents (mínimo)
                 NÃO depende de Infrastructure

Luga.Modules.X.Server
  └─ depende de: BuildingBlocks.Server + Application + Infrastructure + Domain
                 Modules.X.Shared
                 Modules.X.Contracts
                 Contracts de OUTROS módulos (se necessário)
                 NUNCA Domain/Application/Infrastructure de outros módulos

Luga.Modules.X.Client
  └─ depende de: BuildingBlocks.Client
                 Modules.X.Shared
                 NUNCA Modules.X.Server
                 NUNCA outros Modules.X.Client (front modular isolado)

Luga.Modules.X.Shared
  └─ depende de: BuildingBlocks.Domain (mínimo, só para tipos básicos)

Luga.Modules.X.Contracts
  └─ depende de: BuildingBlocks.IntegrationEvents (NADA mais)

Luga.Server.Host
  └─ depende de: TODOS Luga.Modules.X.Server

Luga.Client.Host
  └─ depende de: TODOS Luga.Modules.X.Client

Luga.Mobile.Host (V1.1+)
  └─ depende de: TODOS Luga.Modules.X.Client (mesmos do web)
```

### 7.3 Hierarquia de Entity Base

Em `Luga.BuildingBlocks.Domain.Entities`:

```csharp
// Base mínima — apenas Id e RowVersion
public abstract class EntityBase : IConcurrencyAware
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public byte[] RowVersion { get; set; } = [];
}

// Auditável — quem/quando criou/atualizou (com username + UserId)
public abstract class AuditableEntity : EntityBase, IAuditable
{
    public Guid CreatedById { get; set; }
    public string CreatedByUsername { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public Guid? UpdatedById { get; set; }
    public string? UpdatedByUsername { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

// Auditável + soft delete (com motivo)
public abstract class FullAuditableEntity : AuditableEntity, ISoftDeletable
{
    public Guid? DeletedById { get; set; }
    public string? DeletedByUsername { get; set; }
    public DateTime? DeletedOn { get; set; }
    public bool IsDeleted { get; set; }
    public string? DeletionReason { get; set; }
}

// Multi-tenant + auditável + soft delete (mais comum no Luga)
public abstract class TenantEntity : FullAuditableEntity, IMultiTenant
{
    public Guid TenantId { get; set; }
}
```

### 7.4 Interfaces Marker

Em `Luga.BuildingBlocks.Domain.Abstractions`:

| Interface | Propósito |
|---|---|
| `IAuditable` | CreatedBy/UpdatedBy (UserId + username snapshot) + timestamps |
| `ISoftDeletable` | DeletedBy + DeletionReason + IsDeleted |
| `IMultiTenant` | TenantId (auto-populado em INSERT pelo interceptor) |
| `IConcurrencyAware` | RowVersion (mapeado para ROWVERSION SQL Server) |
| `IActivatable` | IsActive + ActivatedOn/DeactivatedOn + DeactivationReason |
| `IHasDomainEvents` | DomainEvents collection + ClearDomainEvents() |

### 7.5 EF Core Interceptors

**Decisão: Interceptors em vez de override de SaveChangesAsync.** Localização: `Luga.BuildingBlocks.Infrastructure.Persistence.DbContextInterceptors`.

| Interceptor | Responsabilidade |
|---|---|
| `AuditableEntityInterceptor` | Popula CreatedBy/UpdatedBy/timestamps |
| `TenantIdInterceptor` | Auto-popula TenantId em INSERT |
| `SoftDeleteInterceptor` | Intercepta DELETE → converte em UPDATE com IsDeleted=true |
| `ActivationTrackingInterceptor` | Registra ActivatedOn/DeactivatedOn quando IsActive muda |
| `DomainEventToOutboxInterceptor` | Captura IIntegrationEvent → grava em OutboxMessage |

Registrados via DI uma vez em `BuildingBlocks.Infrastructure`. Cada DbContext de módulo recebe automaticamente via `AddInterceptors(sp.GetServices<ISaveChangesInterceptor>())`.

### 7.6 LugaDbContextBase

Localização: `Luga.BuildingBlocks.Infrastructure.Persistence.LugaDbContextBase`.

Classe abstrata base. Aplica:
- **Concurrency tokens automáticos** para `IConcurrencyAware`
- **Query filters globais** para `IMultiTenant` (TenantId) e `ISoftDeletable` (IsDeleted)
- Implementa `IUnitOfWork`

Cada DbContext de módulo herda e fica enxuto:

```csharp
public sealed class CustomersDbContext(DbContextOptions<CustomersDbContext> options)
    : LugaDbContextBase(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomFieldDefinition> CustomFieldDefinitions => Set<CustomFieldDefinition>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<ProcessedIntegrationEvent> ProcessedIntegrationEvents => Set<ProcessedIntegrationEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("customers");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        base.OnModelCreating(modelBuilder);   // aplica filtros globais + concurrency
    }
}
```

### 7.7 Repository Pattern

Base genérica em `BuildingBlocks.Application.Repositories.IRepository<T>`:

```csharp
public interface IRepository<TEntity> where TEntity : EntityBase
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<TEntity>> GetByIdRequiredAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void Remove(TEntity entity);  // soft delete se ISoftDeletable
    IQueryable<TEntity> Query();
    Task<PagedList<TEntity>> ListAsync(
        ISpecification<TEntity>? spec, int page, int pageSize, CancellationToken ct = default);
}
```

Implementação base em `BuildingBlocks.Infrastructure.Persistence.Repositories.Repository<T>`.

Repositórios específicos herdam e adicionam métodos:

```csharp
public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<IReadOnlyList<Customer>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
}
```

### 7.8 IUnitOfWork

`LugaDbContextBase` implementa `IUnitOfWork`. Handlers injetam `IUnitOfWork` para chamar `SaveChangesAsync`:

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

### 7.9 EF Migrations Strategy

**Cada módulo tem migrations próprias com history table em schema próprio.**

```csharp
// Configuração em XServerModule.cs
services.AddDbContext<CustomersDbContext>((sp, options) =>
{
    options.UseSqlServer(
        configuration.GetConnectionString("Default"),
        sql => sql.MigrationsHistoryTable(
            tableName: "__EFMigrationsHistory",
            schema: "customers"));    // ⭐ schema próprio

    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
});
```

Resultado no banco:
```
core.__EFMigrationsHistory
customers.__EFMigrationsHistory
payments.__EFMigrationsHistory
marketing.__EFMigrationsHistory
personalization.__EFMigrationsHistory
```

**Em dev**: aplicação automática no startup via flag `ApplyMigrationsOnStartup=true`.

**Em prod**: job dedicado em CI/CD aplica migrations **antes** do deploy da app. Scripts SQL idempotent gerados e arquivados como artefato.

**Comandos sempre com `--context` explícito**:
```bash
dotnet ef migrations add NomeDaMigration \
    --project src/Modules/Customers/Luga.Modules.Customers.Server \
    --startup-project src/Hosts/Luga.Server.Host \
    --context CustomersDbContext \
    --output-dir Infrastructure/Persistence/Migrations
```

**Scripts PowerShell em `/scripts`** reduzem verbosidade:
```powershell
.\scripts\Add-Migration.ps1 -Module Customers -Name AddCustomFieldsTable
```

**`ModuleMigrationRunner`** em `BuildingBlocks.Infrastructure` orquestra aplicação de migrations de todos os DbContexts no startup (dev) ou sob comando (prod).

### 7.10 Backwards-Compatible Migrations

**Regra crítica**: nova migration NUNCA quebra a versão antiga da app que ainda está rodando durante deploy.

```csharp
// ❌ NUNCA — quebra app antigo
builder.RenameColumn(name: "Name", table: "Customers", newName: "FullName");

// ✅ SIM — fluxo seguro em 3 deploys
// Deploy 1: adiciona FullName, copia dados, mantém Name
// Deploy 2: app passa a ler/escrever FullName, ignora Name
// Deploy 3: drop Name (depois de garantir que nada usa)
```

**Adições são sempre seguras**: nova coluna nullable, nova tabela, novo índice.
**Remoções exigem múltiplos deploys**: deprecar → migrar uso → drop.

### 7.11 Seed de Dados via IModuleInitializer

**Distinção crítica**:
- **Migrations** = schema (DDL)
- **Module Initializers** = dados (DML)

Interface em `BuildingBlocks.Server`:

```csharp
public interface IModuleInitializer
{
    string ModuleCode { get; }
    int Version { get; }
    Task InitializeAsync(InitializationContext context, CancellationToken ct);
}
```

Cada módulo tem seu `XModuleInitializer`. `ModuleInitializerRunner` registra versões aplicadas em `core.module_initializations`:

```
module_code (PK) | version (PK) | applied_at | applied_by
```

Idempotente: só roda versão nova. Quando você incrementa `Version` no código (de 1 para 2), o initializer roda só a parte nova.

### 7.12 Multi-tenancy

- **Estratégia**: shared database, shared schema, discriminator por `TenantId`
- **TenantId resolvido** em middleware via claim `tenant_id` do JWT
- **Global query filter** em `LugaDbContextBase` aplica TenantId automaticamente
- **TenantIdInterceptor** auto-popula TenantId em INSERT
- `IgnoreQueryFilters()` exige justificativa em PR

### 7.13 Auth: Microsoft Entra External ID

- **1 Entra External ID Tenant** para tudo (modelo single-tenant)
- Custom claim **`tenant_id`** no JWT identifica o app-tenant
- **Custom claims provider** chama API do Luga durante login para enriquecer JWT
- API valida JWT (Microsoft.Identity.Web) e popula `ITenantContext`

### 7.14 TimeProvider

**Sempre** usar `TimeProvider` injetado em vez de `DateTime.Now`/`DateTime.UtcNow`:

```csharp
public sealed class CreateCustomerHandler(
    ICustomerRepository repo,
    IUnitOfWork uow,
    TimeProvider timeProvider) : IRequestHandler<CreateCustomerCommand, Result<CreateCustomerResponse>>
{
    public async Task<Result<CreateCustomerResponse>> Handle(...)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        // ...
    }
}
```

Em testes, usar `FakeTimeProvider` (`Microsoft.Extensions.TimeProvider.Testing`).

### 7.15 Result<T> Pattern

Próprio em `BuildingBlocks.Domain.Common`. Sem exceptions para erros de negócio.

```csharp
public class Result { public bool IsSuccess { get; } public Error Error { get; } }
public class Result<T> : Result { public T Value { get; } }
public sealed record Error(string Code, string Message);
```

HTTP conversion via extension `ToActionResult()` retornando RFC 7807 ProblemDetails.

### 7.16 MediatR Pipeline Behaviors

Em `BuildingBlocks.Application.Behaviors`:

1. `LoggingBehavior` — loga requisição/resposta com TenantId
2. `ValidationBehavior` — roda FluentValidation antes do handler
3. `IdempotencyBehavior` — verifica Idempotency-Key
4. `PerformanceBehavior` — alerta queries lentas

### 7.17 Outbox + Integration Events: Garantia de Entrega

**Sistema obrigatório para evitar perda de eventos.**

#### Fluxo correto (garantia de delivery)

```
1. Handler cria entidade + adiciona IntegrationEvent à entidade
2. SaveChanges → DomainEventToOutboxInterceptor grava OutboxMessage na MESMA transação SQL
3. Transação commita → evento PERSISTIDO no banco (mesmo se app crashar agora, evento está salvo)
4. OutboxProcessor (Hangfire recurring job a cada 10s) lê OutboxMessages não processadas
5. Para cada message, chama IIntegrationEventBus.PublishAsync()
6. InProcessBus chama handlers in-memory (no monolito) ou Service Bus (após extração)
7. Handler processa → marca OutboxMessage como Processed
```

#### Handlers DEVEM ser idempotentes

Mesmo com Outbox, handlers podem rodar mais de uma vez (crash entre side effect e marcação de processed). **Solução**: tabela `[Schema].processed_integration_events`:

```
event_id (PK) | handler_name (PK) | processed_at
```

```csharp
public sealed class CustomerCreatedHandler(
    IInvoiceRepository invoices,
    IProcessedEventStore processedEvents,
    IUnitOfWork uow) : IIntegrationEventHandler<CustomerCreatedIntegrationEventV1>
{
    public async Task HandleAsync(CustomerCreatedIntegrationEventV1 evt, CancellationToken ct)
    {
        // 1. Verifica se já processou (idempotência)
        if (await processedEvents.HasProcessedAsync(evt.Id, nameof(CustomerCreatedHandler), ct))
            return;
        
        // 2. Faz o trabalho real
        var welcomeInvoice = Invoice.CreateWelcomeInvoice(evt.CustomerId, evt.TenantId);
        invoices.Add(welcomeInvoice);
        
        // 3. Marca como processado NA MESMA TRANSAÇÃO do side effect
        await processedEvents.MarkProcessedAsync(evt.Id, nameof(CustomerCreatedHandler), ct);
        
        await uow.SaveChangesAsync(ct);
    }
}
```

**Sem Outbox + idempotência**: você PERDE eventos em crashes ou deploys.
**Com Outbox + idempotência**: garantia at-least-once. Reprocessamento é seguro.

#### InProcessIntegrationEventBus (MVP)

- Implementação in-memory para o monolito
- Localizado em `BuildingBlocks.Infrastructure.Events`
- Lê do Outbox via Hangfire, despacha aos handlers registrados
- **Mesma interface** que `ServiceBusIntegrationEventBus` (futura)
- Troca de implementação quando extrair: zero mudança no código de negócio

### 7.18 Background Jobs (Hangfire OSS)

- Storage: SQL Server (mesmo banco)
- Filas separadas por módulo: `core-jobs`, `customers-jobs`, `payments-jobs`, etc.
- Dashboard em `/jobs` (protegido por auth admin)
- Recurring jobs:
  - Geração diária de invoices (Payments)
  - Envio de notification schedules (Payments)
  - Outbox processor (cada módulo)
  - Cleanup de IdempotencyKeys (Core)
  - Cleanup de ProcessedIntegrationEvents antigos (cada módulo)

---

## 8. Frontend Architecture

### 8.1 Single Blazor WASM App com Áreas

```
src/Hosts/Luga.Client.Host/
├── Program.cs
├── App.razor                              # Router com AdditionalAssemblies
├── wwwroot/
│   ├── index.html
│   ├── manifest.json                      # PWA
│   └── service-worker.js
└── ...
```

**App.razor descobre rotas de todos os módulos**:

```razor
<Router AppAssembly="@typeof(Program).Assembly"
        AdditionalAssemblies="@(new[] {
            typeof(MarketingClientModule).Assembly,
            typeof(CoreClientModule).Assembly,
            typeof(CustomersClientModule).Assembly,
            typeof(PaymentsClientModule).Assembly,
            typeof(PersonalizationClientModule).Assembly
        })">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <LayoutView Layout="@typeof(MainLayout)">
            <NotFoundPage />
        </LayoutView>
    </NotFound>
</Router>
```

Páginas `@page "/customers"` em `Luga.Modules.Customers.Client/Pages/CustomersList.razor` são descobertas automaticamente.

### 8.2 Áreas lógicas

Cada módulo pode ter páginas em áreas diferentes baseado em URL pattern:

```
/                              → Marketing (público)
/pricing                       → Marketing (público)
/login                         → Core (auth)
/signup                        → Core (auth)

/dashboard                     → Tenant logado
/customers                     → Tenant logado
/customers/{id}                → Tenant logado
/payments/invoices             → Tenant logado
/settings                      → Tenant logado

/admin/tenants                 → Luga admin
/admin/catalog                 → Luga admin
/admin/users                   → Luga admin
/admin/breadcrumbs             → Luga admin (Personalization, V2+)
```

`MainLayout` detecta a área pela rota e mostra navegação apropriada (público sem sidebar, tenant com sidebar do tenant, admin com sidebar do admin).

### 8.3 Lazy Loading por Módulo

`Luga.Client.Host.csproj`:
```xml
<ItemGroup>
  <BlazorWebAssemblyLazyLoad Include="Luga.Modules.Customers.Client.dll" />
  <BlazorWebAssemblyLazyLoad Include="Luga.Modules.Payments.Client.dll" />
  <BlazorWebAssemblyLazyLoad Include="Luga.Modules.Personalization.Client.dll" />
</ItemGroup>
```

Módulo Marketing carrega imediato (landing pública). Demais lazy-loaded por rota.

### 8.4 Subdomínios

- `luga.com` → institucional (redirect para login se acessar app)
- `app.luga.com` → Blazor WASM (todas as áreas: marketing visualização, dashboard tenant, admin Luga)
- `api.luga.com` → API .NET (Container Apps)

### 8.5 PWA desde Fase 0

- Manifest com ícones, instalável em mobile
- Service worker básico (offline fallback)

### 8.6 Mobile (V1.1+)

`Luga.Mobile.Host` (MAUI Blazor Hybrid) referencia os **mesmos** `Luga.Modules.X.Client` projects do web. **~95% de reuso**.

Diferenças entre `Client.Host` (web) e `Mobile.Host` (mobile):
- `HttpClient.BaseAddress` aponta para API remota em mobile (em web é mesma origem)
- MSAL é diferente (Microsoft.Identity.Client para mobile)
- Layout pode ser diferente (MobileLayout em vez de MainLayout)
- APIs nativas (câmera, biometria, push) disponíveis via MAUI services

---

## 9. Module Discovery & Manifest System

### 9.1 IModuleManifest (em BuildingBlocks.Client)

Cada módulo declara seus recursos via manifest:

```csharp
public interface IModuleManifest
{
    string ModuleCode { get; }
    string DisplayNameKey { get; }              // chave i18n
    string IconName { get; }
    int Order { get; }
    string? RequiredSubscriptionModule { get; } // null = sempre disponível
    
    IReadOnlyList<MenuItem> MenuItems { get; }
    IReadOnlyList<DashboardWidget> Widgets { get; }
    IReadOnlyList<BreadcrumbRoute> BreadcrumbRoutes { get; }
    
    // Reservado para V2+: não preencher no MVP
    IReadOnlyList<EmbeddableComponent> EmbeddableComponents { get; }
}

public sealed record MenuItem(
    string LabelKey,                            // chave i18n, não string literal
    string Route,
    string IconName,
    int Order,
    string? RequiredPermission = null,
    IReadOnlyList<MenuItem>? Children = null);

public sealed record DashboardWidget(
    string Id,
    string TitleKey,
    Type ComponentType,
    int Order,
    DashboardWidgetSize Size,
    string? RequiredPermission = null);

public sealed record BreadcrumbRoute(
    string RoutePattern,
    IReadOnlyList<BreadcrumbSegment> Segments,
    bool IsEnabled = true);

public sealed record BreadcrumbSegment(
    string LabelKey,
    string? Href = null,
    string? IconName = null,
    BreadcrumbSegmentSource Source = BreadcrumbSegmentSource.Static);
```

### 9.2 Registro via DI

Cada módulo `.Client` tem extension method:

```csharp
public static class CustomersClientModule
{
    public static IServiceCollection AddCustomersClientModule(this IServiceCollection services)
    {
        services.AddRefitClient<ICustomersApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("/"));
        
        services.AddSingleton<IModuleManifest, CustomersManifest>();
        
        return services;
    }
}
```

### 9.3 Menu dinâmico via discovery

```razor
@inject IEnumerable<IModuleManifest> Manifests
@inject ITenantContext Tenant
@inject IPermissionService Permissions
@inject IStringLocalizerFactory LocalizerFactory

<nav>
    @foreach (var manifest in Manifests
        .Where(m => m.RequiredSubscriptionModule is null 
                 || Tenant.HasModuleActive(m.RequiredSubscriptionModule))
        .OrderBy(m => m.Order))
    {
        <MenuSection Manifest="@manifest" />
    }
</nav>
```

### 9.4 LugaPageBase

Em `BuildingBlocks.Client.Components`:

```csharp
public abstract class LugaPageBase : ComponentBase
{
    [CascadingParameter] protected TenantContext Tenant { get; set; } = null!;
    [CascadingParameter] protected CurrentUser User { get; set; } = null!;
    [CascadingParameter] protected IPermissionService Permissions { get; set; } = null!;
    
    protected bool HasPermission(string permission) => Permissions.HasPermission(permission);
    protected bool HasModule(string moduleCode) => Tenant.HasModuleActive(moduleCode);
}
```

Páginas herdam:

```razor
@page "/customers"
@inherits LugaPageBase
@inject ICustomersApi Api
@inject IStringLocalizer<CustomersList> L

<PageBreadcrumb />
<h1>@L["pageTitle"]</h1>
@* ... *@
```

### 9.5 IModuleInitializer (server-side)

Em `BuildingBlocks.Server`:

```csharp
public interface IModuleInitializer
{
    string ModuleCode { get; }
    int Version { get; }
    Task InitializeAsync(InitializationContext context, CancellationToken ct);
}
```

Cada módulo tem seu `XModuleInitializer` para seeds. `ModuleInitializerRunner` orquestra execução e versionamento.

---

## 10. Breadcrumbs (MVP: código apenas; V2+: UI override e URL override)

### 10.1 No MVP — apenas declaração em código

`IModuleManifest.BreadcrumbRoutes` declara breadcrumbs por rota usando chaves i18n.

Componente `<PageBreadcrumb />` em `BuildingBlocks.Client.Components` resolve baseado em:
- Rota atual
- Parâmetros de rota
- `DynamicLeaf` parameter (para nomes dinâmicos como "Cliente João Silva")

```razor
@page "/customers/{id:guid}"
@inherits LugaPageBase

<PageBreadcrumb DynamicLeaf="@customer?.Name" />
```

### 10.2 ANOTADO PARA V2+ (não implementar no MVP)

**Sistema de overrides em cascata** (prioridade maior para menor):

1. **URL parameter override** — `?bcLeaf=texto` ou `?bc=label|href,label|href,label`
2. **UI configuration override** — persistido em `personalization.breadcrumb_overrides`
   - Escopo: Tenant > Plan > Global
   - Permite desativar, customizar labels, mudar ícones
   - Cache invalidado quando admin salva
3. **Code default** — do `IModuleManifest`

UI admin no módulo `Personalization` para configurar overrides.

**Quando implementar**: V2+ se houver demanda real. Não no MVP.

---

## 11. Internationalization (i18n) — Full i18n-ready desde MVP

### 11.1 Decisão estratégica

**Opção 1 — Full i18n-ready no MVP**: toda string passa por `IStringLocalizer<T>`. Strings literais em Razor são proibidas (validado por ArchUnitNET).

### 11.2 Stack

- Biblioteca: **My.Extensions.Localization.Json** (JSON em vez de .resx)
- API: `IStringLocalizer<T>` padrão Microsoft (compatível)
- Arquivos JSON em `Resources/` paralelos aos componentes Razor

### 11.3 Idiomas planejados

- **pt-BR** (default, único ativo no MVP)
- **en-US** (preparado, traduzido em V2+ conforme demanda)
- **es-ES** (preparado, traduzido em V2+ conforme demanda)

### 11.4 Estrutura de arquivos

```
Luga.Modules.Customers.Client/
├── Resources/
│   ├── CustomersList.pt-BR.json
│   ├── CustomersList.en-US.json
│   ├── CustomersList.es-ES.json
│   ├── CustomerDetail.pt-BR.json
│   ├── CustomersManifest.pt-BR.json
│   └── ...
├── Pages/
│   ├── CustomersList.razor
│   └── CustomerDetail.razor
└── CustomersManifest.cs
```

Strings compartilhadas em `BuildingBlocks.Client/Resources/SharedStrings.{culture}.json`.

### 11.5 Manifests usam LabelKey

```csharp
public sealed class CustomersManifest : IModuleManifest
{
    public string DisplayNameKey => "manifest.customers.displayName";
    
    public IReadOnlyList<MenuItem> MenuItems =>
    [
        new(LabelKey: "manifest.customers.menu.list", "/customers", "list", 10),
        new(LabelKey: "manifest.customers.menu.new", "/customers/new", "user-plus", 20),
    ];
    
    public IReadOnlyList<BreadcrumbRoute> BreadcrumbRoutes =>
    [
        new("/customers",
            [
                new(LabelKey: "common.home", Href: "/", IconName: "home"),
                new(LabelKey: "manifest.customers.breadcrumb.list")
            ])
    ];
}
```

### 11.6 Uso em componentes

```razor
@inject IStringLocalizer<CustomersList> L

<h1>@L["pageTitle"]</h1>
<button>@L["createButton"]</button>
<MudText>@L["confirmDelete", customer.Name]</MudText>
```

### 11.7 Detecção de idioma (cascata)

1. **Preferência do user** (salvo em `TenantUser.PreferredCulture`)
2. **Default do tenant** (`Tenant.DefaultCulture`)
3. **Browser** (`Accept-Language` header)
4. **Fallback configurado via UI no super admin** (Luga admin define o fallback global)

### 11.8 Formatação cultural

- **UI culture** (textos): segue preferência do user
- **Money culture** (R$): sempre `pt-BR` para tenants brasileiros (independente do idioma da UI)
- **Date culture**: segue preferência do user

```csharp
@invoice.Amount.ToString("C", Tenant.MoneyCulture)    // sempre R$ para BR
@invoice.DueDate.ToString("d", CultureInfo.CurrentCulture)  // formato do user
```

### 11.9 ArchUnitNET test para detectar strings hardcoded

Test em `Luga.Tests.Architecture` que escaneia arquivos `.razor` e detecta strings literais em conteúdo de UI (não em código). Build falha se encontrar.

### 11.10 Conteúdo configurável multi-idioma (V3+)

Templates de notificação, custom field labels, nomes de planos com tradução por tenant: **V3+** quando houver demanda. Adicionado via `NotificationTemplateTranslation`, `CustomFieldLabel`, etc.

---

## 12. Code Conventions

### 12.1 Naming

```
Commands:           Create[Entity]Command, Update[Entity]Command
Queries:            Get[Entity]Query, List[Entities]Query
Handlers:           [Command/Query]Handler
DTOs:               [Entity]Dto, [Entity]SummaryDto, [Entity]DetailDto
Validators:         [Command]Validator
Domain Events:      [Entity][PastTense]DomainEvent
Integration Events: [Entity][PastTense]IntegrationEventV{N}  ← sempre versionado
Controllers:        [Entities]Controller
Repositories:       I[Entity]Repository / [Entity]Repository
Services:           [Domain]Service
Errors:             [Module]Errors (estática com Error fields)
Manifests:          [Module]Manifest : IModuleManifest
Initializers:       [Module]ModuleInitializer : IModuleInitializer
```

### 12.2 Patterns que SÃO usados

- Primary constructors em handlers, services, controllers
- Records para DTOs, Commands, Queries, VOs simples, Events
- `sealed` por padrão (a menos que herança seja intencional)
- File-scoped namespaces sempre
- Nullable reference types ON
- Async/await SEMPRE com CancellationToken
- TimeProvider para tempo testável
- Result<T> para erros de negócio
- Pattern matching em switches
- Collection expressions (`[]`)
- `IStringLocalizer<T>` para toda string visível ao user

### 12.3 Patterns que NÃO são usados

- AutoMapper (usamos Mapperly)
- Exceptions para fluxo de negócio (usamos Result<T>)
- Service Locator
- Magic strings (usar constantes)
- Static singletons mutáveis
- `dynamic`
- Construtores com >5 parâmetros (sinal de violação SRP)
- Métodos com >50 linhas
- Strings literais em conteúdo de Razor (sempre IStringLocalizer)

### 12.4 Configurações globais

`Directory.Build.props` aplica em todos os `.csproj`:
```xml
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
<LangVersion>latest</LangVersion>
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
```

`Directory.Packages.props` para Central Package Management.
`.editorconfig` configurado para Visual Studio com StyleCop rules.

---

## 13. Pricing System (módulos + tiers + bundles)

Modelo 3D no schema `core`:

```
core.subscription_plans (id, name, billing_cycle Mensal|Anual, is_bundle, price)
core.plan_items (plan_id, module_code, tier_code)
core.module_tiers (module_code, tier_code, name, limits_jsonb)
core.tenant_subscriptions (tenant_id, plan_id, started_at, status, ...)
```

Cada módulo declara seus tiers no startup via `IModuleInitializer`. Bundles compõem tiers com preço próprio. UI de gestão de catálogo no admin (Luga gerencia, tenants escolhem).

---

## 14. Notification & Automation System

### 14.1 Notification Policies (cobrança) — MVP

- Tenant define `NotificationPolicy` com regras
- Sistema MATERIALIZA `NotificationSchedule[]` quando Invoice é criada
- Job hourly processa:
  - Auto → dispara via canal (email via Mailtrap)
  - Manual → cria card no painel
- WhatsApp Manual: deep link `wa.me/?text=...`
- Templates Fluid em modo seguro (sem execução de código)

### 14.2 Actions Engine (V2+)

Motor genérico estilo Zapier interno. Não no MVP.

### 14.3 Webhooks de saída

Quando implementados em V2+: validar IPs internos não acessíveis, timeout, anti-DNS-rebinding, rate limit, allowlist de schemas.

---

## 15. Personalization Module

Módulo dedicado para customizações de UI. **MVP** apenas:
- Gestão de menu (override de labels, ordem, planos habilitados)
- RBAC (roles e permissions)

**V2+ anotado**:
- Breadcrumb overrides (UI e URL)
- Page visibility overrides
- Theme customization

**V3+ se demanda**:
- Page composition completa (custom router via NotFound fallback)
- EmbeddableComponents por módulo
- Editor visual de páginas

---

## 16. Security & Compliance

- **Webhooks de gateway**: validar assinatura HMAC sempre
- **Idempotência**: endpoint de mutação aceita header `Idempotency-Key` (TTL 24h, tabela `core.idempotency_keys`)
- **LGPD**: dados pessoais marcados com atributo `[PersonalData]`
- **Audit log**: tabela `core.audit_entries` append-only para ações sensíveis
- **Secrets**: Azure Key Vault em prod, user-secrets em dev
- **JWT**: validado pelo middleware Microsoft.Identity.Web
- **Rate limiting**: ASP.NET Core nativo, por tenant + por IP
- **Chaves Pix do tenant**: criptografadas (AES-256-GCM, chave no Key Vault)
- **Templates Fluid**: modo seguro (sem execução de código)
- **AuthPropagationHandler** preparado para repassar JWT em chamadas cross-service

---

## 17. Testing Strategy

| Tipo | Tecnologia | Cobertura |
|---|---|---|
| Unit (Domain) | xUnit + FluentAssertions | invariantes, factories, comportamento |
| Unit (Application) | xUnit + **Moq** + FluentAssertions | handlers com mocks |
| Integration | Testcontainers (SQL Server) | API completa via WebApplicationFactory |
| Architecture | ArchUnitNET | regras cross-module + i18n keys |
| Component (Blazor) | bUnit | componentes isoladamente |
| E2E (front) | Playwright | fluxos críticos (signup, criar customer, etc) |

**Test naming**: `[Method]_[Scenario]_[ExpectedResult]`
**Cobertura mínima**: 80% em Domain e Application
**Bug fix**: nasce com teste que reproduz o bug

---

## 18. CI/CD

GitHub Actions com OIDC (sem secrets):

```
.github/workflows/
├── ci-backend.yml           # PRs: build + unit + integration + arch tests + format
├── ci-frontend.yml          # PRs: bUnit + Playwright + build
├── ci-infra.yml             # PRs: bicep what-if
├── deploy-migrations.yml    # Aplica migrations antes de deploys de app
├── deploy-staging.yml       # push em main: deploy auto staging
└── deploy-production.yml    # workflow_dispatch: deploy manual prod
```

Migrations são **sempre aplicadas em job separado antes do deploy da app**. Scripts SQL idempotent gerados e arquivados como artifact.

PRs em código de pagamento exigem 2 reviewers.

---

## 19. Dev Local: .NET Aspire

`Luga.AppHost` orquestra dev local:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sql-server")
    .AddDatabase("luga-core");

var api = builder.AddProject<Projects.Luga_Server_Host>("api")
    .WithReference(sqlServer);

var web = builder.AddProject<Projects.Luga_Client_Host>("web")
    .WithReference(api);

builder.Build().Run();
```

Comando: `dotnet run --project src/AppHost/Luga.AppHost`

**Dashboard em `http://localhost:18888`** mostra logs agregados, métricas, traces, health checks.

---

## 20. Glossary

| Termo | Definição |
|---|---|
| **Tenant** | empresa cliente do Luga (academia, escola, clínica) |
| **TenantUser** | pessoa que acessa o painel em nome do tenant |
| **Customer** | cliente final do tenant (aluno, paciente, sócio) |
| **Plan** (do Luga) | pacote de módulos+tiers que o Tenant assina do Luga |
| **Module Tier** | nível de cada módulo (Free, Pro, Business) com limites |
| **Bundle** | combinação de tiers de múltiplos módulos com preço próprio |
| **Subscription** (do Tenant) | vínculo Tenant ↔ Plan do Luga |
| **TenantPlan** | plano que o tenant oferece aos seus customers |
| **Invoice** | fatura gerada para uma mensalidade |
| **Charge** | tentativa de cobrança |
| **GatewayAccount** | conta do tenant em gateway (Manual ou Asaas) |
| **Manual Mode** | tenant cadastra Pix, marca pagamentos manualmente |
| **Gateway Mode** | cobrança automática via Asaas (subconta) |
| **Dunning** | processo de retentativa após falha |
| **NotificationPolicy** | regras de quando/como notificar customer sobre cobrança |
| **Action** (V2+) | automação configurável trigger→steps |
| **IModuleManifest** | declaração de recursos de um módulo (menu, widgets, breadcrumbs) |
| **IModuleInitializer** | seed inicial de dados por módulo, versionado |

---

## 21. Rules for Claude

### Decisões e mudanças
- **NUNCA** introduzir nova lib NuGet/npm sem perguntar primeiro
- **NUNCA** rodar `database update` sem confirmar — apenas gerar migration
- Migrations geradas devem ser inspecionadas antes de aplicar
- Sempre rodar `dotnet build` e `dotnet test` antes de afirmar que algo funciona

### Arquitetura modular
- **NUNCA acoplar módulos via referência direta** — sempre Contracts ou Events
- **NUNCA fazer JOIN, Include, ou FK física entre tabelas de módulos diferentes**
- **NUNCA importar entidade interna de outro módulo**
- **TODO Integration Event** deve estar em `[Module].Contracts` e versionado (sufixo V1, V2)
- **TODA mudança breaking** em Integration Event exige nova versão (V2 coexiste com V1)
- **TODA emissão de Integration Event** passa pelo Outbox (mesma transação)
- **TODO handler de Integration Event** deve ser idempotente (verificar via processed_integration_events)
- **APIs cross-module SEMPRE têm versão batch** (`GetByIdsAsync(ids)`) desde dia 1

### Entidades e EF
- Ao adicionar entidade nova, decidir qual base usar (`EntityBase`, `AuditableEntity`, `FullAuditableEntity`, `TenantEntity`)
- Ao escrever query EF, lembrar que filtros globais são aplicados por padrão
- `IgnoreQueryFilters()` exige justificativa em PR
- Migrations sempre **backwards-compatible** (não quebram app antigo durante deploy)
- Remoções de colunas/tabelas em múltiplos deploys (deprecar → migrar uso → drop)
- Cada DbContext de módulo tem migration history em schema próprio

### Tempo e localização
- **TimeProvider sempre**, NUNCA `DateTime.Now`/`DateTime.UtcNow` direto
- **IStringLocalizer<T> sempre**, NUNCA string literal em Razor
- Manifests usam `LabelKey`, nunca strings literais

### Testes
- Testes de arquitetura (ArchUnitNET) devem cobrir TODA nova regra
- Bug fix nasce com teste que reproduz o bug
- Mocking via Moq (não NSubstitute)

### Pagamentos e segurança
- Ao mexer com gateway de pagamento, sempre considerar idempotência
- Webhooks validar HMAC sempre
- Dados sensíveis (Pix keys, credenciais) criptografados via Key Vault
- LGPD: marcar dados pessoais com `[PersonalData]`

### Module Discovery
- Antes de adicionar entidade ao `core`, questionar se ela é REALMENTE compartilhada
- Cada módulo declara seus recursos via `IModuleManifest`
- Cada módulo tem `IModuleInitializer` para seeds versionados
- Adicionar item ao menu = adicionar entrada no manifest (não tocar em código do host)

### Outras
- Toda nova feature de notificação ao customer passa pelo módulo Automation (V2+)
- NUNCA enviar email/whatsapp direto de outros módulos — sempre via IStepExecutor (V2+) ou NotificationService (MVP)
- Webhooks de SAÍDA passam SEMPRE pelo HttpWebhookStepExecutor (com defesas)
- Templates de notificação NUNCA executam código (Fluid em modo seguro)
- Preferir mudanças pequenas e PRs focados; não refatorar coisas não pedidas
- Em dúvidas sobre regulação/compliance, **perguntar antes de implementar**

### Working Style for Claude Code (quando rodar agentic coding)
- ALWAYS read PLAN.md before starting any work session
- Reference PLAN.md section/item being worked on in commit messages
- Mark items as `- [x]` in PLAN.md when completed (in same PR)
- Run `dotnet build` e `dotnet test` antes de afirmar pronto
- Run `dotnet format` antes de commit
- Create feature branch from main for each PLAN.md section
- Stop and ask before introducing new NuGet/npm packages
- When in doubt about architectural choice, refer to CLAUDE.md
- Prefer asking 1 clarifying question over 5 wrong assumptions
