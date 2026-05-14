# Luga CoreManager — Infrastructure (Bicep)

Azure resources for the Luga CoreManager API. Deployed at subscription scope
to keep the resource group lifecycle declarative.

## Layout

```
infra/
├── main.bicep                     # subscription-scope orchestrator
├── modules/
│   ├── appinsights.bicep          # Log Analytics + Application Insights
│   ├── containerapp.bicep         # Managed environment + API container app + user-assigned identity
│   ├── keyvault.bicep             # RBAC-mode Key Vault
│   ├── sqldatabase.bicep          # Azure SQL Database — Serverless tier
│   └── storage.bicep              # Blob Storage + `documents` container
└── parameters/
    ├── staging.bicepparam         # Scale-to-zero, aggressive SQL auto-pause
    └── production.bicepparam      # min_replicas=1, auto-pause disabled
```

## Prerequisites

- Azure CLI 2.60+ with Bicep extension (`az bicep install`)
- A subscription where you can create resource groups
- The Microsoft Entra External ID tenant id (replace `entraTenantId` in
  `parameters/<env>.bicepparam` before deploying)

## Deploy (manual)

```bash
# Login + select subscription
az login
az account set --subscription "<subscription-id>"

# What-if preview (CI uses the same command).
az deployment sub what-if \
  --location brazilsouth \
  --template-file infra/main.bicep \
  --parameters infra/parameters/staging.bicepparam \
  --parameters sqlAdministratorLogin=$SQL_ADMIN_LOGIN \
               sqlAdministratorPassword=$SQL_ADMIN_PASSWORD

# Actual deploy
az deployment sub create \
  --location brazilsouth \
  --template-file infra/main.bicep \
  --parameters infra/parameters/staging.bicepparam \
  --parameters sqlAdministratorLogin=$SQL_ADMIN_LOGIN \
               sqlAdministratorPassword=$SQL_ADMIN_PASSWORD
```

## After the first deploy

1. **Grant Managed Identity access to Key Vault** (RBAC):

   ```bash
   az role assignment create \
     --assignee <containerapp-principal-id> \
     --role "Key Vault Secrets User" \
     --scope $(az keyvault show -n <kv-name> --query id -o tsv)
   ```

2. **Grant Managed Identity access to Azure SQL** (Entra-only auth):

   ```sql
   -- Run as Entra admin on the SQL server
   CREATE USER [luga-staging-api-identity] FROM EXTERNAL PROVIDER;
   ALTER ROLE db_owner ADD MEMBER [luga-staging-api-identity];
   ```

3. **Replace placeholder secrets** in Key Vault (Asaas, Mailtrap, etc. — see
   `docs/runbooks/` once written in Phase 1).

## Notes

- The `containerImage` parameter defaults to GHCR; the
  `deploy-staging.yml` workflow overrides it with the SHA-tagged image after
  build.
- Auto-pause delay defaults to 60 min in staging, disabled in production
  (CLAUDE.md §7 — risk #7).
- Resources are tagged with `product=luga-coremanager`, `environment=<env>`,
  `managedBy=bicep` to keep Cost Management dashboards clean.
