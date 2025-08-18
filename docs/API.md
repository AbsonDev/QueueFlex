# 📡 Documentação da API

## Visão Geral

A Queue Management API é uma API RESTful que fornece endpoints para gerenciar filas, tickets, atendimentos e recursos em um sistema multi-tenant.

## Base URL

```
Development: http://localhost:5000
Production: https://api.queuemanagement.com
```

## Autenticação

A API utiliza **JWT Bearer Token** para autenticação.

### Obter Token

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "dGhpc2lzYXJlZnJlc2h0b2tlbg...",
    "expiresIn": 3600,
    "user": {
      "id": "uuid",
      "email": "user@example.com",
      "name": "John Doe",
      "role": "Manager",
      "tenantId": "uuid"
    }
  }
}
```

### Usar Token

Inclua o token em todas as requisições:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

## Endpoints

### 🎫 Tickets

#### Criar Ticket

```http
POST /api/tickets
Authorization: Bearer {token}
Content-Type: application/json

{
  "queueId": "uuid",
  "serviceId": "uuid",
  "customerName": "João Silva",
  "customerDocument": "123.456.789-00",
  "customerPhone": "+55 11 99999-9999",
  "priority": "Normal",
  "notes": "Cliente preferencial"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "uuid",
    "number": "A001",
    "status": "Waiting",
    "priority": "Normal",
    "issuedAt": "2024-01-01T10:00:00Z",
    "queueName": "Atendimento Geral",
    "serviceName": "Consulta",
    "estimatedWaitTime": "00:15:00"
  }
}
```

#### Buscar Ticket

```http
GET /api/tickets/{id}
Authorization: Bearer {token}
```

#### Atualizar Status do Ticket

```http
PATCH /api/tickets/{id}/status
Authorization: Bearer {token}
Content-Type: application/json

{
  "status": "InService",
  "resourceId": "uuid",
  "notes": "Atendimento iniciado"
}
```

#### Chamar Próximo Ticket

```http
POST /api/tickets/call-next
Authorization: Bearer {token}
Content-Type: application/json

{
  "queueId": "uuid",
  "resourceId": "uuid"
}
```

#### Listar Tickets

```http
GET /api/tickets?status=Waiting&queueId={uuid}&page=1&pageSize=20
Authorization: Bearer {token}
```

### 📋 Filas (Queues)

#### Criar Fila

```http
POST /api/queues
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Atendimento Prioritário",
  "code": "AP001",
  "displayName": "Prioritário",
  "unitId": "uuid",
  "maxCapacity": 100,
  "isActive": true,
  "serviceIds": ["uuid1", "uuid2"]
}
```

#### Listar Filas

```http
GET /api/queues?unitId={uuid}&isActive=true&page=1&pageSize=20
Authorization: Bearer {token}
```

#### Buscar Fila

```http
GET /api/queues/{id}
Authorization: Bearer {token}
```

#### Atualizar Fila

```http
PUT /api/queues/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Atendimento VIP",
  "displayName": "VIP",
  "maxCapacity": 50,
  "isActive": true
}
```

#### Status da Fila

```http
GET /api/queues/{id}/status
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "queueId": "uuid",
    "status": "Open",
    "currentTicketCount": 15,
    "waitingCount": 12,
    "inServiceCount": 3,
    "averageWaitTime": "00:18:30",
    "isAcceptingTickets": true,
    "isAtCapacity": false
  }
}
```

### 🏢 Unidades (Units)

#### Criar Unidade

```http
POST /api/units
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Filial Centro",
  "code": "FC001",
  "address": {
    "street": "Rua Principal",
    "number": "123",
    "city": "São Paulo",
    "state": "SP",
    "country": "Brasil",
    "zipCode": "01234-567"
  },
  "phone": "+55 11 3333-3333",
  "email": "centro@empresa.com",
  "isActive": true
}
```

#### Listar Unidades

```http
GET /api/units?isActive=true&page=1&pageSize=20
Authorization: Bearer {token}
```

#### Horários de Funcionamento

```http
GET /api/units/{id}/operating-hours
Authorization: Bearer {token}
```

```http
POST /api/units/{id}/operating-hours
Authorization: Bearer {token}
Content-Type: application/json

{
  "dayOfWeek": 1,
  "openTime": "08:00",
  "closeTime": "18:00",
  "isOpen": true
}
```

### 👥 Usuários (Users)

#### Criar Usuário

```http
POST /api/users
Authorization: Bearer {token}
Content-Type: application/json

{
  "email": "novo@usuario.com",
  "password": "SenhaSegura123!",
  "name": "Novo Usuário",
  "role": "Operator",
  "unitIds": ["uuid1", "uuid2"]
}
```

#### Listar Usuários

```http
GET /api/users?role=Operator&unitId={uuid}&page=1&pageSize=20
Authorization: Bearer {token}
```

#### Atualizar Usuário

```http
PUT /api/users/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Nome Atualizado",
  "role": "Manager",
  "isActive": true
}
```

#### Resetar Senha

```http
POST /api/users/{id}/reset-password
Authorization: Bearer {token}
Content-Type: application/json

