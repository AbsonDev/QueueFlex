# Queue Management API

A comprehensive REST API for managing queue systems with multi-tenant support, built with .NET 8 and ASP.NET Core.

## Features

- **Multi-tenant Architecture**: Support for multiple organizations with isolated data
- **Real-time Updates**: SignalR hubs for live queue and dashboard updates
- **JWT Authentication**: Secure token-based authentication and authorization
- **Comprehensive CRUD Operations**: Full management of units, queues, tickets, sessions, services, and users
- **Dashboard & Analytics**: Real-time metrics and performance insights
- **Webhook Support**: External system integration capabilities
- **Rate Limiting**: API protection against abuse
- **Health Checks**: Monitoring and operational insights
- **Swagger Documentation**: Interactive API documentation

## Architecture

The API follows a clean architecture pattern with the following layers:

- **API Layer**: Controllers, DTOs, Middleware, and SignalR Hubs
- **Application Layer**: MediatR commands/queries (to be implemented)
- **Domain Layer**: Business entities and logic
- **Infrastructure Layer**: Data access and external services

## Project Structure

```
QueueManagement.Api/
├── Controllers/           # API controllers
├── DTOs/                 # Data Transfer Objects
│   ├── Auth/            # Authentication DTOs
│   ├── Common/          # Common DTOs (pagination, filters)
│   ├── Dashboard/       # Dashboard metrics DTOs
│   ├── Queues/          # Queue management DTOs
│   ├── Sessions/        # Session management DTOs
│   ├── Services/        # Service management DTOs
│   ├── Tickets/         # Ticket management DTOs
│   ├── Units/           # Unit management DTOs
│   ├── Users/           # User management DTOs
│   └── Webhooks/        # Webhook management DTOs
├── Filters/              # Action filters
├── Hubs/                 # SignalR hubs for real-time communication
├── Mappings/             # AutoMapper profiles
├── Middleware/           # Custom middleware components
├── Services/             # Business logic services
├── Validators/           # FluentValidation validators
└── Program.cs            # Application entry point
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL database
- Redis (optional, for distributed caching)

### Configuration

1. **Database Connection**: Update the connection string in `appsettings.json`
2. **JWT Settings**: Configure JWT secret key and other settings
3. **CORS**: Adjust allowed origins for your frontend applications

### Running the Application

```bash
# Navigate to the API project
cd QueueManagement.Api

# Restore dependencies
dotnet restore

# Run the application
dotnet run

# Or build and run
dotnet build
dotnet run --project QueueManagement.Api
```

The API will be available at:
- **API**: https://localhost:7001/api/v1
- **Swagger UI**: https://localhost:7001
- **Health Check**: https://localhost:7001/health

## API Endpoints

### Authentication
- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/register` - Tenant registration
- `POST /api/v1/auth/refresh` - Token refresh
- `POST /api/v1/auth/validate` - Token validation

### Units
- `GET /api/v1/units` - List units
- `GET /api/v1/units/{id}` - Get unit details
- `POST /api/v1/units` - Create unit
- `PUT /api/v1/units/{id}` - Update unit
- `DELETE /api/v1/units/{id}` - Delete unit

### Queues
- `GET /api/v1/queues` - List queues
- `GET /api/v1/queues/{id}` - Get queue details
- `POST /api/v1/queues` - Create queue
- `PUT /api/v1/queues/{id}` - Update queue
- `DELETE /api/v1/queues/{id}` - Delete queue
- `GET /api/v1/queues/{id}/status` - Get queue status
- `PUT /api/v1/queues/{id}/status` - Update queue status

### Tickets
- `GET /api/v1/tickets` - List tickets
- `GET /api/v1/tickets/{id}` - Get ticket details
- `POST /api/v1/tickets` - Create ticket
- `PUT /api/v1/tickets/{id}` - Update ticket
- `DELETE /api/v1/tickets/{id}` - Cancel ticket
- `POST /api/v1/tickets/{id}/call` - Call ticket
- `POST /api/v1/tickets/{id}/transfer` - Transfer ticket

### Sessions
- `GET /api/v1/sessions` - List sessions
- `GET /api/v1/sessions/{id}` - Get session details
- `POST /api/v1/sessions` - Create session
- `POST /api/v1/sessions/{id}/complete` - Complete session
- `POST /api/v1/sessions/{id}/pause` - Pause session
- `POST /api/v1/sessions/{id}/resume` - Resume session

### Services
- `GET /api/v1/services` - List services
- `GET /api/v1/services/{id}` - Get service details
- `POST /api/v1/services` - Create service
- `PUT /api/v1/services/{id}` - Update service
- `DELETE /api/v1/services/{id}` - Delete service

