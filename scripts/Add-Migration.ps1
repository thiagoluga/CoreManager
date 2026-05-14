#requires -Version 7.0

<#
.SYNOPSIS
    Generates a new EF Core migration for a Luga module's DbContext.

.DESCRIPTION
    Wraps `dotnet ef migrations add` with the per-module conventions from
    CLAUDE.md §7.9 — separate history table in the module's own schema,
    output dir aligned with the module project layout.

.PARAMETER Module
    Module short code (Core, Customers, Payments, ...). Resolves to
    `src/Modules/<Module>/Luga.Modules.<Module>.Server`.

.PARAMETER Name
    Migration name (PascalCase). Example: `AddCustomFieldsTable`.

.EXAMPLE
    ./scripts/Add-Migration.ps1 -Module Customers -Name AddCustomFieldsTable
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Module,
    [Parameter(Mandatory)][string]$Name
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot

$projectPath = Join-Path $repoRoot "src/Modules/$Module/Luga.Modules.$Module.Server"
$startupPath = Join-Path $repoRoot 'src/Hosts/Luga.Server.Host'
$context     = "$Module" + 'DbContext'
$outputDir   = 'Infrastructure/Persistence/Migrations'

if (-not (Test-Path $projectPath)) {
    throw "Module project not found at $projectPath."
}

Write-Host "Adding migration '$Name' to $context ($projectPath)..." -ForegroundColor Cyan

dotnet ef migrations add $Name `
    --project $projectPath `
    --startup-project $startupPath `
    --context $context `
    --output-dir $outputDir

if ($LASTEXITCODE -ne 0) {
    throw "dotnet ef migrations add failed (exit $LASTEXITCODE)."
}

Write-Host "Done. Review the generated files in $projectPath/$outputDir before applying." -ForegroundColor Green
