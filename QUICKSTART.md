# Distribution Management System - Quick Start Guide

## WSO2 IS 7.1 + .NET 9 Integration

---

## 📋 Prerequisites

- ✅ macOS with Homebrew installed
- ✅ .NET 9 SDK: `brew install --cask dotnet-sdk`
- ✅ WSO2 IS 7.1 configured at https://iam.bimats.com

---

## 🚀 Quick Setup (5 minutes)

### Step 1: Run Setup Script

```bash
# Make script executable
chmod +x setup_script.sh

# Run setup
./setup_script.sh
```

This creates the complete project structure with all layers.

### Step 2: Add Code Files

Copy all code from the PDF guide into the respective files:

```
src/
├── DistributionManagement.Domain/
│   ├── Entities/Product.cs, User.cs
│   └── Enums/UserRole.cs
├── DistributionManagement.Application/
│   ├── DTOs/LoginRequest.cs, LoginResponse.cs, etc.
│   ├── Interfaces/IAuthenticationService.cs, etc.
│   └── Services/ProductService.cs
├── DistributionManagement.Infrastructure/
│   ├── Data/ApplicationDbContext.cs, DbInitializer.cs
│   ├── Repositories/ProductRepository.cs
│   └── ExternalServices/WSO2AuthenticationService.cs
└── DistributionManagement.API/
    ├── Controllers/AuthController.cs, ProductController.cs, UserController.cs
    ├── Middleware/ExceptionHandlingMiddleware.cs
    ├── Configuration/AuthenticationConfiguration.cs, DependencyInjection.cs
    ├── Program.cs
    └── appsettings.json
```

### Step 3: Configure Environment

```bash
cd src/DistributionManagement.API

# Copy environment file
cp .env.example .env

# Edit with your credentials (already pre-filled)
nano .env
```

### Step 4: Build and Run

```bash
# Build solution
dotnet build

# Run application
cd src/DistributionManagement.API
dotnet run
```

Application will start at:
- **Swagger UI**: https://localhost:5001/swagger
- **API**: https://localhost:5001/api

---

## 🔐 Testing Authentication

### 1. Login (Get Token)

**Using cURL:**
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "yks",
    "password": "your_password"
  }' \
  -k
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "refreshToken": "...",
  "userInfo": {
    "username": "yks",
    "email": "yks@example.com",
    "role": "yks_admin",
    "roles": ["yks_admin"],
    "groups": ["yksgroup"]
  }
}
```

### 2. Use Token for API Calls

**Get all products:**
```bash
curl -X GET https://localhost:5001/api/product \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -k
```

**Create product (admin/user only):**
```bash
curl -X POST https://localhost:5001/api/product \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "New Product",
    "description": "Product Description",
    "price": 99.99,
    "stockQuantity": 100,
    "category": "Electronics",
    "sku": "SKU999",
    "isActive": true
  }' \
  -k
```

---

## 👥 User Roles & Access

| User | Role | Access |
|------|------|--------|
| **yks** | yks_admin | View users, CRUD products |
| **yks1** | yks_test | View products only |
| **bimdevops** | yks_user | Edit & view products |

### Role Permissions

```
GET    /api/product         → yks_admin, yks_user, yks_test
POST   /api/product         → yks_admin, yks_user
PUT    /api/product/{id}    → yks_admin, yks_user
DELETE /api/product/{id}    → yks_admin only
GET    /api/user            → yks_admin only
```

---

## 🏗️ Project Structure

```
DistributionManagementSystem/
│
├── src/
│   ├── Domain/              # Core entities (Product, User)
│   ├── Application/         # Business logic & interfaces
│   ├── Infrastructure/      # Data access & WSO2 integration
│   └── API/                 # REST endpoints
│
├── .gitignore
├── README.md
└── DistributionManagementSystem.sln
```

**Clean Architecture Flow:**
```
API → Infrastructure → Application → Domain
```

---

## 🔧 Common Commands

### Development

```bash
# Run in watch mode (auto-reload)
dotnet watch run

# Build solution
dotnet build

# Clean solution
dotnet clean
```

### Database

```bash
# Create migration
dotnet ef migrations add MigrationName --project ../DistributionManagement.Infrastructure

# Apply migrations
dotnet ef database update --project ../DistributionManagement.Infrastructure

