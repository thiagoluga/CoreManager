#requires -Version 7.0

<#
.SYNOPSIS
    Emits an idempotent SQL script for a module's pending migrations. Used by
    operators reviewing migrations before they hit staging/production
    (CLAUDE.md §7.9 / §7.10).

.PARAMETER Module
    Module short code.

.PARAMETER Output
    Output file path. Defaults to `artifacts/migrations/<module>.sql`.

.EXAMPLE
    ./scripts/Generate-MigrationScript.ps1 -Module Customers
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Module,
    [string]$Output
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot

$projectPath = Join-Path $repoRoot "src/Modules/$Module/Luga.Modules.$Module.Server"
$startupPath = Join-Path $repoRoot 'src/Hosts/Luga.Server.Host'
$context     = "$Module" + 'DbContext'

if (-not (Test-Path $projectPath)) {
    throw "Module project not found at $projectPath."
}

if (-not $Output) {
    $artifactDir = Join-Path $repoRoot 'artifacts/migrations'
    if (-not (Test-Path $artifactDir)) {
        New-Item -ItemType Directory -Path $artifactDir | Out-Null
    }
    $Output = Join-Path $artifactDir "$($Module.ToLowerInvariant()).sql"
}

Write-Host "Emitting idempotent SQL for $context → $Output" -ForegroundColor Cyan

dotnet ef migrations script `
    --idempotent `
    --project $projectPath `
    --startup-project $startupPath `
    --context $context `
    --output $Output

if ($LASTEXITCODE -ne 0) {
    throw "dotnet ef migrations script failed (exit $LASTEXITCODE)."
}

Write-Host "Done. Review $Output before applying." -ForegroundColor Green
