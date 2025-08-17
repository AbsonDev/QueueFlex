# Queue Management API - Sistema de Gestão de Filas

## 📋 Descrição

Sistema SaaS multi-tenant para gestão de atendimento e filas desenvolvido em .NET 8 com PostgreSQL. Permite que empresas gerenciem seus fluxos de atendimento de forma flexível e eficiente.

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture** com as seguintes camadas:

- **Domain**: Entidades, enums e value objects
- **Infrastructure**: DbContext, configurações do EF Core e extensões
- **Application**: Casos de uso e serviços (a ser implementado)
- **API**: Controllers e endpoints (a ser implementado)

## 🚀 Tecnologias

- **.NET 8** com ASP.NET Core Web API
- **PostgreSQL** como banco de dados principal
- **Entity Framework Core** para ORM
- **Npgsql.EntityFrameworkCore.PostgreSQL** para PostgreSQL
- **Clean Architecture** com separação de responsabilidades
- **Multi-tenant** por TenantId
- **Soft Delete** em todas as entidades
- **Audit Trail** automático

## 📁 Estrutura do Projeto

```
QueueManagement/
├── Domain/                          # Camada de Domínio
│   ├── Entities/                    # Entidades principais
│   │   ├── BaseEntity.cs           # Classe base com propriedades comuns
│   │   ├── Tenant.cs               # Empresa/Cliente do SaaS
│   │   ├── Unit.cs                 # Filiais/Unidades
│   │   ├── User.cs                 # Usuários do sistema
│   │   ├── Service.cs              # Tipos de serviço
│   │   ├── Queue.cs                # Filas de atendimento
│   │   ├── Ticket.cs               # Senhas/Atendimentos
│   │   ├── Session.cs              # Sessões de atendimento
│   │   ├── Resource.cs             # Recursos físicos
│   │   ├── Webhook.cs              # Integrações
│   │   └── JunctionTables/         # Tabelas de junção
│   │       ├── UnitUser.cs         # Usuários por unidade
│   │       ├── QueueService.cs     # Serviços por fila
│   │       └── TicketStatusHistory.cs # Histórico de status
│   ├── Enums/                      # Enumerações do sistema
│   └── ValueObjects/               # Objetos de valor
│       └── Address.cs              # Endereço físico
├── Infrastructure/                  # Camada de Infraestrutura
│   ├── Data/                       # Acesso a dados
│   │   ├── QueueManagementDbContext.cs # Contexto do EF Core
│   │   └── Configurations/         # Configurações das entidades
│   └── Extensions/                 # Extensões do EF Core
└── QueueManagement.sln             # Solução do Visual Studio
```

## 🗄️ Modelo de Dados

### Entidades Principais

1. **Tenant**: Empresa/Cliente do SaaS
2. **Unit**: Filiais/Unidades da empresa
3. **User**: Usuários (atendentes, gerentes, etc.)
4. **Service**: Tipos de serviço oferecidos
5. **Queue**: Filas de atendimento
6. **Ticket**: Senhas/Atendimentos dos clientes
7. **Session**: Sessões de atendimento
8. **Resource**: Recursos físicos (guichês, salas, etc.)

### Relacionamentos

- **Tenant** → **1:N** com todas as outras entidades
- **Unit** → **1:N** com Queue, Resource, UnitOperatingHour
- **User** → **M:N** com Unit (através de UnitUser)
- **Queue** → **M:N** com Service (através de QueueService)
- **Ticket** → **1:N** com Session, TicketStatusHistory

## ⚙️ Configuração

### 1. Pré-requisitos

- .NET 8 SDK
- PostgreSQL 12+
- Visual Studio 2022 ou VS Code

### 2. Configuração do Banco

1. Crie um banco PostgreSQL:
```sql
CREATE DATABASE queue_management;
```

2. Configure a connection string no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=queue_management;Username=postgres;Password=sua_senha;Port=5432"
  }
}
```

### 3. Instalação das Dependências

```bash
# Restaurar pacotes NuGet
dotnet restore

# Build da solução
dotnet build
```

### 4. Migrações do Banco

```bash
# Criar migração inicial
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project Domain

# Aplicar migrações
dotnet ef database update --project Infrastructure --startup-project Domain
```

## 🔧 Uso

### Configuração do DbContext

```csharp
// Program.cs ou Startup.cs
services.AddQueueManagementDbContext(Configuration);

// Aplicar migrações e seed data
var app = builder.Build();
await app.Services.ApplyMigrationsAsync();
await app.Services.SeedDatabaseAsync();
```

### Exemplo de Uso

```csharp
// Configurar contexto do tenant
context.SetTenantContext(tenantId, userId);

// Criar uma nova fila
var queue = new Queue("Fila Principal", "FP001", "Fila Principal - Atendimento", unitId, tenantId, "admin");
context.Queues.Add(queue);
await context.SaveChangesAsync();

// Buscar filas do tenant atual
var queues = await context.Queues.ToListAsync();
```

## 🎯 Funcionalidades

### Multi-tenancy
- Isolamento automático de dados por TenantId
- Global query filters para todas as entidades
- Configuração de contexto por tenant

### Soft Delete
- Todas as entidades suportam soft delete
- Global query filter automático
- Métodos para marcar como deletado/restaurar

### Audit Trail
- Campos automáticos: CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
- RowVersion para optimistic concurrency
- Histórico de mudanças de status

### Performance
- Índices compostos incluindo TenantId
- Configurações otimizadas para PostgreSQL
- Query splitting para consultas complexas

## 📊 Seed Data

O sistema inclui dados de exemplo para desenvolvimento:

- **Tenant Demo**: "Demo Company"
- **Unidade**: "Unidade Central"
- **Serviços**: "Atendimento Geral" e "Consulta Especializada"
- **Usuários**: Admin e Atendente
- **Recursos**: 2 guichês
- **Horários**: Segunda a sexta, 8h às 18h

## 🚧 Próximos Passos

1. **Implementar camada Application** com casos de uso
2. **Criar API Controllers** para todas as entidades
3. **Implementar autenticação e autorização**
4. **Adicionar validações** com FluentValidation
5. **Implementar testes unitários** com xUnit
6. **Adicionar logging** estruturado
7. **Implementar cache** com Redis
8. **Adicionar documentação** com Swagger

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 📞 Suporte

Para dúvidas ou suporte, entre em contato através das issues do GitHub.

---

**Desenvolvido com ❤️ para facilitar a gestão de filas e atendimento**