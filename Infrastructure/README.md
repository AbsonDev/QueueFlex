# Queue Management Infrastructure Layer

Esta camada contém todas as implementações dos repositories, Unit of Work e serviços de cache para o projeto Queue Management API.

## 🏗️ Estrutura do Projeto

```
Infrastructure/
├── Data/
│   ├── Interfaces/           # Interfaces dos repositories
│   ├── Repositories/         # Implementações dos repositories
│   ├── UnitOfWork/          # Unit of Work pattern
│   └── Caching/             # Serviços de cache
├── Extensions/               # Extensões para DI
└── README.md                # Este arquivo
```

## 🚀 Repositories Implementados

### 1. GenericRepository<T>
Repository base genérico com operações CRUD básicas e suporte multi-tenant.

**Características:**
- ✅ Suporte multi-tenant automático
- ✅ Soft delete implementado
- ✅ Auditoria automática (CreatedAt, UpdatedAt)
- ✅ Logging detalhado
- ✅ Tratamento de exceções robusto
- ✅ Queries otimizadas com AsNoTracking()

### 2. Repositories Específicos

#### TicketRepository
- Gerenciamento completo de tickets
- Queries otimizadas para filas
- Cálculo de posição na fila
- Estatísticas de tempo de espera
- Histórico de tickets por cliente

#### QueueRepository
- Gerenciamento de filas
- Status em tempo real
- Busca e filtros avançados
- Validação de códigos únicos

#### SessionRepository
- Gerenciamento de sessões ativas
- Estatísticas de duração
- Filtros por usuário e recurso

#### UnitRepository
- Gerenciamento de unidades
- Busca e validação de códigos
- Relacionamentos com filas e usuários

#### ServiceRepository
- Gerenciamento de serviços
- Validação de códigos únicos
- Busca por fila

#### UserRepository
- Gerenciamento de usuários
- Validação de email e código de funcionário
- Filtros por unidade e papel

#### TenantRepository
- Gerenciamento de tenants
- Busca por subdomínio
- Validação de subdomínios únicos

#### WebhookRepository
- Gerenciamento de webhooks
- Filtros por evento e tenant
- Log de execuções

## 🔄 Unit of Work

### Características
- ✅ Gerenciamento de transações
- ✅ Lazy loading de repositories
- ✅ Dispatch automático de domain events
- ✅ Auditoria automática
- ✅ Tratamento de conflitos de concorrência
- ✅ Rollback automático em caso de erro

### Uso
```csharp
public class TicketService
{
    private readonly IUnitOfWork _unitOfWork;

    public TicketService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Ticket> CreateTicketAsync(CreateTicketCommand command)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var ticket = new Ticket { /* ... */ };
            await _unitOfWork.TicketRepository.AddAsync(ticket);
            
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync(transaction);
            
            return ticket;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(transaction);
            throw;
        }
    }
}
```

## 🗄️ Sistema de Cache

### Características
- ✅ Cache em memória (IMemoryCache)
- ✅ Cache distribuído (Redis/SQL Server)
- ✅ Estratégias de expiração configuráveis
- ✅ Invalidação por padrão
- ✅ Fallback automático

### Configuração
```json
{
  "Cache": {
    "Provider": "Redis",
    "Redis": {
      "ConnectionString": "localhost:6379",
      "InstanceName": "QueueManagement"
    }
  }
}
```

### Uso
```csharp
public class CachedTicketRepository : ITicketRepository
{
    private readonly ICacheService _cacheService;
    
    public async Task<Ticket?> GetByIdAsync(Guid tenantId, Guid id)
    {
        var cacheKey = $"ticket:{tenantId}:{id}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetByIdAsync(tenantId, id), 
            TimeSpan.FromMinutes(15));
    }
}
```

## 📊 Performance e Otimizações

### Queries Otimizadas
- **AsNoTracking()** para queries somente leitura
- **Include()** para eager loading quando necessário
- **Projection** para grandes datasets
- **Pagination** automática
- **Index hints** através de ordenação

### Cache Strategy
- **Cache-aside pattern** implementado
- **TTL configurável** por tipo de dado
- **Invalidation automática** em updates
- **Layered caching** (Memory + Distributed)

### Multi-tenant Security
- **Filtro automático** por TenantId
- **Validação de acesso** em todos os métodos
- **Prevenção de vazamento** entre tenants
- **Global query filters** onde possível

## 🛠️ Configuração e Uso

### 1. Registrar no Program.cs
```csharp
using QueueManagement.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Adicionar repositories e serviços
builder.Services.AddRepositories(builder.Configuration);
```

### 2. Usar nos Controllers/Handlers
```csharp
[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public TicketsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetTicket(Guid id)
    {
        var tenantId = GetCurrentTenantId();
        var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(tenantId, id);
        
        if (ticket == null)
            return NotFound();
            
        return Ok(ticket);
    }
}
```

