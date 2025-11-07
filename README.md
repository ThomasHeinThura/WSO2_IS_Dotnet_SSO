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
