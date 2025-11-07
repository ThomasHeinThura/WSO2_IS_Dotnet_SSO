#!/bin/bash

# WSO2 IS + .NET 9 SSO - Setup Script
# GitHub: https://github.com/ThomasHeinThura/WSO2_IS_Dotnet_SSO

set -e

echo "======================================"
echo "WSO2 IS + .NET 9 SSO Setup"
echo "GitHub: ThomasHeinThura/WSO2_IS_Dotnet_SSO"
echo "======================================"
echo ""

# Check if .NET 9 is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ Error: .NET 9 SDK is not installed"
    echo ""
    echo "Please install .NET 9 SDK from:"
    echo "https://dotnet.microsoft.com/download/dotnet/9.0"
    echo ""
    exit 1
fi

echo "✓ .NET Version: $(dotnet --version)"
echo ""

# Verify we're in the right directory
if [ ! -f "WSO2_IS_Dotnet_SSO.sln" ]; then
    echo "❌ Error: WSO2_IS_Dotnet_SSO.sln not found"
    echo ""
    echo "Please run this script from the repository root directory:"
    echo "  cd WSO2_IS_Dotnet_SSO"
    echo "  ./setup.sh"
    echo ""
    exit 1
fi

echo "✓ Found solution file: WSO2_IS_Dotnet_SSO.sln"
echo ""

# Restore NuGet packages
echo "📦 Restoring NuGet packages..."
echo ""
dotnet restore

echo ""
echo "✓ Packages restored successfully"
echo ""

# Check if .env exists
if [ ! -f "src/DistributionManagement.API/.env" ]; then
    echo "⚠️  Creating .env file from template..."
    cp src/DistributionManagement.API/.env.example src/DistributionManagement.API/.env
    echo "✓ Created: src/DistributionManagement.API/.env"
    echo ""
    echo "⚠️  IMPORTANT: Update your WSO2 credentials in .env file"
else
    echo "✓ .env file already exists"
    echo ""
fi

echo ""
echo "======================================"
echo "✓ Setup Complete!"
echo "======================================"
echo ""
echo "📝 Next Steps:"
echo ""
echo "1️⃣  Edit WSO2 credentials:"
echo "   nano src/DistributionManagement.API/.env"
echo ""
echo "   Update these values:"
echo "   - WSO2__TokenEndpoint"
echo "   - WSO2__UserInfoEndpoint"
echo "   - WSO2__ClientId"
echo "   - WSO2__ClientSecret"
echo ""
echo "2️⃣  Build the solution:"
echo "   dotnet build"
echo ""
echo "3️⃣  Run the application:"
echo "   dotnet run --project src/DistributionManagement.API"
echo ""
echo "4️⃣  Access the application:"
echo "   🌐 Web UI: http://localhost:5299"
echo "   📚 Swagger: http://localhost:5299/swagger"
echo ""
echo "For detailed instructions, see:"
echo "   📖 QUICKSTART.md"
echo "   📘 README.md"
echo ""