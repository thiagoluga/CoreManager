# Luga CoreManager

Plataforma SaaS B2B brasileira **multi-produto** (Suite Integrada) para SMBs — academias, escolas, clubes, prestadores de serviço, profissionais liberais, clínicas pequenas.

Vendemos **gestão**, não processamento. Não movimentamos dinheiro dos customers finais.

```
Luga CoreManager
  │
  └── Tenant (academia/escola/clínica) — paga mensalidade ao Luga
        │
        └── Customer final (aluno/paciente/sócio) — paga ao Tenant
```

> **Status:** Fase 0 (Fundação) em andamento.
> Acompanhe progresso em [PLAN.md](./PLAN.md).

---

## Documentação canônica

Antes de qualquer mudança, ler:

- **[CLAUDE.md](./CLAUDE.md)** — manual permanente do projeto: visão, arquitetura, convenções, regras.
- **[PLAN.md](./PLAN.md)** — roadmap executável: fases, checklists, ADRs, riscos.

---

## Stack

| Camada | Tecnologia |
|---|---|
| Runtime | .NET 10 (LTS) |
| Linguagem | C# 14 |
| API | ASP.NET Core 10 (Controllers) |
| ORM | EF Core 10 + Migrations |
| Banco | Azure SQL Database Serverless |
| Auth | Microsoft Entra External ID (JWT) |
| Mediator | MediatR |
| Validation | FluentValidation |
| Mapping | Mapperly (source-generated) |
| Background Jobs | Hangfire OSS |
| Logging | Serilog |
| Telemetry | OpenTelemetry + Application Insights |
| Frontend Web | Blazor WebAssembly + MudBlazor + Tailwind v4 |
| Frontend Mobile (V1.1+) | .NET MAUI Blazor Hybrid |
| i18n | My.Extensions.Localization.Json + IStringLocalizer |
| Hosting | Azure Container Apps |
| IaC | Bicep |
| CI/CD | GitHub Actions com OIDC |
| Dev local | .NET Aspire 13 |
| Testes | xUnit + Moq + FluentAssertions + Testcontainers + bUnit + Playwright + ArchUnitNET |

---

## Arquitetura

**Modular Monolith → Extractable.** Um único deployment hoje; módulos extraíveis como microsserviços amanhã sem redesign de domínio.

Cada módulo é dono dos seus dados. Comunicação cross-module apenas via **Contracts** (síncrono in-process) ou **Integration Events** versionados (assíncrono, com Outbox + idempotência).

### Módulos MVP (Fase 1)

1. **Core** — Tenants, Users, Auth, Subscriptions ao Luga, Catálogo de Planos
2. **Marketing** — Site institucional, página de planos, signup público
3. **Customers** — Cadastro de end customers com custom fields
4. **Payments** — Mensalidades, planos do tenant, cobrança Manual + Asaas
5. **Personalization** — Overrides de menu, RBAC

### Estrutura do repositório

```
src/
  BuildingBlocks/        # Domain, Application, Infrastructure, IntegrationEvents, Server, Client
  Modules/<Module>/      # .Server, .Client, .Shared, .Contracts (4 projetos por módulo)
  Hosts/                 # Server.Host (API), Client.Host (Blazor WASM), Mobile.Host (MAUI, V1.1+)
  AppHost/               # Aspire orquestrador dev local

tests/
  Architecture/          # ArchUnitNET — valida regras cross-module
  BuildingBlocks/
  Modules/<Module>/

infra/                   # Bicep modules
docs/                    # ADRs, runbooks, architecture docs
scripts/                 # Add-Migration.ps1, Update-Database.ps1, etc.
.github/workflows/       # CI/CD
```

Detalhes completos em [CLAUDE.md §5](./CLAUDE.md).

---

## Dev local

> Pré-requisitos (a serem instalados na Fase 0):
> - .NET 10 SDK
> - Visual Studio 2022 17.12+ (ou Rider 2024.3+)
> - Docker Desktop (para Testcontainers e SQL Server em dev)
> - PowerShell 7+

```powershell
# Restaurar dependências
dotnet restore src/Luga.CoreManager.slnx

# Build
dotnet build src/Luga.CoreManager.slnx

# Subir tudo via Aspire (API + Blazor WASM + SQL Server)
dotnet run --project src/AppHost/Luga.AppHost
```

Dashboard Aspire: <http://localhost:18888>

### Migrations

```powershell
# Adicionar migration de um módulo
./scripts/Add-Migration.ps1 -Module Customers -Name AddCustomFieldsTable

# Aplicar em dev (automático no startup quando ApplyMigrationsOnStartup=true)
./scripts/Update-Database.ps1 -Module Customers
```

---

## Convenções

Resumo dos princípios não-negociáveis (detalhes em [CLAUDE.md §21](./CLAUDE.md)):

- **Modular**: módulos nunca acessam tabelas/entidades alheias. Só Contracts ou Integration Events.
- **Eventos versionados**: `*IntegrationEventV1`, `*IntegrationEventV2`...
- **Outbox obrigatório**: toda emissão de Integration Event passa pelo Outbox na mesma transação.
- **Handlers idempotentes**: verificar via `processed_integration_events`.
- **Batch APIs desde dia 1**: `GetByIdsAsync(ids)` para evitar N+1 quando virar HTTP.
- **TimeProvider sempre**: nunca `DateTime.Now`/`DateTime.UtcNow` direto.
- **IStringLocalizer sempre**: strings literais em Razor são proibidas (validado por ArchUnitNET).
- **Migrations backwards-compatible**: zero downtime em deploys.
- **Validação arquitetural em CI** via ArchUnitNET.

---

## Roadmap

- **Fase 0 — Fundação** (atual): plumbing técnico completo, sem features de negócio. Login funcional + Aspire + CI/CD + staging.
- **Fase 1 — MVP**: 5 módulos funcionais, 1-3 tenants beta em produção.
- **V1.1+**: app mobile (MAUI), customer portal, multi-gateway, dunning sofisticado.
- **V2+**: Documents, Signatures, Actions engine, API pública.

Detalhes e checklists em [PLAN.md](./PLAN.md).

---

## Contribuindo

PRs pequenos e focados, sempre referenciando seção do PLAN.md no commit.

Antes de submeter:

```powershell
dotnet build
dotnet test
dotnet format --verify-no-changes
```

Code review obrigatório. PRs em código de pagamento exigem 2 reviewers.

---

## Licença

Proprietário. Todos os direitos reservados.
