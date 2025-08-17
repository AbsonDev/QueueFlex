using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Enums;
using QueueManagement.Infrastructure.Data.Interfaces;

namespace QueueManagement.Infrastructure.Examples;

/// <summary>
/// Exemplos práticos de uso dos repositories implementados
/// </summary>
public static class RepositoryUsageExamples
{
    /// <summary>
    /// Exemplo de criação de ticket com validações
    /// </summary>
    public static async Task<Ticket> CreateTicketExample(
        IUnitOfWork unitOfWork,
        CreateTicketRequest request)
    {
        // Validar se a fila existe e está ativa
        var queue = await unitOfWork.QueueRepository.GetByIdAsync(request.TenantId, request.QueueId);
        if (queue == null || !queue.IsActive)
            throw new InvalidOperationException("Queue not found or inactive");

        // Validar se o serviço existe e está ativo
        var service = await unitOfWork.ServiceRepository.GetByIdAsync(request.TenantId, request.ServiceId);
        if (service == null || !service.IsActive)
            throw new InvalidOperationException("Service not found or inactive");

        // Gerar número único do ticket
        var ticketNumber = await GenerateUniqueTicketNumber(unitOfWork, request.TenantId, request.QueueId);

        // Criar ticket
        var ticket = new Ticket
        {
            Number = ticketNumber,
            QueueId = request.QueueId,
            ServiceId = request.ServiceId,
            Priority = request.Priority,
            CustomerName = request.CustomerName,
            CustomerDocument = request.CustomerDocument,
            CustomerPhone = request.CustomerPhone,
            Notes = request.Notes,
            TenantId = request.TenantId
        };

        // Definir informações de auditoria
        ticket.SetCreated(request.CreatedBy);

        // Adicionar ticket
        await unitOfWork.TicketRepository.AddAsync(ticket);

        // Salvar alterações
        await unitOfWork.SaveChangesAsync();

        return ticket;
    }

    /// <summary>
    /// Exemplo de processamento de fila
    /// </summary>
    public static async Task<ProcessQueueResult> ProcessQueueExample(
        IUnitOfWork unitOfWork,
        Guid tenantId,
        Guid queueId)
    {
        // Obter próximo ticket na fila
        var nextTicket = await unitOfWork.TicketRepository.GetNextInQueueAsync(queueId);
        if (nextTicket == null)
            return new ProcessQueueResult { Success = false, Message = "No tickets in queue" };

        // Obter usuário disponível para atendimento
        var availableUser = await GetAvailableUser(unitOfWork, tenantId, queueId);
        if (availableUser == null)
            return new ProcessQueueResult { Success = false, Message = "No available users" };

        // Criar sessão de atendimento
        var session = new Session
        {
            TicketId = nextTicket.Id,
            UserId = availableUser.Id,
            Status = SessionStatus.Active,
            StartedAt = DateTime.UtcNow,
            TenantId = tenantId
        };

        session.SetCreated(availableUser.Name);

        // Atualizar status do ticket
        nextTicket.Status = TicketStatus.InService;
        nextTicket.StartedAt = DateTime.UtcNow;
        nextTicket.SetUpdated(availableUser.Name);

        // Adicionar sessão e atualizar ticket
        await unitOfWork.SessionRepository.AddAsync(session);
        await unitOfWork.TicketRepository.UpdateAsync(nextTicket);

        // Salvar alterações
        await unitOfWork.SaveChangesAsync();

        return new ProcessQueueResult
        {
            Success = true,
            TicketId = nextTicket.Id,
            UserId = availableUser.Id,
            SessionId = session.Id
        };
    }

