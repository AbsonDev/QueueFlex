# 🛠️ Guia de Desenvolvimento

## Pré-requisitos

### Software Necessário

- **.NET 8 SDK** ou superior
- **PostgreSQL 14+** 
- **Node.js 18+** e npm/yarn
- **Docker** e Docker Compose (opcional)
- **Git**

### IDEs Recomendadas

- **Visual Studio 2022** (Windows)
- **Visual Studio Code** com extensões C# e .NET
- **JetBrains Rider** (Multiplataforma)

## 🚀 Setup Inicial

### 1. Clonar o Repositório

```bash
git clone https://github.com/your-org/queue-management.git
cd queue-management
```

### 2. Configurar Banco de Dados

#### Opção A: PostgreSQL Local

```bash
# Criar banco de dados
psql -U postgres
CREATE DATABASE queue_management;
CREATE DATABASE queue_management_test; # Para testes
\q
```

#### Opção B: Docker

```bash
# Usar docker-compose
docker-compose up -d postgres
```

### 3. Configurar Connection String

Edite `QueueManagement.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=queue_management;Username=postgres;Password=yourpassword;Port=5432"
  }
}
```

### 4. Aplicar Migrations

```bash
# Navegar para a pasta da API
cd QueueManagement.Api

# Aplicar migrations
dotnet ef database update
```

### 5. Executar a API

```bash
# Na pasta QueueManagement.Api
dotnet run

# Ou com hot reload
dotnet watch run
```

A API estará disponível em:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger: http://localhost:5000/swagger

### 6. Setup do Frontend

```bash
# Navegar para o frontend
cd queuemanagement-admin

# Instalar dependências
npm install

# Executar em desenvolvimento
npm run dev
```

Frontend disponível em: http://localhost:5173

## 📁 Estrutura do Projeto

```
QueueManagement/
├── Domain/                    # Entidades e lógica de domínio
│   ├── Entities/             # Modelos do domínio
│   ├── Enums/               # Enumerações
│   └── ValueObjects/        # Objetos de valor
│
├── Infrastructure/           # Implementações de infraestrutura
│   ├── Data/               # DbContext e configurações
│   ├── Extensions/         # Métodos de extensão
│   └── Examples/           # Exemplos de uso
│
├── QueueManagement.Api/     # API REST
│   ├── Controllers/        # Endpoints da API
│   ├── DTOs/              # Data Transfer Objects
│   ├── Hubs/              # SignalR hubs
│   ├── Middleware/        # Middleware customizado
│   ├── Services/          # Serviços da aplicação
│   └── Validators/        # Validadores FluentValidation
│
├── QueueManagement.SDK/     # SDK cliente .NET
│   ├── src/              # Código fonte do SDK
│   ├── tests/            # Testes do SDK
│   └── examples/         # Exemplos de uso
│
├── QueueManagement.ValidationTests/  # Testes de integração
│
└── queuemanagement-admin/   # Frontend React
    ├── src/
    │   ├── components/    # Componentes React
    │   ├── pages/        # Páginas da aplicação
    │   ├── services/     # Serviços e API calls
    │   └── hooks/        # Custom React hooks
    └── public/           # Assets públicos
```

## 🔧 Comandos Úteis

### .NET

```bash
# Restaurar pacotes
dotnet restore

# Build
dotnet build

# Executar testes
dotnet test

# Criar nova migration
dotnet ef migrations add NomeDaMigracao --project Infrastructure --startup-project QueueManagement.Api

# Reverter migration
dotnet ef database update NomeMigracaoAnterior --project Infrastructure --startup-project QueueManagement.Api

# Gerar script SQL da migration
dotnet ef migrations script --project Infrastructure --startup-project QueueManagement.Api

# Limpar solução
dotnet clean
```

### NPM (Frontend)

```bash
# Instalar dependências
npm install

# Desenvolvimento
npm run dev

# Build para produção
npm run build

# Preview do build
npm run preview

# Executar testes
npm test

# Lint
npm run lint

# Format code
npm run format
```

## 🧪 Testes

### Executar Testes Unitários

```bash
# Todos os testes
dotnet test

# Com coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Testes específicos
dotnet test --filter "FullyQualifiedName~QueueManagement.Tests.TicketTests"
```

### Executar Testes de Integração

```bash
cd QueueManagement.ValidationTests
dotnet run
```

### Testes do Frontend

```bash
cd queuemanagement-admin
npm test
npm run test:coverage
```

## 🐛 Debugging

