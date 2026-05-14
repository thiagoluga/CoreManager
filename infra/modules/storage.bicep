// Azure Blob Storage — single account with the `documents` container used by
// the future Documents module (V2) and by Customers' uploads.

@description('Short prefix for the resource names.')
param namePrefix string

@description('Environment name.')
param environmentName string

@description('Resource location.')
param location string

@description('Tags applied to every resource.')
param tags object

// Storage account names are globally unique and 3-24 chars, lower-case alphanumeric.
var storageAccountName = take(toLower(replace('${namePrefix}${environmentName}sa${uniqueString(resourceGroup().id)}', '-', '')), 24)

resource storage 'Microsoft.Storage/storageAccounts@2024-01-01' = {
  name: storageAccountName
  location: location
  tags: tags
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: false
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

resource blobServices 'Microsoft.Storage/storageAccounts/blobServices@2024-01-01' = {
  parent: storage
  name: 'default'
  properties: {
    deleteRetentionPolicy: {
      enabled: true
      days: 7
    }
  }
}

resource documentsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2024-01-01' = {
  parent: blobServices
  name: 'documents'
  properties: {
    publicAccess: 'None'
  }
}

output accountName string = storage.name
output accountId string = storage.id
output primaryBlobEndpoint string = storage.properties.primaryEndpoints.blob
