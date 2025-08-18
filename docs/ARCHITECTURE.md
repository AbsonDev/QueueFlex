# 🏗️ Arquitetura do Sistema

## Visão Geral

O Queue Management System segue os princípios da **Clean Architecture** e **Domain-Driven Design (DDD)**, garantindo separação de responsabilidades, testabilidade e manutenibilidade.

## Princípios Arquiteturais

### 1. Clean Architecture

```
┌─────────────────────────────────────────────┐
│                Presentation                  │
│         (Controllers, Hubs, DTOs)           │
├─────────────────────────────────────────────┤
│                Application                   │
│      (Use Cases, Commands, Queries)         │
├─────────────────────────────────────────────┤
│                  Domain                      │
│        (Entities, Value Objects)            │
├─────────────────────────────────────────────┤
│              Infrastructure                  │
│      (EF Core, External Services)           │
└─────────────────────────────────────────────┘
```

### 2. Camadas do Sistema

#### **Domain Layer** (Núcleo)
- **Responsabilidade**: Lógica de negócio pura
- **Componentes**:
  - Entities (BaseEntity, Tenant, User, Queue, Ticket, etc.)
  - Value Objects (Address)
  - Enums (UserRole, TicketStatus, Priority, etc.)
  - Domain Events (futura implementação)
- **Características**:
  - Sem dependências externas
  - Imutabilidade onde possível
  - Rica em comportamento de domínio

#### **Infrastructure Layer**
- **Responsabilidade**: Implementações concretas
- **Componentes**:
  - DbContext (QueueManagementDbContext)
  - Entity Configurations
  - Migrations
  - External Service Integrations
- **Características**:
  - Implementa abstrações do domínio
  - Gerencia persistência de dados
  - Integração com serviços externos

#### **Application Layer** (Em desenvolvimento)
- **Responsabilidade**: Orquestração de casos de uso
- **Componentes planejados**:
  - Commands & Command Handlers
  - Queries & Query Handlers
  - Application Services
  - DTOs & Mappings
- **Padrões**:
  - CQRS com MediatR
  - Repository Pattern
  - Unit of Work

#### **Presentation Layer** (API)
- **Responsabilidade**: Interface com o mundo externo
- **Componentes**:
  - REST Controllers
  - SignalR Hubs
  - Middleware
  - Filters
  - DTOs de Request/Response

## Padrões e Práticas

### 1. Multi-Tenancy

```csharp
// Isolamento automático por TenantId
public class BaseEntity
{
    public Guid TenantId { get; set; }
    // Global Query Filter aplicado automaticamente
}
```

**Implementação**:
- Global Query Filters no EF Core
- TenantId em todas as entidades
- Contexto de tenant por requisição
- Validação automática em todas as operações

### 2. Soft Delete

```csharp
public class BaseEntity
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
```

**Benefícios**:
- Recuperação de dados deletados
- Auditoria completa
- Conformidade com regulamentações

### 3. Audit Trail

```csharp
public class BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public byte[] RowVersion { get; set; } // Optimistic Concurrency
}
```

### 4. CQRS Pattern

```
Commands (Write)          Queries (Read)
     │                         │
     ▼                         ▼
  Handlers                 Handlers
     │                         │
     ▼                         ▼
  Domain                   Read Models
     │                         │
     ▼                         ▼
  Database ◄──────────────────┘
```

**Benefícios**:
- Separação de leitura e escrita
- Otimização independente
- Escalabilidade horizontal

### 5. Repository Pattern

```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

## Fluxo de Dados

### 1. Fluxo de Request

```
Client Request
     │
     ▼
Middleware Pipeline
     │
     ├─> Authentication
     ├─> Authorization
     ├─> Rate Limiting
     └─> Tenant Context
     │
     ▼
Controller/Hub
     │
     ▼
MediatR (Command/Query)
     │
     ▼
Handler
     │
     ▼
