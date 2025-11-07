# WSO2 IS + .NET 9 SSO - Quick Start Guide

## 🎯 Overview

A complete Single Sign-On (SSO) solution integrating **WSO2 Identity Server 7.1** with **.NET 9** featuring:
- Custom login page (OAuth2 Password Grant)
- JWT Bearer token authentication
- Role-based access control
- Product management system
- Web UI dashboard

**Repository:** https://github.com/ThomasHeinThura/WSO2_IS_Dotnet_SSO

---

## 📋 Prerequisites

- ✅ .NET 9 SDK ([download](https://dotnet.microsoft.com/download/dotnet/9.0))
- ✅ Git installed
- ✅ WSO2 IS 7.1 running at https://iam.bimats.com (or your instance)

---

## 🚀 Quick Setup (10 minutes)

### ⚙️ WINDOWS USERS (PowerShell)

#### Step 1: Clone Repository

```powershell
git clone https://github.com/ThomasHeinThura/WSO2_IS_Dotnet_SSO.git
cd WSO2_IS_Dotnet_SSO
```

#### Step 2: Allow Scripts (if blocked)

Run PowerShell as **Administrator**, then allow scripts temporarily:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
# or for persistent change:
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned -Force
```

#### Step 3: Run Setup Script

```powershell
.\setup.ps1
```

This will:
- ✅ Verify .NET 9 is installed
- ✅ Restore NuGet packages
- ✅ Create `.env` file from template
- ✅ Display next steps

#### Step 4: Configure WSO2 Credentials

Edit the `.env` file with your WSO2 details:

```powershell
notepad .\src\DistributionManagement.API\.env
```

Update these values:

```bash
# WSO2 OAuth2
WSO2__TokenEndpoint=https://iam.bimats.com/oauth2/token
WSO2__UserInfoEndpoint=https://iam.bimats.com/oauth2/userinfo
WSO2__ClientId=YOUR_CLIENT_ID
WSO2__ClientSecret=YOUR_CLIENT_SECRET

# JWT
JWT__Issuer=https://iam.bimats.com/oauth2/token
JWT__Audience=YOUR_CLIENT_ID

# Database
Database__Provider=SQLite
ConnectionStrings__DefaultConnection=Data Source=distribution.db

# Environment
ASPNETCORE_ENVIRONMENT=Development
```

#### Step 5: Build Solution

```powershell
dotnet build
```

Expected output:
```
Build succeeded with 0 warnings.
```

#### Step 6: Run Application

```powershell
dotnet run --project .\src\DistributionManagement.API
```

Expected output:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5299
```

#### Step 7: Access Application

| URL | Purpose |
|-----|---------|
| http://localhost:5299 | Web UI (Login page) |
| http://localhost:5299/swagger | API Documentation |

---

### 🐧 macOS/LINUX USERS (Bash)

#### Step 1: Clone Repository

```bash
git clone https://github.com/ThomasHeinThura/WSO2_IS_Dotnet_SSO.git
cd WSO2_IS_Dotnet_SSO
```

#### Step 2: Run Setup Script

```bash
# Make script executable
chmod +x setup.sh

# Run setup
./setup.sh
```

This will:
- ✅ Verify .NET 9 is installed
- ✅ Restore NuGet packages
- ✅ Create `.env` file from template
- ✅ Display next steps

#### Step 3: Configure WSO2 Credentials

Edit the `.env` file with your WSO2 details:

```bash
nano src/DistributionManagement.API/.env
```

Update these values:

```bash
# WSO2 OAuth2
WSO2__TokenEndpoint=https://iam.bimats.com/oauth2/token
WSO2__UserInfoEndpoint=https://iam.bimats.com/oauth2/userinfo
WSO2__ClientId=YOUR_CLIENT_ID
WSO2__ClientSecret=YOUR_CLIENT_SECRET

# JWT
JWT__Issuer=https://iam.bimats.com/oauth2/token
JWT__Audience=YOUR_CLIENT_ID

# Database
Database__Provider=SQLite
ConnectionStrings__DefaultConnection=Data Source=distribution.db

# Environment
ASPNETCORE_ENVIRONMENT=Development
```

#### Step 4: Build Solution

```bash
dotnet build
```

Expected output:
```
Build succeeded with 0 warnings.
```

#### Step 5: Run Application

```bash
dotnet run --project src/DistributionManagement.API
```

Expected output:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5299
```

#### Step 6: Access Application

| URL | Purpose |
|-----|---------|
| http://localhost:5299 | Web UI (Login page) |
| http://localhost:5299/swagger | API Documentation |

---

## 🔐 Test Login

Use these test users (must exist in your WSO2 IS):

| Username | Role | Access |
|----------|------|--------|
| **yks** | yks_admin | Full access + Users page |
| **bimdevops** | yks_user | Create & Edit products |
| **yks1** | yks_test | View products only |

### Via Web UI
1. Open http://localhost:5299
2. Enter username & password
3. View products dashboard

### Via cURL

```bash
# Login
curl -X POST http://localhost:5299/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "yks", "password": "YOUR_PASSWORD"}'
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6ImF0K2p3dCIsImtpZCI6Im...",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "refreshToken": "b6e698d1-a3ce-3808-a492-2f40950273b3",
  "userInfo": {
    "username": "yks",
    "email": "yks@example.com",
    "firstName": "yks",
    "lastName": "admin",
    "role": "yks_admin",
    "roles": ["yks_admin"],
    "groups": ["admin"]
  }
}
```

---

## 🛠️ Common Commands

### Build & Run

```bash
# Build solution
dotnet build

# Run with auto-reload (macOS/Linux)
dotnet watch run --project src/DistributionManagement.API

# Run application
dotnet run --project src/DistributionManagement.API
```

### Database

```bash
# Reset database (macOS/Linux)
rm src/DistributionManagement.API/distribution.db

# Reset database (Windows PowerShell)
Remove-Item .\src\DistributionManagement.API\distribution.db

# Restart application
dotnet run --project src/DistributionManagement.API
```

### API Testing

#### Get all products

```bash
curl -X GET http://localhost:5299/api/product \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Product A",
    "description": "Description for Product A",
    "price": 99.99,
    "stockQuantity": 100,
    "category": "Category 1",
    "sku": "SKU001",
    "isActive": true
  },
  {
    "id": 2,
    "name": "Product B",
    "description": "Description for Product B",
    "price": 149.99,
    "stockQuantity": 75,
    "category": "Category 1",
    "sku": "SKU002",
    "isActive": true
  }
]
```

#### Create product

```bash
curl -X POST http://localhost:5299/api/product \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "New Product",
    "description": "New Description",
    "price": 199.99,
    "stockQuantity": 50,
    "category": "Electronics",
    "sku": "SKU999",
    "isActive": true
  }'