    /// <summary>
    /// Exemplo de busca avançada de tickets
    /// </summary>
    public static async Task<TicketSearchResult> SearchTicketsExample(
        IUnitOfWork unitOfWork,
        TicketSearchRequest request)
    {
        var result = new TicketSearchResult();

        // Buscar por fila específica
        if (request.QueueId.HasValue)
        {
            var tickets = await unitOfWork.TicketRepository.GetByQueueAsync(
                request.TenantId, 
                request.QueueId.Value, 
                request.Status);

            result.Tickets = tickets;
            result.TotalCount = tickets.Count;
        }
        // Buscar por cliente
        else if (!string.IsNullOrEmpty(request.CustomerDocument))
        {
            var tickets = await unitOfWork.TicketRepository.GetByCustomerAsync(
                request.TenantId, 
                request.CustomerDocument);

            result.Tickets = tickets;
            result.TotalCount = tickets.Count;
        }
        // Busca geral com paginação
        else
        {
            var tickets = await unitOfWork.TicketRepository.GetPagedAsync(
                request.TenantId, 
                request.Page, 
                request.PageSize);

            result.Tickets = tickets;
            result.TotalCount = await unitOfWork.TicketRepository.CountAsync(request.TenantId);
        }

        // Aplicar filtros adicionais
        if (request.FromDate.HasValue)
        {
            result.Tickets = result.Tickets
                .Where(t => t.IssuedAt >= request.FromDate.Value)
                .ToList();
        }

        if (request.ToDate.HasValue)
        {
            result.Tickets = result.Tickets
                .Where(t => t.IssuedAt <= request.ToDate.Value)
                .ToList();
        }

        // Calcular estatísticas
        result.AverageWaitTime = await unitOfWork.TicketRepository
            .GetAverageWaitTimeAsync(request.QueueId ?? Guid.Empty, request.FromDate);

        return result;
    }

    /// <summary>
    /// Exemplo de gerenciamento de unidades
    /// </summary>
    public static async Task<UnitManagementResult> ManageUnitExample(
        IUnitOfWork unitOfWork,
        Guid tenantId,
        CreateUnitRequest request)
    {
        // Validar se o código da unidade é único
        var codeExists = await unitOfWork.UnitRepository.CodeExistsAsync(tenantId, request.Code);
        if (codeExists)
            throw new InvalidOperationException($"Unit code '{request.Code}' already exists");

        // Criar unidade
        var unit = new Unit
        {
            Name = request.Name,
            Code = request.Code,
            Address = request.Address,
            Phone = request.Phone,
            Email = request.Email,
            IsActive = true,
            TenantId = tenantId
        };

        unit.SetCreated(request.CreatedBy);

        // Adicionar unidade
        await unitOfWork.UnitRepository.AddAsync(unit);

        // Criar fila padrão para a unidade
        var defaultQueue = new Queue
        {
            Name = "Fila Geral",
            Code = "GERAL",
            Description = "Fila padrão para atendimento geral",
            IsActive = true,
            UnitId = unit.Id,
            TenantId = tenantId
        };

        defaultQueue.SetCreated(request.CreatedBy);

        await unitOfWork.QueueRepository.AddAsync(defaultQueue);

        // Salvar alterações
        await unitOfWork.SaveChangesAsync();

        return new UnitManagementResult
        {
            UnitId = unit.Id,
            QueueId = defaultQueue.Id,
            Success = true
        };
    }

    /// <summary>
    /// Exemplo de relatório de performance
    /// </summary>
    public static async Task<PerformanceReport> GeneratePerformanceReportExample(
        IUnitOfWork unitOfWork,
        Guid tenantId,
        DateTime fromDate,
        DateTime toDate)
    {
        var report = new PerformanceReport
        {
            FromDate = fromDate,
            ToDate = toDate,
            TenantId = tenantId
        };

        // Obter todas as unidades do tenant
        var units = await unitOfWork.UnitRepository.GetActiveUnitsAsync(tenantId);

        foreach (var unit in units)
        {
            var unitStats = new UnitPerformanceStats
            {
                UnitId = unit.Id,
                UnitName = unit.Name
            };

            // Obter filas da unidade
            var queues = await unitOfWork.QueueRepository.GetByUnitAsync(tenantId, unit.Id);

            foreach (var queue in queues)
            {
                var queueStats = new QueuePerformanceStats
                {
                    QueueId = queue.Id,
                    QueueName = queue.Name
                };

                // Estatísticas de tickets
                var tickets = await unitOfWork.TicketRepository.GetByQueueAsync(tenantId, queue.Id);
                var periodTickets = tickets.Where(t => t.IssuedAt >= fromDate && t.IssuedAt <= toDate).ToList();

                queueStats.TotalTickets = periodTickets.Count;
                queueStats.CompletedTickets = periodTickets.Count(t => t.Status == TicketStatus.Completed);
                queueStats.CancelledTickets = periodTickets.Count(t => t.Status == TicketStatus.Cancelled);
                queueStats.AverageWaitTime = await unitOfWork.TicketRepository
                    .GetAverageWaitTimeAsync(queue.Id, fromDate);

                // Estatísticas de sessões
                var sessions = await unitOfWork.SessionRepository.GetCompletedSessionsAsync(tenantId, fromDate, toDate);
                var queueSessions = sessions.Where(s => s.Ticket.QueueId == queue.Id).ToList();

                queueStats.TotalSessions = queueSessions.Count;
                queueStats.AverageSessionDuration = unitOfWork.SessionRepository
                    .GetAverageSessionDurationAsync(tenantId, null, fromDate).Result;

                unitStats.Queues.Add(queueStats);
            }

            report.Units.Add(unitStats);
        }

        return report;
    }

