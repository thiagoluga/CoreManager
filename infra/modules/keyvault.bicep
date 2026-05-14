// Azure Key Vault — RBAC-mode. The Container App's managed identity gets
// `Key Vault Secrets User` role to read secrets at runtime.

@description('Short prefix for the resource names.')
param namePrefix string

@description('Environment name.')
param environmentName string

@description('Resource location.')
param location string

@description('Microsoft Entra tenant id (subscription home tenant).')
param entraTenantId string

@description('Tags applied to every resource.')
param tags object

// Key Vault names are globally unique and max 24 chars.
var keyVaultName = take('${namePrefix}-${environmentName}-kv-${uniqueString(resourceGroup().id)}', 24)

resource keyVault 'Microsoft.KeyVault/vaults@2024-04-01-preview' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    tenantId: entraTenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    enablePurgeProtection: (environmentName == 'production') ? true : null
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

output id string = keyVault.id
output name string = keyVault.name
output uri string = keyVault.properties.vaultUri
