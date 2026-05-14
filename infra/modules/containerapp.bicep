// Container Apps Environment + the Luga API container app.
// scale-to-zero on staging, min_replicas=1 on production (mitigates cold start
// risk from CLAUDE.md §7 — risk #6).

@description('Short prefix for the resource names.')
param namePrefix string

@description('Environment name.')
param environmentName string

@description('Resource location.')
param location string

@description('Log Analytics workspace id (for the Container Apps environment logs).')
param logAnalyticsWorkspaceId string

@description('Application Insights connection string (env var APPLICATIONINSIGHTS_CONNECTION_STRING).')
@secure()
param appInsightsConnectionString string

@description('Key Vault URI (env var so the app can resolve secrets via Managed Identity).')
param keyVaultUri string

@description('Connection string the API uses for the primary DbContext.')
@secure()
param sqlConnectionString string

@description('Container image to deploy. Defaults to the GHCR public registry.')
param containerImage string = 'ghcr.io/thiagoluga/coremanager/api:latest'

@description('Min replicas. 0 = scale-to-zero in staging, 1 in production.')
param minReplicas int = (environmentName == 'production') ? 1 : 0

@description('Max replicas.')
param maxReplicas int = (environmentName == 'production') ? 5 : 2

@description('Tags applied to every resource.')
param tags object

var environmentName2 = '${namePrefix}-${environmentName}-cae'
var containerAppName = '${namePrefix}-${environmentName}-api'

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' existing = {
  name: split(logAnalyticsWorkspaceId, '/')[8]
}

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: '${containerAppName}-identity'
  location: location
  tags: tags
}

resource environment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: environmentName2
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
    zoneRedundant: false
  }
}

resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: containerAppName
  location: location
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentity.id}': {}
    }
  }
  properties: {
    environmentId: environment.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
        transport: 'auto'
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
      activeRevisionsMode: 'Single'
      secrets: [
        {
          name: 'app-insights-connection-string'
          value: appInsightsConnectionString
        }
        {
          name: 'connection-strings-default'
          value: sqlConnectionString
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'api'
          image: containerImage
          resources: {
            cpu: json('0.5')
            memory: '1.0Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: (environmentName == 'production') ? 'Production' : 'Staging'
            }
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://+:8080'
            }
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              secretRef: 'app-insights-connection-string'
            }
            {
              name: 'KeyVault__Uri'
              value: keyVaultUri
            }
            {
              name: 'AZURE_CLIENT_ID'
              value: managedIdentity.properties.clientId
            }
            {
              name: 'ConnectionStrings__Default'
              secretRef: 'connection-strings-default'
            }
            {
              name: 'ConnectionStrings__Hangfire'
              secretRef: 'connection-strings-default'
            }
          ]
          probes: [
            {
              type: 'Liveness'
              httpGet: {
                path: '/health/live'
                port: 8080
              }
              initialDelaySeconds: 10
              periodSeconds: 30
            }
            {
              type: 'Readiness'
              httpGet: {
                path: '/health/ready'
                port: 8080
              }
              initialDelaySeconds: 15
              periodSeconds: 30
            }
          ]
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
}

output fqdn string = containerApp.properties.configuration.ingress.fqdn
output principalId string = managedIdentity.properties.principalId
output identityClientId string = managedIdentity.properties.clientId
output containerAppName string = containerApp.name
