// Log Analytics + Application Insights (CLAUDE.md §4.5).
// The Container App streams logs to the workspace; the API ships traces/metrics
// to the App Insights connection string injected as an environment variable.

@description('Short prefix for the resource names.')
param namePrefix string

@description('Environment name (staging/production).')
param environmentName string

@description('Resource location.')
param location string

@description('Tags applied to every resource.')
param tags object

var logAnalyticsName = '${namePrefix}-${environmentName}-law'
var appInsightsName = '${namePrefix}-${environmentName}-ai'

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logAnalyticsName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    workspaceCapping: {
      dailyQuotaGb: 1
    }
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
    IngestionMode: 'LogAnalytics'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

output logAnalyticsWorkspaceId string = logAnalytics.id
output logAnalyticsCustomerId string = logAnalytics.properties.customerId
output appInsightsId string = appInsights.id
output connectionString string = appInsights.properties.ConnectionString
output instrumentationKey string = appInsights.properties.InstrumentationKey
