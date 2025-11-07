#!/bin/bash

# Distribution Management System - Quick Setup Script for macOS
# This script will create the project structure and initialize the solution

set -e  # Exit on error

echo "======================================"
echo "Distribution Management System Setup"
echo "WSO2 IS 7.1 + .NET 9 Integration"
echo "======================================"
echo ""

# Check if .NET 9 is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET 9 SDK is not installed"
    echo "Please install from: https://dotnet.microsoft.com/download/dotnet/9.0"
    exit 1
fi

echo "✓ .NET SDK version: $(dotnet --version)"
echo ""

# Create solution directory
PROJECT_NAME="DistributionManagementSystem"
echo "Creating solution directory: $PROJECT_NAME"
# mkdir -p $PROJECT_NAME
# cd $PROJECT_NAME

# Create solution file
echo "Creating solution file..."
dotnet new sln -n $PROJECT_NAME

# Create src directory
mkdir -p src
cd src

# Create Domain Layer
echo "Creating Domain Layer..."
dotnet new classlib -n DistributionManagement.Domain
dotnet sln ../$PROJECT_NAME.sln add DistributionManagement.Domain
rm DistributionManagement.Domain/Class1.cs
mkdir -p DistributionManagement.Domain/Entities
mkdir -p DistributionManagement.Domain/Enums

# Create Application Layer
echo "Creating Application Layer..."
dotnet new classlib -n DistributionManagement.Application
dotnet sln ../$PROJECT_NAME.sln add DistributionManagement.Application
rm DistributionManagement.Application/Class1.cs
mkdir -p DistributionManagement.Application/Interfaces
mkdir -p DistributionManagement.Application/Services
mkdir -p DistributionManagement.Application/DTOs

# Create Infrastructure Layer
echo "Creating Infrastructure Layer..."
dotnet new classlib -n DistributionManagement.Infrastructure
dotnet sln ../$PROJECT_NAME.sln add DistributionManagement.Infrastructure
rm DistributionManagement.Infrastructure/Class1.cs
mkdir -p DistributionManagement.Infrastructure/Data
mkdir -p DistributionManagement.Infrastructure/Repositories
mkdir -p DistributionManagement.Infrastructure/ExternalServices

# Create API Layer
echo "Creating API Layer..."
dotnet new webapi -n DistributionManagement.API --use-minimal-apis false
dotnet sln ../$PROJECT_NAME.sln add DistributionManagement.API
mkdir -p DistributionManagement.API/Middleware
mkdir -p DistributionManagement.API/Configuration

# Add project references
echo "Adding project references..."
cd DistributionManagement.Application
dotnet add reference ../DistributionManagement.Domain

cd ../DistributionManagement.Infrastructure
dotnet add reference ../DistributionManagement.Domain
dotnet add reference ../DistributionManagement.Application

cd ../DistributionManagement.API
dotnet add reference ../DistributionManagement.Application
dotnet add reference ../DistributionManagement.Infrastructure

# Install NuGet packages
echo "Installing NuGet packages..."

cd ../DistributionManagement.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package System.IdentityModel.Tokens.Jwt

cd ../DistributionManagement.API
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package DotNetEnv
dotnet add package System.IdentityModel.Tokens.Jwt

cd ../DistributionManagement.Application
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions

cd ../..

# Create .gitignore
echo "Creating .gitignore..."
cat > .gitignore << 'EOF'
## Ignore Visual Studio temporary files, build results
*.suo
*.user
*.userosscache
*.sln.docstates

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
build/
bld/
[Bb]in/
[Oo]bj/

# Environment files
.env
.env.local
.env.production

# Database files
*.db
*.db-shm
*.db-wal

# VS Code
.vscode/

# JetBrains Rider
.idea/
*.sln.iml

# Mac
.DS_Store
EOF

# Create .env.example
echo "Creating .env.example..."
cat > src/DistributionManagement.API/.env.example << 'EOF'
# WSO2 Identity Server Configuration
WSO2__TokenEndpoint=https://iam.bimats.com/oauth2/token
WSO2__UserInfoEndpoint=https://iam.bimats.com/oauth2/userinfo
WSO2__ClientId=7eWli_Xh2SdqbNQHfex0nZfC1mUa
WSO2__ClientSecret=oyvSQgpwsvJWrbncCsqTR9lbVjPr4Sq8abdUhfN11R4a

# JWT Configuration
JWT__Issuer=https://iam.bimats.com/oauth2/token
JWT__Audience=7eWli_Xh2SdqbNQHfex0nZfC1mUa

# Database Configuration
Database__Provider=SQLite
ConnectionStrings__DefaultConnection=Data Source=distribution.db

# ASPNET Environment
ASPNETCORE_ENVIRONMENT=Development
EOF

# Create README.md
echo "Creating README.md..."
cat > README.md << 'EOF'
# Distribution Management System

A .NET 9 application integrated with WSO2 Identity Server 7.1 for authentication and authorization.

## Features

- Custom login page (OAuth2 Password Grant)
- JWT Bearer authentication
- Role-based access control (yks_admin, yks_user, yks_test)
- Clean Architecture
- RESTful API
- Swagger documentation

## Prerequisites

- .NET 9 SDK
- WSO2 Identity Server 7.1
- SQLite (development) or PostgreSQL (production)

## Quick Start

1. Copy .env.example to .env in the API project
2. Update credentials in .env file
3. Run the application:

\`\`\`bash
cd src/DistributionManagement.API
dotnet run
\`\`\`

4. Open Swagger UI: https://localhost:5001/swagger

## Project Structure

- **Domain**: Core entities and business rules
- **Application**: Business logic and interfaces
- **Infrastructure**: Data access and external services
- **API**: REST API endpoints and configuration

## Authentication

Use the `/api/auth/login` endpoint to authenticate:

\`\`\`json
{
  "username": "yks",
  "password": "your_password"
}
\`\`\`

## Role-Based Access

- **yks_admin**: Full access (view users, CRUD products)
- **yks_user**: Edit and view products
- **yks_test**: View-only access

## Documentation

See the PDF guide for complete implementation details.

## License

MIT License
EOF

echo ""
echo "======================================"
echo "✓ Setup completed successfully!"
echo "======================================"
echo ""
echo "Project structure created at: $PWD"
echo ""
echo "Next steps:"
echo "1. Copy all code files from the PDF guide into their respective locations"
echo "2. Copy .env.example to .env and update credentials:"
echo "   cd src/DistributionManagement.API"
echo "   cp .env.example .env"
echo "   nano .env"
echo ""
echo "3. Build the solution:"
echo "   dotnet build"
echo ""
echo "4. Run the application:"
echo "   cd src/DistributionManagement.API"
echo "   dotnet run"
echo ""
echo "5. Open Swagger UI: https://localhost:5001/swagger"
echo ""