```

**Response:**
```json
{
  "id": 12,
  "name": "New Product",
  "description": "New Description",
  "price": 199.99,
  "stockQuantity": 50,
  "category": "Electronics",
  "sku": "SKU999",
  "isActive": true
}
```

#### Get current user

```bash
curl -X GET http://localhost:5299/api/auth/me \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Response:**
```json
{
  "username": "yks",
  "email": "yks@example.com",
  "firstName": "yks",
  "lastName": "admin",
  "role": "yks_admin",
  "roles": ["yks_admin"]
}
```

---

## 📁 Project Structure

```
WSO2_IS_Dotnet_SSO/
├── src/
│   ├── DistributionManagement.Domain/
│   │   ├── Entities/
│   │   │   ├── Product.cs
│   │   │   └── User.cs
│   │   └── Enums/
│   │       └── UserRole.cs
│   │
│   ├── DistributionManagement.Application/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   └── Services/
│   │
│   ├── DistributionManagement.Infrastructure/
│   │   ├── Data/
│   │   ├── Repositories/
│   │   └── ExternalServices/
│   │       └── WSO2AuthenticationService.cs
│   │
│   └── DistributionManagement.API/
│       ├── Controllers/
│       ├── Middleware/
│       ├── Configuration/
│       ├── wwwroot/
│       │   ├── index.html (Login page)
│       │   ├── styles.css
│       │   └── app.js
│       ├── Program.cs
│       └── appsettings.json
│
├── setup.sh (macOS/Linux)
├── setup.ps1 (Windows PowerShell)
├── README.md
├── QUICKSTART.md
└── WSO2_IS_Dotnet_SSO.sln
```

