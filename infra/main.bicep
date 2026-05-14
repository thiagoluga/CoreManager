// Luga CoreManager — top-level Azure resource composition (CLAUDE.md §5.12).
// Deploy with:
//   az deployment sub create -l brazilsouth -f infra/main.bicep \
//     -p infra/parameters/staging.bicepparam

targetScope = 'subscription'

@description('Environment name (staging, production).')
@allowed([
  'staging'
  'production'
])
param environmentName string

@description('Primary Azure region (Brazil South unless overridden).')
param location string = 'brazilsouth'

@description('Short prefix used to name every resource group / resource.')
@minLength(2)
@maxLength(10)
param namePrefix string = 'luga'

@description('Tenant id of the Microsoft Entra External ID instance used by the API.')
param entraTenantId string

@description('SQL server administrator login. Stored only in the Bicep state — actual auth is Managed Identity.')
@secure()
param sqlAdministratorLogin string

@description('SQL server administrator password. Stored only in the Bicep state — actual auth is Managed Identity.')
@secure()
param sqlAdministratorPassword string

@description('Tags applied to every resource for cost tracking.')
param tags object = {
  product: 'luga-coremanager'
  environment: environmentName
  managedBy: 'bicep'
}

var resourceGroupName = '${namePrefix}-${environmentName}-rg'

resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: resourceGroupName
  location: location
  tags: tags
}

module appInsights 'modules/appinsights.bicep' = {
  scope: rg
  name: 'appInsights'
  params: {
    namePrefix: namePrefix
    environmentName: environmentName
    location: location
    tags: tags
  }
}

module sql 'modules/sqldatabase.bicep' = {
  scope: rg
  name: 'sql'
  params: {
    namePrefix: namePrefix
    environmentName: environmentName
    location: location
    administratorLogin: sqlAdministratorLogin
    administratorPassword: sqlAdministratorPassword
    tags: tags
  }
}

module keyVault 'modules/keyvault.bicep' = {
  scope: rg
  name: 'keyVault'
  params: {
    namePrefix: namePrefix
    environmentName: environmentName
    location: location
    entraTenantId: entraTenantId
    tags: tags
  }
}

module storage 'modules/storage.bicep' = {
  scope: rg
  name: 'storage'
  params: {
    namePrefix: namePrefix
    environmentName: environmentName
    location: location
    tags: tags
  }
}

module containerApp 'modules/containerapp.bicep' = {
  scope: rg
  name: 'containerApp'
  params: {
    namePrefix: namePrefix
    environmentName: environmentName
    location: location
    logAnalyticsWorkspaceId: appInsights.outputs.logAnalyticsWorkspaceId
    appInsightsConnectionString: appInsights.outputs.connectionString
    keyVaultUri: keyVault.outputs.uri
    sqlConnectionString: sql.outputs.connectionString
    tags: tags
  }
}

output resourceGroup string = rg.name
output containerAppFqdn string = containerApp.outputs.fqdn
output keyVaultUri string = keyVault.outputs.uri
output appInsightsConnectionString string = appInsights.outputs.connectionString
