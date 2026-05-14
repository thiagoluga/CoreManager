#requires -Version 7.0

<#
.SYNOPSIS
    Scaffolds the four projects for a new Luga module (Server, Client, Shared,
    Contracts) following the convention in CLAUDE.md §6.

.DESCRIPTION
    Creates the standard folder layout, adds the projects to the solution,
    wires the canonical references, and seeds the basic files (manifest stub,
    composition root, _Imports.razor, AssemblyMarker, etc.).

.PARAMETER Name
    Module short code in PascalCase (Customers, Payments, ...). Plural form.

.EXAMPLE
    ./scripts/Add-Module.ps1 -Name Customers
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Name
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$moduleRoot = Join-Path $repoRoot "src/Modules/$Name"
$sln = Join-Path $repoRoot 'src/Luga.CoreManager.slnx'

if (Test-Path $moduleRoot) {
    throw "Module folder already exists at $moduleRoot."
}

Write-Host "Scaffolding module '$Name' at $moduleRoot..." -ForegroundColor Cyan

# 1. Server (class library) — Domain + Application + Infrastructure + Api
$server = Join-Path $moduleRoot "Luga.Modules.$Name.Server"
dotnet new classlib -n "Luga.Modules.$Name.Server" -o $server --framework net10.0 | Out-Null

# 2. Client (Razor class library)
$client = Join-Path $moduleRoot "Luga.Modules.$Name.Client"
dotnet new razorclasslib -n "Luga.Modules.$Name.Client" -o $client --framework net10.0 --support-pages-and-views false | Out-Null

# 3. Shared (HTTP DTOs + Refit interface)
$shared = Join-Path $moduleRoot "Luga.Modules.$Name.Shared"
dotnet new classlib -n "Luga.Modules.$Name.Shared" -o $shared --framework net10.0 | Out-Null

# 4. Contracts (cross-module surface)
$contracts = Join-Path $moduleRoot "Luga.Modules.$Name.Contracts"
dotnet new classlib -n "Luga.Modules.$Name.Contracts" -o $contracts --framework net10.0 | Out-Null

# Clean up the template-generated Class1.cs stubs.
Get-ChildItem -Path $moduleRoot -Recurse -Filter 'Class1.cs' | Remove-Item -Force

Write-Host "Adding projects to the solution..." -ForegroundColor Cyan
dotnet sln $sln add `
    "$server/Luga.Modules.$Name.Server.csproj" `
    "$client/Luga.Modules.$Name.Client.csproj" `
    "$shared/Luga.Modules.$Name.Shared.csproj" `
    "$contracts/Luga.Modules.$Name.Contracts.csproj" | Out-Null

Write-Host "Done." -ForegroundColor Green
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Wire the ProjectReferences (Server → Contracts/Shared, Client → Shared)."
Write-Host "  2. Register the module in Server.Host (AddXServerModule, ApplicationPart, MediatR assemblies)."
Write-Host "  3. Register in Client.Host (AddXClientModule + App.razor AdditionalAssemblies)."
Write-Host "  4. Add the module to PLAN.md and CLAUDE.md §2.1 if it is not already listed."