### Users
- `GET /api/v1/users` - List users
- `GET /api/v1/users/{id}` - Get user details
- `POST /api/v1/users` - Create user
- `PUT /api/v1/users/{id}` - Update user
- `DELETE /api/v1/users/{id}` - Delete user
- `PATCH /api/v1/users/{id}/status` - Update user status

### Dashboard
- `GET /api/v1/dashboard/overview` - Get dashboard overview
- `GET /api/v1/dashboard/unit/{id}` - Get unit dashboard
- `GET /api/v1/dashboard/metrics` - Get dashboard metrics
- `GET /api/v1/dashboard/queues/status` - Get real-time queue status
- `GET /api/v1/dashboard/users/activity` - Get user activity summary

### Webhooks
- `GET /api/v1/webhooks` - List webhooks
- `GET /api/v1/webhooks/{id}` - Get webhook details
- `POST /api/v1/webhooks` - Create webhook
- `PUT /api/v1/webhooks/{id}` - Update webhook
- `DELETE /api/v1/webhooks/{id}` - Delete webhook
- `POST /api/v1/webhooks/{id}/test` - Test webhook

## SignalR Hubs

### QueueHub (`/hubs/queue`)
- Real-time queue status updates
- Ticket call notifications
- Queue capacity alerts

### DashboardHub (`/hubs/dashboard`)
- Live dashboard metrics
- Performance trend updates
- Real-time analytics

## Authentication & Authorization

The API uses JWT Bearer tokens for authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

### JWT Claims
- `sub`: User ID
- `email`: User email
- `role`: User role
- `tenant_id`: Tenant ID

### Authorization Policies
- `AdminOnly`: Admin users only
- `ManagerOrAdmin`: Manager and Admin users
- `AgentOrAbove`: Agent, Manager, and Admin users

## Multi-tenancy

The API supports multi-tenancy through:
- **JWT Claims**: Tenant ID embedded in JWT tokens
- **HTTP Headers**: `X-Tenant-ID` or `Tenant-ID` headers
- **Subdomain**: Automatic tenant resolution from subdomain
- **Query Parameters**: `tenant` or `tenantId` parameters

## Validation

Input validation is handled by FluentValidation with automatic validation through action filters. Validation errors return standardized error responses.

## Error Handling

The API uses standardized error responses with:
- HTTP status codes
- Error codes for programmatic handling
- Human-readable error messages
- Detailed validation errors when applicable

## Rate Limiting

API requests are rate-limited to prevent abuse:
- **Production**: 100 requests per minute per user
- **Development**: 1000 requests per minute per user

## Health Checks

Health check endpoint at `/health` provides:
- Database connectivity status
- Application health status
- Response time metrics

## Logging

Structured logging with Serilog:
- Request/response logging
- Performance metrics
- Error tracking
- Tenant and user context

## Caching

- **Memory Cache**: In-memory caching for frequently accessed data
- **Distributed Cache**: Redis-based caching for multi-instance deployments

## Development

### Adding New Endpoints

1. Create DTOs in the appropriate folder
2. Add validation rules
3. Create AutoMapper profiles
4. Implement controller actions
5. Add Swagger documentation

### Adding New Validators

1. Create validator class inheriting from `AbstractValidator<T>`
2. Define validation rules
3. Register in `Program.cs` if needed

### Adding New Mappings

1. Create mapping profile inheriting from `Profile`
2. Define mapping configurations
3. Register in `Program.cs`

## Testing

The API includes comprehensive test coverage (to be implemented):
- Unit tests for controllers and services
- Integration tests for API endpoints
- Performance tests for critical paths

## Deployment

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["QueueManagement.Api/QueueManagement.Api.csproj", "QueueManagement.Api/"]
COPY ["QueueManagement.Domain/QueueManagement.Domain.csproj", "QueueManagement.Domain/"]
COPY ["QueueManagement.Infrastructure/QueueManagement.Infrastructure.csproj", "QueueManagement.Infrastructure/"]
RUN dotnet restore "QueueManagement.Api/QueueManagement.Api.csproj"
COPY . .
WORKDIR "/src/QueueManagement.Api"
RUN dotnet build "QueueManagement.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QueueManagement.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QueueManagement.Api.dll"]
```

### Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Environment name
- `ConnectionStrings__DefaultConnection`: Database connection string
- `ConnectionStrings__Redis`: Redis connection string
- `Jwt__SecretKey`: JWT secret key
- `Jwt__Issuer`: JWT issuer
- `Jwt__Audience`: JWT audience

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation

## Roadmap

- [ ] Implement MediatR commands and queries
- [ ] Add comprehensive test coverage
- [ ] Implement webhook delivery system
- [ ] Add audit logging
- [ ] Implement user activity tracking
- [ ] Add advanced analytics
- [ ] Implement mobile push notifications
- [ ] Add API versioning
- [ ] Implement caching strategies
- [ ] Add monitoring and alerting