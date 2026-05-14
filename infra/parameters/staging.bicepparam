using '../main.bicep'

// Staging — scales to zero, cheaper SQL auto-pause aggressive.
param environmentName = 'staging'
param location = 'brazilsouth'
param namePrefix = 'luga'

// Replace with the real Entra External ID tenant id before deploying.
param entraTenantId = '00000000-0000-0000-0000-000000000000'

// SQL admin credentials — passed via CLI/CI; the values here are placeholders.
// In CI, use:
//   --parameters sqlAdministratorLogin=$(SQL_ADMIN_LOGIN) sqlAdministratorPassword=$(SQL_ADMIN_PASSWORD)
param sqlAdministratorLogin = ''
param sqlAdministratorPassword = ''