### Visual Studio

1. Abra `QueueManagement.sln`
2. Defina `QueueManagement.Api` como projeto de inicialização
3. Pressione F5 para debug

### VS Code

1. Abra a pasta do projeto
2. Vá para a aba Debug (Ctrl+Shift+D)
3. Selecione ".NET Core Launch (web)"
4. Pressione F5

### Logs

```csharp
// Logs são salvos em:
logs/queue-management-api-{date}.txt

// Para debug detalhado, ajuste appsettings.Development.json:
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

## 📝 Convenções de Código

### C# / .NET

```csharp
// Naming conventions
public class TicketService { }        // PascalCase para classes
public interface ITicketService { }   // Interface com prefixo I
private readonly ILogger _logger;     // Campos privados com underscore
public string TicketNumber { get; set; } // Propriedades em PascalCase

// Async/Await
public async Task<Ticket> GetTicketAsync(Guid id)
{
    return await _repository.GetByIdAsync(id);
}

// Dependency Injection
public class TicketService : ITicketService
{
    private readonly ITicketRepository _repository;
    
    public TicketService(ITicketRepository repository)
    {
        _repository = repository;
    }
}
```

### TypeScript / React

```typescript
// Componentes em PascalCase
export const TicketList: React.FC<Props> = ({ tickets }) => {
  // Hooks no início
  const [loading, setLoading] = useState(false);
  
  // Handlers com handle prefix
  const handleClick = () => {
    // ...
  };
  
  return <div>{/* JSX */}</div>;
};

// Interfaces com I prefix opcional
interface ITicket {
  id: string;
  number: string;
}

// Enums em PascalCase
enum TicketStatus {
  Waiting = 'WAITING',
  InService = 'IN_SERVICE'
}
```

## 🔄 Git Workflow

### Branch Strategy

```bash
main           # Produção
├── develop    # Desenvolvimento
    ├── feature/ticket-123-description
    ├── bugfix/ticket-456-fix
    └── hotfix/critical-issue
```

### Commit Messages

```bash
# Formato
<type>(<scope>): <subject>

# Exemplos
feat(api): add ticket creation endpoint
fix(frontend): resolve queue refresh issue
docs(readme): update installation instructions
refactor(domain): simplify ticket status logic
test(api): add integration tests for queues
```

### Pull Request Process

1. Criar branch da `develop`
2. Fazer commits atômicos
3. Executar testes localmente
4. Criar PR para `develop`
5. Code review obrigatório
6. Merge após aprovação

## 🚢 Deploy

### Development

```bash
# Build
dotnet publish -c Debug -o ./publish

# Docker
docker build -t queue-api:dev .
docker run -p 5000:80 queue-api:dev
```

### Production

```bash
# Build otimizado
dotnet publish -c Release -o ./publish --self-contained false

# Docker multi-stage
docker build -f Dockerfile.prod -t queue-api:latest .
```

### Environment Variables

```bash
# API
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=...
Jwt__SecretKey=your-production-key

# Frontend
VITE_API_URL=https://api.production.com
VITE_WS_URL=wss://api.production.com/hubs
```

## 📚 Recursos Adicionais

### Documentação

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [React Documentation](https://react.dev/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

### Ferramentas

- **Postman Collection**: `/docs/postman/collection.json`
- **Database Diagram**: `/docs/database/erd.png`
- **Architecture Diagrams**: `/docs/architecture/`

### Scripts Úteis

```bash
# Script de setup completo
./scripts/setup.sh

# Reset do banco de dados
./scripts/reset-db.sh

# Seed de dados de teste
./scripts/seed-data.sh
```

## 🆘 Troubleshooting

### Problema: Migrations falhando

```bash
# Verificar connection string
dotnet ef dbcontext info

# Forçar recreate
dotnet ef database drop --force
dotnet ef database update
```

### Problema: CORS errors no frontend

```csharp
// Verificar appsettings.json
"Cors": {
  "AllowedOrigins": ["http://localhost:5173"]
}
```

### Problema: SignalR não conecta

```javascript
// Verificar URL e token
const connection = new HubConnectionBuilder()
  .withUrl("http://localhost:5000/hubs/queue", {
    accessTokenFactory: () => getToken()
  })
  .build();
```

## 📞 Suporte

- **Email**: dev-team@queuemanagement.com
- **Slack**: #queue-management-dev
- **Wiki**: https://wiki.queuemanagement.com

---

📝 **Última atualização**: Dezembro 2024