#!/usr/bin/env pwsh

# WSO2 IS + .NET 9 SSO - Windows Setup (PowerShell)
# GitHub: https://github.com/ThomasHeinThura/WSO2_IS_Dotnet_SSO
# Run from repo root:  .\setup.ps1

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "WSO2 IS + .NET 9 SSO - Windows Setup" -ForegroundColor Cyan
Write-Host "GitHub: ThomasHeinThura/WSO2_IS_Dotnet_SSO" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET 9 is installed
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
  Write-Host "❌ Error: .NET 9 SDK is not installed" -ForegroundColor Red
  Write-Host ""
  Write-Host "Please install .NET 9 SDK from:" -ForegroundColor Yellow
  Write-Host "https://dotnet.microsoft.com/download/dotnet/9.0"
  Write-Host ""
  exit 1
}

Write-Host "✓ .NET Version: $(dotnet --version)" -ForegroundColor Green
Write-Host ""

# Verify solution file exists
$solution = Join-Path (Get-Location) "WSO2_IS_Dotnet_SSO.sln"
if (-not (Test-Path $solution)) {
  Write-Host "❌ Error: WSO2_IS_Dotnet_SSO.sln not found" -ForegroundColor Red
  Write-Host ""
  Write-Host "Please run this script from the repository root directory:" -ForegroundColor Yellow
  Write-Host "  cd WSO2_IS_Dotnet_SSO"
  Write-Host "  .\setup.ps1"
  Write-Host ""
  exit 1
}

Write-Host "✓ Found solution file: WSO2_IS_Dotnet_SSO.sln" -ForegroundColor Green
Write-Host ""

# Restore NuGet packages
Write-Host "📦 Restoring NuGet packages..." -ForegroundColor Yellow
Write-Host ""
dotnet restore

if ($LASTEXITCODE -ne 0) {
  Write-Host "❌ Error: NuGet restore failed" -ForegroundColor Red
  exit 1
}

Write-Host ""
Write-Host "✓ Packages restored successfully" -ForegroundColor Green
Write-Host ""

# Ensure .env exists
$apiPath = "src\DistributionManagement.API"
$envExample = Join-Path $apiPath ".env.example"
$envFile = Join-Path $apiPath ".env"

if (-not (Test-Path $envFile)) {
  Write-Host "⚠️  Creating .env file from template..." -ForegroundColor Yellow
  
  if (Test-Path $envExample) {
    Copy-Item $envExample $envFile -Force
    Write-Host "✓ Created: $envFile" -ForegroundColor Green
  } else {
    Write-Host "⚠️  .env.example not found; creating empty .env" -ForegroundColor Yellow
    New-Item -ItemType File -Path $envFile -Force | Out-Null
  }
  
  Write-Host ""
  Write-Host "⚠️  IMPORTANT: Update your WSO2 credentials in .env file" -ForegroundColor Yellow
} else {
  Write-Host "✓ .env file already exists" -ForegroundColor Green
  Write-Host ""
}

Write-Host ""
Write-Host "======================================" -ForegroundColor Cyan
Write-Host "✓ Setup Complete!" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📝 Next Steps:" -ForegroundColor Green
Write-Host ""
Write-Host "1️⃣  Edit WSO2 credentials:" -ForegroundColor White
Write-Host "   notepad .\src\DistributionManagement.API\.env"
Write-Host ""
Write-Host "   Update these values:" -ForegroundColor White
Write-Host "   - WSO2__TokenEndpoint" -ForegroundColor Gray
Write-Host "   - WSO2__UserInfoEndpoint" -ForegroundColor Gray
Write-Host "   - WSO2__ClientId" -ForegroundColor Gray
Write-Host "   - WSO2__ClientSecret" -ForegroundColor Gray
Write-Host ""
Write-Host "2️⃣  Build the solution:" -ForegroundColor White
Write-Host "   dotnet build"
Write-Host ""
Write-Host "3️⃣  Run the application:" -ForegroundColor White
Write-Host "   dotnet run --project .\src\DistributionManagement.API"
Write-Host ""
Write-Host "4️⃣  Access the application:" -ForegroundColor White
Write-Host "   🌐 Web UI: http://localhost:5299"
Write-Host "   📚 Swagger: http://localhost:5299/swagger"
Write-Host ""
Write-Host "For detailed instructions, see:" -ForegroundColor White
Write-Host "   📖 QUICKSTART.md"
Write-Host "   📘 README.md"
Write-Host ""
