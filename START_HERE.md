# 🚀 Queue Management Platform - Guia de Inicialização Rápida

## 📱 Otimizado para Mac Mini M4 (Apple Silicon)

Este guia foi criado especialmente para você iniciar todo o ambiente sem bugs ou problemas de configuração.

---

## 🎯 Visão Geral do Sistema

O Queue Management Platform consiste em 3 aplicações principais:

1. **API Backend** (.NET 8.0) - Porta 5000
2. **Admin Dashboard** (React + Vite) - Porta 5173  
3. **Platform Frontend** (Next.js) - Porta 3000

Além de:
- **PostgreSQL** - Banco de dados principal
- **Redis** - Cache e mensageria em tempo real

---

## ⚡ Início Rápido (5 minutos)

### 1️⃣ Configuração Inicial (Executar apenas uma vez)

```bash
# Clone o repositório (se ainda não tiver)
git clone [seu-repositorio]
cd [nome-do-projeto]

# Execute o script de setup automático
./scripts/setup-mac.sh
```

Este script irá:
- ✅ Instalar todas as dependências (Homebrew, Node.js, .NET, PostgreSQL, Redis)
- ✅ Configurar o banco de dados
- ✅ Instalar pacotes do projeto
- ✅ Criar arquivos de configuração
- ✅ Compilar os projetos

### 2️⃣ Iniciar Todos os Serviços

```bash
# Inicia tudo com um comando
./scripts/start-all.sh
```

### 3️⃣ Acessar as Aplicações

Após iniciar, acesse:

- 🔧 **API Documentation**: http://localhost:5000/swagger
- 👨‍💼 **Admin Dashboard**: http://localhost:5173
- 🌐 **Platform**: http://localhost:3000

### 4️⃣ Parar os Serviços

```bash
# Para todos os serviços
./scripts/stop-all.sh
```

---

## 🛠️ Scripts Disponíveis

Todos os scripts estão na pasta `scripts/`:

| Script | Descrição |
|--------|-----------|
| `setup-mac.sh` | Configuração inicial completa do ambiente |
| `start-all.sh` | Inicia todos os serviços de uma vez |
| `stop-all.sh` | Para todos os serviços |
| `start-api.sh` | Inicia apenas a API .NET |
| `start-admin.sh` | Inicia apenas o Admin Dashboard |
| `start-platform.sh` | Inicia apenas o Platform Frontend |
| `health-check.sh` | Verifica o status de todos os serviços |

---

## 🔍 Verificação de Saúde

Para verificar se tudo está funcionando:

```bash
./scripts/health-check.sh
```

Este comando verifica:
- Todas as dependências instaladas
- Serviços em execução
- Portas disponíveis
- Arquivos de configuração

---

## 📝 Comandos Úteis

### Desenvolvimento Individual

```bash
# API .NET
cd QueueManagement.Api
dotnet run

# Admin Dashboard
cd queuemanagement-admin
npm run dev

# Platform Frontend
cd queue-management-platform
npm run dev
```

### Logs em Tempo Real

```bash
# Ver logs da API
tail -f logs/api.log

# Ver logs do Admin
tail -f logs/admin.log

# Ver logs da Platform
tail -f logs/platform.log
```

### Banco de Dados

```bash
# Acessar PostgreSQL
psql -U postgres -d QueueManagement

# Verificar Redis
redis-cli ping
```

---

## 🐛 Solução de Problemas

### Problema: "Port already in use"

```bash
# Mata processos em portas específicas
lsof -ti:5000 | xargs kill -9  # API
lsof -ti:5173 | xargs kill -9  # Admin
lsof -ti:3000 | xargs kill -9  # Platform
```

### Problema: "Database connection failed"

```bash
# Reinicia PostgreSQL
brew services restart postgresql@16

# Verifica se está rodando
brew services list | grep postgresql
```

### Problema: "Redis connection failed"

```bash
# Reinicia Redis
brew services restart redis

# Verifica se está rodando
redis-cli ping
```

### Problema: "npm install failed"

```bash
# Limpa cache do npm
npm cache clean --force

# Remove node_modules e reinstala
rm -rf node_modules package-lock.json
npm install
```

### Problema: ".NET build failed"

```bash
# Limpa build anterior
dotnet clean

# Restaura pacotes
dotnet restore

# Reconstrói
dotnet build
```

---

## 🔐 Variáveis de Ambiente

As variáveis de ambiente já estão configuradas nos arquivos:

- `.env` - Configurações globais
- `queuemanagement-admin/.env` - Frontend Admin
- `queue-management-platform/.env.local` - Platform Frontend

**Importante**: Não commite esses arquivos no git!

---

## 📦 Estrutura do Projeto

```
queue-management-platform/
├── scripts/                    # Scripts de automação
│   ├── setup-mac.sh           # Setup inicial
│   ├── start-all.sh           # Inicia tudo
│   ├── stop-all.sh            # Para tudo
│   └── health-check.sh        # Verificação
├── QueueManagement.Api/        # API .NET
├── queuemanagement-admin/      # Admin Dashboard (React)
├── queue-management-platform/  # Platform (Next.js)
├── Domain/                     # Domínio .NET
├── Infrastructure/             # Infraestrutura .NET
└── logs/                       # Logs das aplicações
```

---

## 🚦 Status dos Serviços

Para verificar se tudo está rodando:

```bash
# Verifica todas as portas
lsof -i :5000  # API
lsof -i :5173  # Admin
lsof -i :3000  # Platform
lsof -i :5432  # PostgreSQL
lsof -i :6379  # Redis
```

---

## 💡 Dicas para Mac Mini M4

1. **Performance**: O M4 é extremamente rápido, todos os builds devem levar segundos
2. **Memória**: Com 16GB+ de RAM, você pode rodar tudo simultaneamente sem problemas
3. **Rosetta**: Não é necessário, tudo roda nativamente em ARM64
4. **Terminal**: Use o Terminal nativo ou iTerm2 para melhor experiência

---

## 🆘 Suporte Rápido

Se algo não funcionar:

1. Execute o health check:
   ```bash
   ./scripts/health-check.sh
   ```

2. Verifique os logs:
   ```bash
   ls -la logs/
   tail -f logs/*.log
   ```

3. Reinicie tudo:
   ```bash
   ./scripts/stop-all.sh
   ./scripts/start-all.sh
   ```

---

## ✅ Checklist de Verificação

Antes de começar a desenvolver, confirme:

- [ ] PostgreSQL está rodando na porta 5432
- [ ] Redis está rodando na porta 6379
- [ ] API .NET está acessível em http://localhost:5000/swagger
- [ ] Admin Dashboard está em http://localhost:5173
- [ ] Platform está em http://localhost:3000
- [ ] Todos os `npm install` foram executados
- [ ] O banco de dados `QueueManagement` existe

---

## 🎉 Pronto para Usar!

Se você seguiu os passos acima, seu ambiente está 100% configurado e funcionando. 

**Comando mágico para começar:**

```bash
./scripts/start-all.sh
```

Bom desenvolvimento! 🚀