### 3. Usar nos CQRS Handlers
```csharp
public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Number = request.Number,
            QueueId = request.QueueId,
            ServiceId = request.ServiceId,
            TenantId = request.TenantId
        };

        await _unitOfWork.TicketRepository.AddAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
```

## 🔒 Segurança Multi-tenant

### Implementação
- Todas as entidades herdam de `BaseEntity` com `TenantId`
- Filtros automáticos em todas as queries
- Validação de acesso em todos os métodos
- Interface `IHasTenant` para tipagem

### Exemplo de Segurança
```csharp
public async Task<Ticket?> GetByIdAsync(Guid tenantId, Guid id)
{
    // Sempre filtra por TenantId para segurança
    return await _dbSet
        .AsNoTracking()
        .Where(e => e.Id == id && EF.Property<Guid>(e, "TenantId") == tenantId)
        .FirstOrDefaultAsync();
}
```

## 📈 Monitoramento e Logging

### Logging Estruturado
- **Log levels** apropriados (Debug, Info, Warning, Error)
- **Contexto rico** com IDs e parâmetros
- **Performance metrics** para queries lentas
- **Audit trail** completo

### Métricas de Performance
- Tempo de execução das queries
- Hit/miss ratio do cache
- Contagem de operações por tenant
- Alertas para queries lentas

## 🚨 Tratamento de Erros

### Exceções Específicas
- **DbUpdateConcurrencyException** para conflitos
- **DbUpdateException** para erros de validação
- **InvalidOperationException** para regras de negócio
- **ArgumentException** para parâmetros inválidos

### Estratégias de Retry
- Retry automático para falhas transitórias
- Circuit breaker para falhas persistentes
- Fallback para cache em caso de falha do banco

## 🔧 Configurações Avançadas

### Connection Pooling
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=QueueManagement;Username=postgres;Password=password;Maximum Pool Size=100;Minimum Pool Size=10"
  }
}
```

### Cache Policies
```json
{
  "Cache": {
    "Policies": {
      "Tickets": {
        "RealTimeDataMinutes": 2,
        "HistoricalDataMinutes": 30
      }
    }
  }
}
```

## 📚 Exemplos de Uso Avançado

### 1. Transação com Rollback
```csharp
public async Task ProcessTicketBatchAsync(List<CreateTicketCommand> commands)
{
    using var transaction = await _unitOfWork.BeginTransactionAsync();
    
    try
    {
        foreach (var command in commands)
        {
            var ticket = new Ticket { /* ... */ };
            await _unitOfWork.TicketRepository.AddAsync(ticket);
        }
        
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync(transaction);
    }
    catch
    {
        await _unitOfWork.RollbackTransactionAsync(transaction);
        throw;
    }
}
```

### 2. Cache com Invalidação Inteligente
```csharp
public async Task UpdateTicketAsync(UpdateTicketCommand command)
{
    var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(command.TenantId, command.Id);
    if (ticket == null) throw new NotFoundException();
    
    // Update ticket
    ticket.Update(command);
    await _unitOfWork.TicketRepository.UpdateAsync(ticket);
    
    // Invalidate related cache
    await _cacheService.RemoveAsync(
        $"ticket:{command.TenantId}:{command.Id}",
        $"tickets:queue:{command.TenantId}:{ticket.QueueId}:*"
    );
    
    await _unitOfWork.SaveChangesAsync();
}
```

### 3. Query Otimizada com Projection
```csharp
public async Task<List<TicketSummaryDto>> GetTicketSummariesAsync(Guid tenantId, Guid queueId)
{
    return await _dbSet
        .Where(t => t.QueueId == queueId && t.Queue.Unit.TenantId == tenantId)
        .Select(t => new TicketSummaryDto
        {
            Id = t.Id,
            Number = t.Number,
            Status = t.Status,
            Priority = t.Priority,
            IssuedAt = t.IssuedAt
        })
        .OrderBy(t => t.Priority)
        .ThenBy(t => t.IssuedAt)
        .ToListAsync();
}
```

## 🎯 Próximos Passos

### Melhorias Planejadas
- [ ] **Compiled Queries** para queries frequentes
- [ ] **Bulk Operations** para grandes datasets
- [ ] **Query Caching** com Redis
- [ ] **Performance Monitoring** com Application Insights
- [ ] **Database Sharding** para multi-tenant
- [ ] **Read Replicas** para queries de leitura

### Integrações
- [ ] **Redis Sentinel** para alta disponibilidade
- [ ] **Elasticsearch** para busca avançada
- [ ] **Hangfire** para jobs em background
- [ ] **SignalR** para notificações em tempo real

## 📞 Suporte

Para dúvidas ou sugestões sobre a implementação dos repositories, entre em contato com a equipe de desenvolvimento.

---

**Queue Management API** - Infrastructure Layer v1.0.0