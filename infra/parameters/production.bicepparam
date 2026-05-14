using '../main.bicep'

// Production — min_replicas=1, SQL auto-pause disabled (autoPauseDelay = -1).
param environmentName = 'production'
param location = 'brazilsouth'
param namePrefix = 'luga'

// Replace with the real Entra External ID tenant id before deploying.
param entraTenantId = '00000000-0000-0000-0000-000000000000'

// SQL admin credentials — passed via CLI/CI; the values here are placeholders.
param sqlAdministratorLogin = ''
param sqlAdministratorPassword = ''
