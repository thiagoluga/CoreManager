#requires -Version 7.0

<#
.SYNOPSIS
    Applies pending EF migrations for a Luga module's DbContext against the
    LocalDB / dev database. NEVER point this at staging/production — use the
    CI/CD `deploy-migrations.yml` workflow there (CLAUDE.md §21).

.PARAMETER Module
    Module short code.

.PARAMETER Connection
    Optional connection string override. Defaults to the value in
    `Luga.Server.Host/appsettings.Development.json`.

.EXAMPLE
    ./scripts/Update-Database.ps1 -Module Customers
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Module,
    [string]$Connection
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot

$projectPath = Join-Path $repoRoot "src/Modules/$Module/Luga.Modules.$Module.Server"
$startupPath = Join-Path $repoRoot 'src/Hosts/Luga.Server.Host'
$context     = "$Module" + 'DbContext'

if (-not (Test-Path $projectPath)) {
    throw "Module project not found at $projectPath."
}

Write-Host "Applying migrations for $context ($projectPath)..." -ForegroundColor Cyan
Write-Warning "This targets your LOCAL database. Cancel now if anything else is selected."
Start-Sleep -Seconds 2

$args = @(
    'ef', 'database', 'update',
    '--project', $projectPath,
    '--startup-project', $startupPath,
    '--context', $context
)
if ($Connection) {
    $args += @('--connection', $Connection)
}

dotnet @args
if ($LASTEXITCODE -ne 0) {
    throw "dotnet ef database update failed (exit $LASTEXITCODE)."
}

Write-Host "Done." -ForegroundColor Green