Domain Service
     │
     ▼
Repository
     │
     ▼
Database
```

### 2. Real-time Communication

```
Client ◄──────┐
   ▲          │
   │      SignalR Hub
   │          │
   └──────────┘
       
Events:
- Queue Updates
- Ticket Status Changes
- Dashboard Metrics
- System Notifications
```

## Modelo de Dados

### Entidades Principais

```
Tenant (1) ──────> (*) Unit
   │                    │
   │                    ├──> (*) Queue
   │                    ├──> (*) Resource
   │                    └──> (*) UnitOperatingHour
   │
   ├──> (*) User ◄────> (*) Unit (via UnitUser)
   │
   ├──> (*) Service ◄──> (*) Queue (via QueueService)
   │
   ├──> (*) Ticket
   │         │
   │         ├──> (*) Session
   │         └──> (*) TicketStatusHistory
   │
   └──> (*) Webhook
```

### Índices e Performance

- **Índices Compostos**: Incluem TenantId para particionamento eficiente
- **Índices Únicos**: Code + TenantId para garantir unicidade por tenant
- **Índices de Busca**: Em campos frequentemente consultados
- **Query Splitting**: Para queries complexas com múltiplos includes

## Segurança

### 1. Autenticação

- **JWT Bearer Tokens**
- **Refresh Tokens**
- **Token Expiration**: 60 minutos (configurável)

### 2. Autorização

- **Role-Based Access Control (RBAC)**
- **Roles**: Admin, Manager, Operator, Viewer
- **Policy-Based Authorization**

### 3. Proteção de Dados

- **Encryption at Rest**: Via PostgreSQL
- **Encryption in Transit**: HTTPS obrigatório
- **Sensitive Data Logging**: Desabilitado em produção

## Escalabilidade

### Horizontal Scaling

```
Load Balancer
     │
     ├──> API Instance 1
     ├──> API Instance 2
     └──> API Instance N
            │
            ▼
     PostgreSQL (Primary)
            │
            ├──> Read Replica 1
            └──> Read Replica 2
```

### Caching Strategy

- **Redis**: Para cache distribuído
- **In-Memory Cache**: Para dados estáticos
- **Response Caching**: Para endpoints GET

### Message Queue (Futuro)

- **RabbitMQ/Azure Service Bus**
- **Event-Driven Architecture**
- **Async Processing**

## Monitoramento e Observabilidade

### 1. Logging

- **Serilog**: Logging estruturado
- **Log Levels**: Configurável por ambiente
- **Log Sinks**: Console, File, ElasticSearch

### 2. Metrics

- **Health Checks**: /health endpoint
- **Performance Counters**
- **Custom Metrics**

### 3. Tracing

- **Correlation IDs**
- **Request/Response Logging**
- **Exception Tracking**

## Decisões Arquiteturais

### Por que Clean Architecture?

1. **Testabilidade**: Fácil criar testes unitários
2. **Manutenibilidade**: Código organizado e previsível
3. **Flexibilidade**: Fácil trocar implementações
4. **Independência**: Domínio não depende de frameworks

### Por que PostgreSQL?

1. **Open Source**: Sem custos de licença
2. **Performance**: Excelente para aplicações web
3. **Features**: JSON support, full-text search
4. **Escalabilidade**: Suporta grandes volumes

### Por que SignalR?

1. **Real-time**: Atualizações instantâneas
2. **Fallback**: Múltiplos transportes
3. **Integração**: Nativo do .NET
4. **Escalável**: Suporta backplane Redis

## Próximos Passos Arquiteturais

1. **Implementar Event Sourcing** para auditoria completa
2. **Adicionar API Gateway** para microserviços futuros
3. **Implementar SAGA Pattern** para transações distribuídas
4. **Adicionar GraphQL** como alternativa ao REST
5. **Implementar Circuit Breaker** para resiliência

---

📝 **Última atualização**: Dezembro 2024