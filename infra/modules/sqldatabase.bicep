// Azure SQL Database — Serverless tier (auto-pause to keep dev/staging cost low).
// ADR 004. Hangfire and every module's DbContext share the same database;
// isolation between modules happens at the schema level (CLAUDE.md §7.9).

@description('Short prefix for the resource names.')
param namePrefix string

@description('Environment name.')
param environmentName string

@description('Resource location.')
param location string

@secure()
@description('SQL server administrator login.')
param administratorLogin string

@secure()
@description('SQL server administrator password.')
param administratorPassword string

@description('Auto-pause delay in minutes. Use -1 to disable (production).')
param autoPauseDelayMinutes int = (environmentName == 'production') ? -1 : 60

@description('Tags applied to every resource.')
param tags object

var sqlServerName = '${namePrefix}-${environmentName}-sql'
var databaseName = '${namePrefix}-${environmentName}-db'

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  tags: tags
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: 'Disabled'
  }
}

resource allowAzureServices 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  tags: tags
  sku: {
    name: 'GP_S_Gen5'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 1
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    autoPauseDelay: autoPauseDelayMinutes
    minCapacity: json('0.5')
    maxSizeBytes: 34359738368 // 32 GB
    requestedBackupStorageRedundancy: 'Local'
    zoneRedundant: false
  }
}

output serverFqdn string = sqlServer.properties.fullyQualifiedDomainName
output databaseName string = sqlDatabase.name
output connectionString string = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Database=${sqlDatabase.name};Authentication=Active Directory Default;Encrypt=True;'
