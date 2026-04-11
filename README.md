# NovaCRM Enterprise Backend

NovaCRM is a production-grade, enterprise-scale CRM backend built using **ASP.NET Core 10.0**. It follows the principles of **Clean Architecture** and **Domain-Driven Design (DDD)**, utilizing high-performance patterns like **CQRS**, **Distributed Caching (Redis)**, and **Real-time WebSockets (SignalR)**.

## 🚀 Key Features

- **Enterprise Data Access**: Direct `IApplicationDbContext` usage optimized for EF Core (Projections, No-Tracking, Scalable IDs).
- **Real-time Notifications**: Automatic broadcasting of Domain Events (e.g., `DealWon`, `CustomerCreated`) to all connected clients via **SignalR**.
- **Distributed Caching**: High-speed read performance using **Redis** for dashboard analytics and JWT token blacklisting.
- **Security**: Robust JWT Authentication with **Redis-backed Token Revocation** (Stateless Blacklisting).
- **Architecture**: 100% decouple layers (Domain -> Application -> Infrastructure -> API).
- **Logging & Observability**: Structured logging using **Serilog** with **Correlation IDs** for request tracing.
- **Performance**: Optimized SQL queries with AutoMapper `.ProjectTo<T>()` to eliminate N+1 tracking issues.

---

## 🏗️ Technical Stack

- **Framework**: .NET 10.0
- **Database**: Entity Framework Core (SQLite for Dev, easily swappable to SQL Server/Postgres)
- **Mediator Pattern**: MediatR for CQRS
- **Caching**: StackExchange.Redis (Distributed Cache)
- **Real-time**: ASP.NET Core SignalR
- **Validation**: FluentValidation
- **Logging**: Serilog
- **Documentation**: Swagger / Scalar API docs

---

## 🛠️ Project Structure

- **NovaCRM.Domain**: Core entities, Enums, and Domain Events. No dependencies.
- **NovaCRM.Application**: Business logic, CQRS Handlers, DTOs, and Interfaces.
- **NovaCRM.Infrastructure**: Persistence (EF Core), Redis Services, Email, and Background Workers.
- **NovaCRM.API**: Controllers, Middlewares, SignalR Hubs, and API Configuration.
- **NovaCRM.UnitTests**: XUnit & NSubstitute for handler logic validation using InMemory EF Core.

---

## 📡 API Documentation

### 🔒 Authentication (`/api/Auth`)
| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| POST | `/api/Auth/register` | Anonymous | Register a new system user |
| POST | `/api/Auth/login` | Anonymous | Authenticate and receive JWT Token |
| POST | `/api/Auth/logout` | Authorize | Revoke JWT token and add to Redis blacklist |

### 👥 Customers (`/api/Customers`)
| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| GET | `/api/Customers` | User | Paged list of customers with search |
| GET | `/api/Customers/{id}` | User | Detailed customer information |
| POST | `/api/Customers` | WritePolicy | Create a new customer |
| PUT | `/api/Customers/{id}` | WritePolicy | Update customer details |
| DELETE| `/api/Customers/{id}` | Admin | Delete customer and related data |

### 💼 Deals (`/api/Deals`)
| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| GET | `/api/Deals` | User | List of deals filtered by stage |
| GET | `/api/Deals/pipeline` | User | Visual pipeline grouping |
| POST | `/api/Deals` | Sales/Admin | Create a new sales opportunity |
| PUT | `/api/Deals/{id}` | Sales/Admin | Update deal stage or value |

### 📊 Dashboard (`/api/Dashboard`)
| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| GET | `/api/Dashboard` | User | High-speed statistics (Redis Cached) |

### 🔔 SignalR (WebSockets)
- **Endpoint**: `/hubs/domain-events`
- **Event**: `ReceiveDomainEvent`
- **Description**: Frontend clients subscribe to this hub to receive instant updates on system-wide domain events.

---

## 🛠️ Data Schemas & API Response

### Common Response Model
All API responses follow a consistent wrapper:
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... },
  "correlationId": "8f3a..."
}
```

### Example: Create Customer Request
**POST** `/api/Customers`
```json
{
  "fullName": "Amr Radi",
  "email": "amr@example.com",
  "phone": "+20123456789",
  "company": "Tech Innovations",
  "status": "Lead"
}
```

---

## 🛡️ Error Handling
The system uses a Global Exception Middleware to return standardized RFC-compliant error responses.
- **400 Bad Request**: Validation failures (FluentValidation results).
- **401 Unauthorized**: Missing or expired JWT / Blacklisted token.
- **403 Forbidden**: Role/Policy check failure.
- **404 Not Found**: Resource non-existent.
- **429 Too Many Requests**: Rate limit exceeded (configured in `Program.cs`).

---

## ⚙️ Setup & Installation

### 1. Requirements
- .NET 10 SDK
- Redis Server (Optional: defaults to `localhost:6379`)

### 2. Configuration (`appsettings.json`)
Ensure your connection strings are set:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=NovaCRM.db",
    "Redis": "localhost:6379"
  }
}
```

### 3. Execution
```bash
dotnet build
dotnet run --project NovaCRM.API
```

---

## 🧪 Testing
The project includes a comprehensive suite of unit tests.
```bash
dotnet test
```

---

## 📝 License
This project is for demonstration and production deployment readiness. All comments and mock data have been purged for professional standards.