---

## 📝 API Endpoints

### Authentication
```
POST   /api/auth/login       → Login (get JWT)
GET    /api/auth/me          → Current user info
POST   /api/auth/validate    → Validate token
```

### Products
```
GET    /api/product          → All products (all roles)
GET    /api/product/{id}     → Single product (all roles)
POST   /api/product          → Create (admin, user)
PUT    /api/product/{id}     → Update (admin, user)
DELETE /api/product/{id}     → Delete (admin only)
```

### Users
```
GET    /api/user             → All users (admin only)
```

---

## 🐛 Troubleshooting

### WSO2 Connection Failed

**Windows (PowerShell):**
```powershell
# Check WSO2 is running
Invoke-WebRequest -Uri "https://iam.bimats.com/oauth2/token" -Method Head

# Verify credentials in .env
Get-Content .\src\DistributionManagement.API\.env
```

**macOS/Linux (Bash):**
```bash
# Check WSO2 is running
curl -X GET https://iam.bimats.com/oauth2/token

# Verify credentials in .env
cat src/DistributionManagement.API/.env
```

### Port 5299 Already in Use

**Windows (PowerShell):**
```powershell
netstat -ano | findstr :5299
taskkill /PID <PID_NUMBER> /F
```

**macOS/Linux (Bash):**
```bash
lsof -ti:5299 | xargs kill -9
```

### 403 Unauthorized on API Calls

1. Verify token is valid (not expired)
2. Check Authorization header: `Authorization: Bearer TOKEN`
3. Verify user role exists in WSO2
4. Decode token at https://jwt.io

### Database Error

**Windows (PowerShell):**
```powershell
Remove-Item .\src\DistributionManagement.API\distribution.db
dotnet run --project .\src\DistributionManagement.API
```

**macOS/Linux (Bash):**
```bash
rm src/DistributionManagement.API/distribution.db
dotnet run --project src/DistributionManagement.API
```

---

## 🔑 Environment Variables

Configure via `.env` file:

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
Database__Provider=SQLite
ConnectionStrings__DefaultConnection=Data Source=distribution.db

# Environment
ASPNETCORE_ENVIRONMENT=Development
```

---

## 📚 Key Features

✅ **Custom Login Page** - No WSO2 hosted pages  
✅ **OAuth2 ROPC Flow** - Password grant authentication  
✅ **JWT Tokens** - Stateless authentication  
✅ **Role-Based Access Control** - 3 user roles  
✅ **Web Dashboard** - Modern UI for products management  
✅ **RESTful API** - Full CRUD operations  
✅ **Swagger Documentation** - Interactive API explorer  
✅ **Clean Architecture** - Domain, Application, Infrastructure, API layers  
✅ **SQLite Database** - 11 pre-seeded products  
✅ **Global Error Handling** - Centralized exception handling  

---

## 🚀 Next Steps

1. ✅ Clone repository
2. ✅ Run `setup.sh` (macOS/Linux) or `setup.ps1` (Windows)
3. ✅ Configure `.env` with WSO2 credentials
4. ✅ Build: `dotnet build`
5. ✅ Run: `dotnet run --project src/DistributionManagement.API`
6. ✅ Access: http://localhost:5299
7. ✅ Test login and API endpoints

---

## 📞 Support Resources

- [WSO2 IS Documentation](https://is.docs.wso2.com/en/7.1.0/)
- [.NET 9 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [OAuth2 Specification](https://oauth.net/2/)
- [JWT.io - JWT Debugger](https://jwt.io)

---

**Repository:** https://github.com/ThomasHeinThura/WSO2_IS_Dotnet_SSO  
**Happy Coding! 🚀**