{
  "newPassword": "NovaSenhaSegura123!"
}
```

### 🔧 Serviços (Services)

#### Criar Serviço

```http
POST /api/services
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Consulta Médica",
  "code": "CM001",
  "description": "Consulta com clínico geral",
  "estimatedDuration": "00:30:00",
  "isActive": true
}
```

#### Listar Serviços

```http
GET /api/services?isActive=true&page=1&pageSize=20
Authorization: Bearer {token}
```

### 💻 Recursos (Resources)

#### Criar Recurso

```http
POST /api/resources
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Guichê 01",
  "code": "G01",
  "type": "Counter",
  "unitId": "uuid",
  "isActive": true,
  "isAvailable": true
}
```

### 📊 Dashboard

#### Métricas em Tempo Real

```http
GET /api/dashboard/metrics
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "totalTicketsToday": 245,
    "ticketsWaiting": 18,
    "ticketsInService": 5,
    "ticketsCompleted": 222,
    "averageWaitTime": "00:12:30",
    "averageServiceTime": "00:08:45",
    "satisfactionRate": 4.5,
    "queues": [
      {
        "id": "uuid",
        "name": "Atendimento Geral",
        "waiting": 10,
        "inService": 3
      }
    ],
    "operators": [
      {
        "id": "uuid",
        "name": "João Silva",
        "status": "Available",
        "ticketsAttended": 35
      }
    ]
  }
}
```

#### Relatórios

```http
GET /api/dashboard/reports?startDate=2024-01-01&endDate=2024-01-31&type=daily
Authorization: Bearer {token}
```

### 🔄 WebSockets (SignalR Hubs)

#### Queue Hub

**Connection:**
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/hubs/queue", {
        accessTokenFactory: () => token
    })
    .build();
```

**Events:**

```javascript
// Receber atualizações de fila
connection.on("QueueUpdated", (queue) => {
    console.log("Fila atualizada:", queue);
});

// Novo ticket criado
connection.on("TicketCreated", (ticket) => {
    console.log("Novo ticket:", ticket);
});

// Ticket chamado
connection.on("TicketCalled", (ticket) => {
    console.log("Ticket chamado:", ticket);
});

// Status do ticket alterado
connection.on("TicketStatusChanged", (ticket) => {
    console.log("Status alterado:", ticket);
});
```

**Methods:**

```javascript
// Inscrever-se em atualizações de uma fila
await connection.invoke("SubscribeToQueue", queueId);

// Desinscrever-se
await connection.invoke("UnsubscribeFromQueue", queueId);
```

#### Dashboard Hub

**Events:**

```javascript
// Métricas em tempo real
connection.on("MetricsUpdated", (metrics) => {
    console.log("Métricas atualizadas:", metrics);
});

// Alertas do sistema
connection.on("SystemAlert", (alert) => {
    console.log("Alerta:", alert);
});
```

### 🔗 Webhooks

#### Registrar Webhook

```http
POST /api/webhooks
Authorization: Bearer {token}
Content-Type: application/json

{
  "url": "https://example.com/webhook",
  "events": ["ticket.created", "ticket.called", "ticket.completed"],
  "secret": "webhook-secret-key",
  "isActive": true
}
```

#### Eventos Disponíveis

- `ticket.created` - Novo ticket criado
- `ticket.called` - Ticket chamado para atendimento
- `ticket.started` - Atendimento iniciado
- `ticket.completed` - Atendimento finalizado
- `ticket.cancelled` - Ticket cancelado
- `queue.opened` - Fila aberta
- `queue.closed` - Fila fechada
- `queue.capacity_reached` - Capacidade máxima atingida

**Payload Example:**
```json
{
  "event": "ticket.created",
  "timestamp": "2024-01-01T10:00:00Z",
  "data": {
    "ticketId": "uuid",
    "number": "A001",
    "queueId": "uuid",
    "status": "Waiting"
  }
}
```

## Response Format

### Success Response

```json
{
  "success": true,
  "data": { },
  "meta": {
    "pagination": {
      "page": 1,
      "pageSize": 20,
      "totalItems": 100,
      "totalPages": 5,
      "hasNext": true,
      "hasPrevious": false
    }
  }
}
```

### Error Response

```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Validation failed",
    "details": [
      {
        "field": "email",
        "message": "Email is required"
      }
    ]
  }
}
```

## Status Codes

| Code | Description |
|------|-------------|
| 200 | OK - Requisição bem-sucedida |
| 201 | Created - Recurso criado |
| 204 | No Content - Sem conteúdo |
| 400 | Bad Request - Requisição inválida |
| 401 | Unauthorized - Não autenticado |
| 403 | Forbidden - Sem permissão |
| 404 | Not Found - Recurso não encontrado |
| 409 | Conflict - Conflito de dados |
| 422 | Unprocessable Entity - Validação falhou |
| 429 | Too Many Requests - Rate limit excedido |
| 500 | Internal Server Error - Erro no servidor |

## Rate Limiting

- **Limite**: 100 requisições por minuto
- **Window**: 1 minuto
- **Headers de Response**:
  - `X-RateLimit-Limit`: 100
  - `X-RateLimit-Remaining`: 95
  - `X-RateLimit-Reset`: 1609459200

## Versionamento

A API utiliza versionamento na URL:

```
/api/v1/tickets
/api/v2/tickets (futura versão)
```

## Swagger Documentation

Documentação interativa disponível em:

```
http://localhost:5000/swagger
```

## Postman Collection

Collection disponível para download:

```
http://localhost:5000/api/docs/postman-collection
```

## SDKs

### .NET SDK

```csharp
var client = new QueueManagementClient(apiKey);
var ticket = await client.Tickets.CreateAsync(new CreateTicketRequest
{
    QueueId = queueId,
    ServiceId = serviceId,
    CustomerName = "João Silva"
});
```

### JavaScript/TypeScript SDK (Em desenvolvimento)

```typescript
const client = new QueueManagementClient({ apiKey });
const ticket = await client.tickets.create({
    queueId,
    serviceId,
    customerName: "João Silva"
});
```

---

📝 **Última atualização**: Dezembro 2024