# Remove last migration
dotnet ef migrations remove --project ../DistributionManagement.Infrastructure
```

### Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

---

## 🐛 Troubleshooting

### Issue: Authentication Failed

**Solution:**
1. Verify WSO2 IS is running at https://iam.bimats.com
2. Check credentials in `.env` file
3. Ensure user exists in WSO2 IS with correct role
4. Test password grant type is enabled in WSO2 application

### Issue: Role Authorization Not Working

**Solution:**
1. Check JWT token contains `roles` claim (decode at jwt.io)
2. Verify role names match exactly (case-sensitive)
3. Check `RoleClaimType = "roles"` in JWT configuration

### Issue: Database Error

**Solution:**
```bash
# Delete database and recreate
rm distribution.db
dotnet ef database update --project ../DistributionManagement.Infrastructure
```

### Issue: HTTPS Certificate Error

**Solution:**
```bash
# Trust development certificate
dotnet dev-certs https --trust

# Or use -k flag with curl for testing
```

---

## 📝 API Endpoints

### Authentication

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/login` | ❌ | Login with username/password |
| GET | `/api/auth/me` | ✅ | Get current user info |
| POST | `/api/auth/validate` | ❌ | Validate JWT token |

### Products

| Method | Endpoint | Roles | Description |
|--------|----------|-------|-------------|
| GET | `/api/product` | All | Get all products |
| GET | `/api/product/{id}` | All | Get product by ID |
| POST | `/api/product` | Admin, User | Create new product |
| PUT | `/api/product/{id}` | Admin, User | Update product |
| DELETE | `/api/product/{id}` | Admin | Delete product |

### Users

| Method | Endpoint | Roles | Description |
|--------|----------|-------|-------------|
| GET | `/api/user` | Admin | Get all users |

---

## 🔐 Security Best Practices

✅ **DO:**
- Use `.env` file for sensitive data (never commit)
- Always use HTTPS in production
- Implement rate limiting
- Validate all user input
- Log security events
- Use strong passwords for WSO2 users

❌ **DON'T:**
- Commit secrets to Git
- Log sensitive data (passwords, tokens)
- Use HTTP in production
- Trust client-side validation only
- Expose detailed error messages to clients

---

## 📚 Key Technologies

- **.NET 9**: Latest framework
- **WSO2 IS 7.1**: Identity & Access Management
- **OAuth2 ROPC**: Password grant flow
- **JWT**: Bearer token authentication
- **Entity Framework Core**: ORM
- **Clean Architecture**: Separation of concerns
- **Swagger/OpenAPI**: API documentation

---

## 🌐 Environment Variables

Configure via `.env` file or system environment:

```bash
# WSO2 Configuration
WSO2__TokenEndpoint=https://iam.bimats.com/oauth2/token
WSO2__UserInfoEndpoint=https://iam.bimats.com/oauth2/userinfo
WSO2__ClientId=7eWli_Xh2SdqbNQHfex0nZfC1mUa
WSO2__ClientSecret=oyvSQgpwsvJWrbncCsqTR9lbVjPr4Sq8abdUhfN11R4a

# JWT Configuration
JWT__Issuer=https://iam.bimats.com/oauth2/token
JWT__Audience=7eWli_Xh2SdqbNQHfex0nZfC1mUa

# Database
Database__Provider=SQLite  # or PostgreSQL for production
ConnectionStrings__DefaultConnection=Data Source=distribution.db

# Environment
ASPNETCORE_ENVIRONMENT=Development
```

**Note:** Use double underscores `__` to represent nested JSON keys in environment variables.

---

## 🎯 Next Steps

After basic setup:

1. ✅ Test all API endpoints via Swagger
2. ✅ Implement frontend (React/Blazor/MVC)
3. ✅ Add unit and integration tests
4. ✅ Set up CI/CD pipeline
5. ✅ Configure production database (PostgreSQL)
6. ✅ Implement refresh token mechanism
7. ✅ Add comprehensive logging
8. ✅ Set up monitoring and alerting

---

## 📞 Support & Resources

- **WSO2 IS Docs**: https://is.docs.wso2.com/en/7.1.0/
- **.NET Docs**: https://learn.microsoft.com/en-us/dotnet/
- **OAuth2 Spec**: https://oauth.net/2/
- **JWT Info**: https://jwt.io

---

## ✅ Checklist

Before deploying to production:

- [ ] Change default credentials
- [ ] Use PostgreSQL database
- [ ] Configure SSL certificates
- [ ] Set up proper logging
- [ ] Implement rate limiting
- [ ] Add health checks
- [ ] Configure CORS properly
- [ ] Set up backup strategy
- [ ] Review security settings
- [ ] Load test the application

---

**Happy Coding! 🚀**

For detailed implementation, refer to the complete PDF guide.