    #region Helper Methods

    private static async Task<string> GenerateUniqueTicketNumber(
        IUnitOfWork unitOfWork, 
        Guid tenantId, 
        Guid queueId)
    {
        var today = DateTime.UtcNow.Date;
        var dailyCount = await unitOfWork.TicketRepository.GetDailyCountAsync(queueId, today);
        
        // Formato: A001, A002, etc.
        var queue = await unitOfWork.QueueRepository.GetByIdAsync(tenantId, queueId);
        var prefix = queue?.Code?.Substring(0, 1) ?? "T";
        var number = (dailyCount + 1).ToString("D3");
        
        return $"{prefix}{number}";
    }

    private static async Task<User?> GetAvailableUser(
        IUnitOfWork unitOfWork, 
        Guid tenantId, 
        Guid queueId)
    {
        // Obter usuários da unidade da fila
        var queue = await unitOfWork.QueueRepository.GetByIdAsync(tenantId, queueId);
        if (queue == null) return null;

        var users = await unitOfWork.UserRepository.GetByUnitAsync(tenantId, queue.UnitId);
        
        // Filtrar usuários ativos e disponíveis
        var availableUsers = users.Where(u => 
            u.IsActive && 
            u.Role == UserRole.Attendant).ToList();

        // Verificar se algum usuário não tem sessão ativa
        foreach (var user in availableUsers)
        {
            var activeSession = await unitOfWork.SessionRepository
                .GetActiveSessionByUserAsync(tenantId, user.Id);
            
            if (activeSession == null)
                return user;
        }

        return null;
    }

    #endregion
}

#region DTOs and Models

public class CreateTicketRequest
{
    public Guid TenantId { get; set; }
    public Guid QueueId { get; set; }
    public Guid ServiceId { get; set; }
    public Priority Priority { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerDocument { get; set; }
    public string? CustomerPhone { get; set; }
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class ProcessQueueResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? TicketId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? SessionId { get; set; }
}

public class TicketSearchRequest
{
    public Guid TenantId { get; set; }
    public Guid? QueueId { get; set; }
    public TicketStatus? Status { get; set; }
    public string? CustomerDocument { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class TicketSearchResult
{
    public List<Ticket> Tickets { get; set; } = new();
    public int TotalCount { get; set; }
    public double AverageWaitTime { get; set; }
}

public class CreateUnitRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class UnitManagementResult
{
    public bool Success { get; set; }
    public Guid UnitId { get; set; }
    public Guid QueueId { get; set; }
}

public class PerformanceReport
{
    public Guid TenantId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<UnitPerformanceStats> Units { get; set; } = new();
}

public class UnitPerformanceStats
{
    public Guid UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public List<QueuePerformanceStats> Queues { get; set; } = new();
}

public class QueuePerformanceStats
{
    public Guid QueueId { get; set; }
    public string QueueName { get; set; } = string.Empty;
    public int TotalTickets { get; set; }
    public int CompletedTickets { get; set; }
    public int CancelledTickets { get; set; }
    public double AverageWaitTime { get; set; }
    public int TotalSessions { get; set; }
    public double AverageSessionDuration { get; set; }
}

#endregion