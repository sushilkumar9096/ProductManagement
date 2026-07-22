# Product Management API

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blueviolet.svg?style=for-the-badge&logo=.net)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![Docker](https://img.shields.io/badge/Docker-Enabled-blue.svg?style=for-the-badge&logo=docker)](https://www.docker.com/)
[![EF Core](https://img.shields.io/badge/EF_Core-8.0-blue.svg?style=for-the-badge)](https://learn.microsoft.com/en-us/ef/core/)
[![MSSQL](https://img.shields.io/badge/SQL_Server-2022-red.svg?style=for-the-badge&logo=microsoft-sql-server)](https://www.microsoft.com/en-us/sql-server/)
[![JWT](https://img.shields.io/badge/JWT-Authentication-orange.svg?style=for-the-badge&logo=json-web-tokens)](https://jwt.io/)

A robust, enterprise-grade ASP.NET Core 8 Web API solution designed for managing products and their associated stock items. This project incorporates Clean Architecture principles, Entity Framework Core with automated migrations, Role-Based Access Control (RBAC), and JWT-based authentication featuring secure Access/Refresh Token Rotation (RTR).

---

## 📖 Table of Contents
1. [Project Overview](#-project-overview)
2. [Features](#-features)
3. [Tech Stack](#-tech-stack)
4. [Architecture](#-architecture)
5. [Folder Structure](#-folder-structure)
6. [Documentation](#-documentation)
7. [Database Setup](#-database-setup)
8. [How to Run](#-how-to-run)
9. [Docker Instructions](#-docker-instructions)
10. [Authentication Flow](#-authentication-flow)
11. [API Endpoints](#-api-endpoints)
12. [Swagger & API Screenshots](#-swagger--api-screenshots)
13. [Future Improvements](#-future-improvements)

---

## 🔍 Project Overview

The **Product Management API** serves as a secure, scalable backend system to manage catalog products and inventory items. It implements a decoupled, maintainable structure where business rules are isolated from infrastructure details, databases, and UI layers. By utilizing ASP.NET Core 8, the API provides high throughput, cross-platform deployment capability, and robust developer experiences through interactive Swagger documentation.

---

## 🌟 Features

* **Clean Architecture**: Deep separation of concerns into Domain, Application, Infrastructure, and Presentation (Web API) projects.
* **Access/Refresh Token Rotation (RTR)**: Highly secure token rotation strategy ensuring short-lived access tokens (15 mins) and one-time use refresh tokens (7 days) with reuse detection.
* **Role-Based Access Control (RBAC)**: Fine-grained access level permissions separating standard users (read-only operations) and Administrators (authorized for all CRUD operations).
* **Automated Migrations & Auditing**: Automatic Entity Framework Core migrations on startup, and automatic auditing of `CreatedBy`/`CreatedOn` and `ModifiedBy`/`ModifiedOn` properties using DbContext hooks.
* **FluentValidation**: Strong request validation using FluentValidation integrated into ASP.NET Core action filters.
* **AutoMapper**: Automatic DTO mapping mapping Domain entities to clean application data transfer objects.
* **Serilog Logging**: Comprehensive logging to Console with minimum level overrides.
* **Comprehensive Test Suite**: Unit and integration test coverage across application layers using xUnit.

---

## 🛠️ Tech Stack

* **Language**: C# 12
* **Framework**: .NET 8.0 SDK
* **Database**: Microsoft SQL Server / LocalDB
* **ORM**: Entity Framework Core 8
* **Authentication**: JWT Bearer Tokens with custom rotation
* **Validation**: FluentValidation
* **Object Mapping**: AutoMapper 13
* **Logging**: Serilog
* **API Versioning**: ASP.NET Core API Versioning (v1.0)
* **Testing**: xUnit, FluentAssertions, Moq, Microsoft.AspNetCore.Mvc.Testing
* **Containerization**: Docker, Docker Compose

---

## 📐 Architecture

This project is built using **Clean Architecture** principles. The layers are structured as follows:

```mermaid
graph TD
    Domain[Product.Domain] -->|Independent| Domain
    Application[Product.Application] --> Domain
    Infrastructure[Product.Infrastructure] --> Application
    Api[Product.Api] --> Application
    Api --> Infrastructure
    
    style Domain fill:#2e7d32,stroke:#1b5e20,stroke-width:2px,color:#fff
    style Application fill:#1565c0,stroke:#0d47a1,stroke-width:2px,color:#fff
    style Infrastructure fill:#e65100,stroke:#b71c1c,stroke-width:2px,color:#fff
    style Api fill:#ad1457,stroke:#880e4f,stroke-width:2px,color:#fff
```

### Architectural Layer Breakdown
1. **Product.Domain**: Core entity models (`Product`, `Item`, `User`, `RefreshToken`), custom exceptions, and base constructs (independent of external frameworks/libraries).
2. **Product.Application**: Defines data transfer objects (DTOs), mappings, service interfaces, validators, and business logic implementation (`ProductService`, `ItemService`, `AuthService`).
3. **Product.Infrastructure**: Implements persistence details via EF Core DbContext, Repositories, Unit of Work, SQL migrations, and identity helper operations (JWT generation, password hashing).
4. **Product.Api**: The entry point API layer hosting Controllers, Middlewares, Validation Filters, API versioning setup, and Swagger configurations.

---

## 📂 Folder Structure

The structural organization of the repository is detailed below:

```
ProductManagement/
│
├── .github/                       # CI/CD workflow configurations
├── docs/                          # Project documentation and assets
│   └── screenshots/               # Interface and Swagger UI screenshots
│       ├── dashboard_mockup.png
│       └── swagger_ui.png
│
├── src/                           # Source projects
│   ├── Product.Domain/            # Domain Layer (Core Entities & Rules)
│   │   ├── Entities/
│   │   └── Exceptions/
│   │
│   ├── Product.Application/       # Application Layer (DTOs, Interfaces & Services)
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   ├── Mapping/
│   │   ├── Services/
│   │   └── Validators/
│   │
│   ├── Product.Infrastructure/    # Infrastructure Layer (DbContext, Repositories & Security)
│   │   ├── Data/
│   │   │   ├── Configurations/
│   │   │   └── Repositories/
│   │   ├── Identity/
│   │   └── Migrations/
│   │
│   └── Product.Api/               # Presentation Layer (API Controllers & Middlewares)
│       ├── Controllers/
│       │   └── v1/
│       ├── Extensions/
│       ├── Filters/
│       ├── Middleware/
│       ├── Properties/
│       ├── Services/
│       ├── appsettings.json
│       └── Program.cs
│
├── tests/                         # Automated test projects
│   ├── Product.Domain.Tests/
│   ├── Product.Application.Tests/
│   ├── Product.Infrastructure.Tests/
│   └── Product.Api.Tests/
│
├── docker-compose.yml             # Docker Compose orchestration definition
├── ProductManagement.slnx         # Solution definition file
└── README.md                      # This documentation file
```

---

## 📄 Documentation

This section describes the various layers of documentation configured within the codebase:

### 1. OpenAPI / Swagger Documentation
Interactive API playground and specification are automatically generated for all endpoints using **Swashbuckle.AspNetCore**:
* **Access URL**: Once the API is running, access `https://localhost:7200/swagger/index.html` (or `http://localhost:8080/swagger` in Docker).
* **Triple-Slash XML Comments**: Endpoints are documented using standard XML tags. The system compiles these comments into an XML file (`Product.Api.xml`) at build time (configured via `<GenerateDocumentationFile>true</GenerateDocumentationFile>` in [Product.Api.csproj](file:///d:/Asp.NetCore_WEBAPI/ProductManagement/src/Product.Api/Product.Api.csproj)). Swashbuckle reads this metadata to construct rich parameters and response schemas in the UI.

### 2. Code Documentation (XML Comments vs JSDoc)
Because this is a back-end C# (.NET) codebase rather than a JavaScript/TypeScript project, standard **C# XML Documentation Comments** are used in place of JSDoc syntax. Both formats serve the identical purpose of providing structured method metadata:

| Feature | JSDoc (JS/TS) | C# XML Comments (.NET) |
|---|---|---|
| **Syntax** | `/** Description */` | `/// <summary>Description</summary>` |
| **Parameters** | `@param {type} name Description` | `<param name="name">Description</param>` |
| **Return Values** | `@returns {type} Description` | `<returns>Description</returns>` |
| **Response Type** | N/A | `<response code="200">Description</response>` |

Example from [AuthController.cs](file:///d:/Asp.NetCore_WEBAPI/ProductManagement/src/Product.Api/Controllers/v1/AuthController.cs):
```csharp
/// <summary>
/// Registers a new user.
/// </summary>
/// <param name="request">The registration details.</param>
/// <returns>A token response with the access and refresh tokens.</returns>
/// <response code="200">User registered successfully.</response>
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
```

### 3. High-Level Authentication Flow
The security mechanism is detailed in the [Authentication Flow](#-authentication-flow) section. It maps out user logins, short-lived JWT token issuance, Refresh Token Rotation (RTR), and token reuse attack protection.

### 4. High-Level Environment Setup
High-level instructions to configure connection strings, run EF Core CLI migrations, and set up LocalDB are documented under the [Database Setup](#-database-setup) section.

### 5. High-Level Deployment Procedures
High-level steps for containerizing the API using multi-stage builds and launching orchestration are outlined under [Docker Instructions](#-docker-instructions).

---

## 💾 Database Setup

The project uses EF Core with automated migrations to handle database creation and schemas.

### 1. Connection String Configuration
Database configurations are managed in `src/Product.Api/appsettings.json`. Update the `DefaultConnection` string under `ConnectionStrings`:

* **For SQL Server LocalDB (Development)**:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProductManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }
  ```
* **For Local/Docker SQL Server Express**:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ProductManagementDb;User Id=sa;Password=YourSecurePassword123!;TrustServerCertificate=True;"
  }
  ```

### 2. Running Migrations CLI
If you want to manually manage or apply database migrations via EF Core Tools:
```bash
# Apply migrations to database
dotnet ef database update --project src/Product.Infrastructure --startup-project src/Product.Api
```

### 3. Startup Execution
During the API's bootstrap sequence (in `Program.cs`), a startup task automatically attempts to run pending database migrations (with retry logic), meaning manual database command execution is not strictly required.

---

## 🏃 How to Run Across Devices & Platforms

### 🖥️ 1. Running on Windows (Local Machine)
**Prerequisites**: [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) and [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (or Docker Desktop).

1. **Restore Packages & Build Solution**:
   ```bash
   dotnet build
   ```
2. **Launch the API**:
   ```bash
   dotnet run --project src/Product.Api
   ```
3. **Access Swagger UI**:
   - HTTPS: `https://localhost:7200/swagger/index.html`
   - HTTP: `http://localhost:5131/swagger/index.html`

---

### 🍏 🐧 2. Running on macOS & Linux
*Note: SQL Server LocalDB is Windows-only. On macOS and Linux, SQL Server should be run in a Docker container.*

1. **Start SQL Server via Docker**:
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourSecurePassword123!" -p 1433:1433 --name mssql_dev -d mcr.microsoft.com/mssql/server:2022-latest
   ```
2. **Update Connection String** in `src/Product.Api/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost,1433;Database=ProductManagementDb;User Id=sa;Password=YourSecurePassword123!;TrustServerCertificate=True;"
   }
   ```
3. **Run the API**:
   ```bash
   dotnet run --project src/Product.Api
   ```

---

### 🐳 3. Cross-Platform Docker Run (Windows, macOS, Linux — Recommended)
*No local SQL Server or .NET SDK installation required—runs identically on any device with Docker installed.*

1. **Start all services (API + SQL Server)**:
   ```bash
   docker-compose up --build -d
   ```
2. **Access Swagger UI**: `http://localhost:8080/swagger`
3. **Stop containers**: `docker-compose down`

---

### 📱 🌐 4. Accessing from Other Devices on Local Network (Mobile / Other PCs)
To test the API from mobile phones, tablets, or other computers on the same local Wi-Fi / LAN network:

1. **Find your host computer's Local IP address**:
   - **Windows**: Run `ipconfig` in CMD/PowerShell (look for `IPv4 Address`, e.g., `192.168.1.50`).
   - **macOS / Linux**: Run `ifconfig` or `ip a`.
2. **Bind API to all network interfaces (`0.0.0.0`)**:
   ```bash
   dotnet run --project src/Product.Api --urls "http://0.0.0.0:5131"
   ```
   *(Note: Docker Compose containers automatically listen on `0.0.0.0:8080`).*
3. **Access from any device on your Wi-Fi**:
   Open browser on phone or secondary PC and navigate to:
   - `http://<YOUR_LOCAL_IP>:5131/swagger` (e.g. `http://192.168.1.50:5131/swagger`)


---

## 🐳 Docker Instructions

The repository includes support for containerized environments using Docker and Docker Compose.

### 1. Traditional Docker Image Build
You can build the API image manually from the root directory:
```bash
docker build -t product-api:latest -f src/Product.Api/Dockerfile .
```

### 2. Orchestrated Run (Docker Compose)
To start both SQL Server and the API services in isolated, configured containers:

1. Launch the docker-compose services:
   ```bash
   docker-compose up --build -d
   ```
2. The containers will start on the following ports:
   - **Product API**: `http://localhost:8080` (HTTP) / `http://localhost:8081` (HTTPS)
   - **SQL Server DB**: `localhost:1433`
3. The database migrations run automatically on container startup. Access the Swagger endpoint:
   - `http://localhost:8080/swagger`
4. To stop the containers:
   ```bash
   docker-compose down
   ```

---

## 🔒 Authentication Flow

The application utilizes a secure JWT-based authentication flow with **Refresh Token Rotation (RTR)**.

```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant AuthController
    participant AuthService
    participant Database

    Client->>AuthController: POST /auth/login (username, password)
    AuthController->>AuthService: LoginAsync()
    AuthService->>Database: Verify credentials
    Database-->>AuthService: Valid user
    AuthService->>AuthService: Generate Access Token (15m) & Refresh Token (7d)
    AuthService->>Database: Store Refresh Token (revoke old ones)
    AuthService-->>AuthController: Return Access + Refresh tokens
    AuthController-->>Client: HTTP 200 OK

    Note over Client, Database: Access Token expires...

    Client->>AuthController: POST /auth/refresh (Expired Access + Current Refresh)
    AuthController->>AuthService: RefreshTokenAsync()
    AuthService->>Database: Verify Refresh Token validity
    
    alt Refresh Token is active
        AuthService->>Database: Revoke old Refresh Token & Save new Refresh Token (RTR)
        AuthService->>AuthService: Generate new Access Token
        AuthService-->>Client: HTTP 200 OK (New Access + New Refresh)
    else Refresh Token is already Revoked (Reuse Detected!)
        AuthService->>Database: Revoke ALL active sessions for this user (Account Locked)
        AuthService-->>Client: HTTP 400 Bad Request (Session Compromised)
    end
```

---

## 🔌 API Endpoints

### 🔐 Authentication (`api/v1/auth/*`)
| HTTP Method | Endpoint | Request Payload | Description | Access Level |
|---|---|---|---|---|
| **POST** | `/api/v1/auth/register` | `RegisterRequest` DTO | Registers a new user. | Anonymous |
| **POST** | `/api/v1/auth/login` | `LoginRequest` DTO | Authenticates user credentials and returns tokens. | Anonymous |
| **POST** | `/api/v1/auth/refresh` | `RefreshTokenRequest` DTO | Refreshes expired access tokens using a refresh token. | Anonymous |
| **POST** | `/api/v1/auth/revoke` | `string` (Refresh Token) | Revokes the provided refresh token (logs out session). | Authenticated |

### 📦 Products (`api/v1/products/*`)
| HTTP Method | Endpoint | Request Payload | Description | Access Level |
|---|---|---|---|---|
| **GET** | `/api/v1/products` | Query Params (Page, Size) | Retrieves a paginated list of all products. | Authenticated |
| **GET** | `/api/v1/products/{id}` | None | Retrieves details of a specific product by its ID. | Authenticated |
| **POST** | `/api/v1/products` | `CreateProductRequest` DTO | Creates a new catalog product. | Administrator |
| **PUT** | `/api/v1/products/{id}` | `UpdateProductRequest` DTO | Updates details of an existing product. | Administrator |
| **DELETE** | `/api/v1/products/{id}` | None | Deletes a product catalog entry. | Administrator |

### 🔍 Product Items (`api/v1/items/*` & `/products/{id}/items`)
| HTTP Method | Endpoint | Request Payload | Description | Access Level |
|---|---|---|---|---|
| **GET** | `/api/v1/products/{productId}/items` | None | Retrieves all items associated with a specific product. | Authenticated |
| **GET** | `/api/v1/items/{id}` | None | Retrieves details of a specific item by its ID. | Authenticated |
| **POST** | `/api/v1/items` | `CreateItemRequest` DTO | Creates a new inventory item. | Administrator |
| **PUT** | `/api/v1/items/{id}` | `UpdateItemRequest` DTO | Updates properties of an existing item. | Administrator |
| **DELETE** | `/api/v1/items/{id}` | None | Deletes a specific inventory item. | Administrator |

---

## 📊 Swagger & API Screenshots

Below are the actual screenshots showing the interactive Swagger API documentation playground and testing operations:

### 1. Swagger UI Main Dashboard
The Swagger playground hosts all Auth, Products, and Items endpoints:
![Swagger Home](docs/screenshots/swagger_home.png)

### 2. User Registration Endpoint
Requesting new user registration via the POST action body:
![User Registration](docs/screenshots/swagger_register_request.png)

### 3. Successful Authentication & JWT Token Return
Successful `200 OK` response returning the short-lived JWT access token and refresh token:
![Successful Login Response](docs/screenshots/swagger_login_response.png)

### 4. Product Catalog Resource Endpoints
Managing product catalogs via GET, POST, PUT, and DELETE methods:
![Product Catalog Actions](docs/screenshots/swagger_products.png)

### 5. Product Items Resource Endpoints
Managing inventory items (child resources of products) via GET, POST, PUT, and DELETE methods:
![Product Items Actions](docs/screenshots/swagger_items.png)

---
## 🚀 Future Improvements

To transition this project into a production-level service, the following improvements are recommended:
1. **Redis Cache Integration**: Implement memory-caching of product catalogs to optimize lookup performance and reduce direct database stress.
2. **Rate Limiting**: Apply endpoint rate-limiting middleware (e.g. AspNetCoreRateLimit) to guard against DDoS threats and automated scraping.
3. **Enhanced Logging (ELK / Grafana)**: Pipe Serilog output to central indexers such as Elasticsearch/Grafana Loki for detailed trace metrics.
4. **CI/CD Integration**: Construct automated GitHub Actions pipelines to run tests, build Docker images, and publish packages automatically.
5. **Secure Vault Configuration**: Transition secrets (JWT signing keys, DB SA passwords) to cloud key management solutions like Azure Key Vault or AWS Secrets Manager.
