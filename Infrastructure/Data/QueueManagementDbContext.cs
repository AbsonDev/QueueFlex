using Microsoft.EntityFrameworkCore;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Entities.JunctionTables;
using QueueManagement.Domain.ValueObjects;

namespace QueueManagement.Infrastructure.Data;

/// <summary>
/// Main database context for the Queue Management system
/// </summary>
public class QueueManagementDbContext : DbContext
{
    /// <summary>
    /// Current tenant ID for multi-tenancy
    /// </summary>
    public Guid CurrentTenantId { get; set; }

    /// <summary>
    /// Current user ID for audit purposes
    /// </summary>
    public string CurrentUserId { get; set; } = string.Empty;

    // DbSets for main entities
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<UnitOperatingHour> UnitOperatingHours { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Queue> Queues { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Webhook> Webhooks { get; set; }

    // DbSets for junction tables
    public DbSet<UnitUser> UnitUsers { get; set; }
    public DbSet<QueueService> QueueServices { get; set; }
    public DbSet<TicketStatusHistory> TicketStatusHistories { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public QueueManagementDbContext(DbContextOptions<QueueManagementDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures the model using Fluent API
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(QueueManagementDbContext).Assembly);

        // Configure value objects
        ConfigureValueObjects(modelBuilder);

        // Configure global query filters
        ConfigureGlobalQueryFilters(modelBuilder);

        // Configure indexes
        ConfigureIndexes(modelBuilder);
    }

    /// <summary>
    /// Configures value objects
    /// </summary>
    private void ConfigureValueObjects(ModelBuilder modelBuilder)
    {
        // Configure Address as owned entity type
        modelBuilder.Entity<Unit>()
            .OwnsOne(u => u.Address, address =>
            {
                address.Property(a => a.Street).HasColumnName("street").HasMaxLength(200);
                address.Property(a => a.Number).HasColumnName("number").HasMaxLength(20);
                address.Property(a => a.Complement).HasColumnName("complement").HasMaxLength(100);
                address.Property(a => a.Neighborhood).HasColumnName("neighborhood").HasMaxLength(100);
                address.Property(a => a.City).HasColumnName("city").HasMaxLength(100);
                address.Property(a => a.State).HasColumnName("state").HasMaxLength(100);
                address.Property(a => a.ZipCode).HasColumnName("zip_code").HasMaxLength(20);
                address.Property(a => a.Country).HasColumnName("country").HasMaxLength(100);
            });
    }

    /// <summary>
    /// Configures global query filters for multi-tenancy and soft delete
    /// </summary>
    private void ConfigureGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        // Global query filter for multi-tenancy (excluding Tenant entity)
        modelBuilder.Entity<Unit>().HasQueryFilter(e => e.TenantId == CurrentTenantId && !e.IsDeleted);
        modelBuilder.Entity<UnitOperatingHour>().HasQueryFilter(e => e.TenantId == CurrentTenantId && !e.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(e => e.TenantId == CurrentTenantId && !e.IsDeleted);
        modelBuilder.Entity<Service>().HasQueryFilter(e => e.TenantId == CurrentTenantId && !e.IsDeleted);
        modelBuilder.Entity<Queue>().HasQueryFilter(e => e.TenantId == CurrentTenantId && !e.IsDeleted);
        modelBuilder.Entity<Ticket>().HasQueryFilter(e => e.TenantId == CurrentTenantId && !e.IsDeleted);
        modelBuilder.Entity<Session>().HasQueryFilter(e => e.TenantId == CurrentTenantId && !e.IsDeleted);
        modelBuilder.Entity<Resource>().HasQueryFilter(e => e.TenantId == CurrentTenantId && !e.IsDeleted);
        modelBuilder.Entity<Webhook>().HasQueryFilter(e => e.TenantId == CurrentTenantId && !e.IsDeleted);

        // Global query filter for junction tables
        modelBuilder.Entity<UnitUser>().HasQueryFilter(e => e.TenantId == CurrentTenantId && !e.IsDeleted);
        modelBuilder.Entity<QueueService>().HasQueryFilter(e => e.TenantId == CurrentTenantId && !e.IsDeleted);
        modelBuilder.Entity<TicketStatusHistory>().HasQueryFilter(e => e.TenantId == CurrentTenantId && !e.IsDeleted);

        // Global query filter for soft delete on Tenant
        modelBuilder.Entity<Tenant>().HasQueryFilter(e => !e.IsDeleted);
    }

    /// <summary>
    /// Configures database indexes for performance
    /// </summary>
    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Composite indexes for multi-tenant queries
        modelBuilder.Entity<Unit>()
            .HasIndex(e => new { e.TenantId, e.Code })
            .IsUnique()
            .HasDatabaseName("ix_units_tenant_code");

        modelBuilder.Entity<User>()
            .HasIndex(e => new { e.TenantId, e.Email })
            .IsUnique()
            .HasDatabaseName("ix_users_tenant_email");

        modelBuilder.Entity<User>()
            .HasIndex(e => new { e.TenantId, e.EmployeeCode })
            .IsUnique()
            .HasDatabaseName("ix_users_tenant_employee_code");

        modelBuilder.Entity<Service>()
            .HasIndex(e => new { e.TenantId, e.Code })
            .IsUnique()
            .HasDatabaseName("ix_services_tenant_code");

        modelBuilder.Entity<Queue>()
            .HasIndex(e => new { e.TenantId, e.UnitId, e.Code })
            .IsUnique()
            .HasDatabaseName("ix_queues_tenant_unit_code");

        modelBuilder.Entity<Ticket>()
            .HasIndex(e => new { e.TenantId, e.QueueId, e.Number })
            .IsUnique()
            .HasDatabaseName("ix_tickets_tenant_queue_number");

        modelBuilder.Entity<Resource>()
            .HasIndex(e => new { e.TenantId, e.UnitId, e.Code })
            .IsUnique()
            .HasDatabaseName("ix_resources_tenant_unit_code");

        // Performance indexes
        modelBuilder.Entity<Ticket>()
            .HasIndex(e => new { e.TenantId, e.Status, e.Priority, e.IssuedAt })
            .HasDatabaseName("ix_tickets_tenant_status_priority_issued");

        modelBuilder.Entity<Session>()
            .HasIndex(e => new { e.TenantId, e.Status, e.StartedAt })
            .HasDatabaseName("ix_sessions_tenant_status_started");

        modelBuilder.Entity<UnitOperatingHour>()
            .HasIndex(e => new { e.TenantId, e.UnitId, e.DayOfWeek })
            .HasDatabaseName("ix_unit_operating_hours_tenant_unit_day");
    }

    /// <summary>
    /// Override SaveChanges to automatically set audit fields
    /// </summary>
    public override int SaveChanges()
    {
        SetAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically set audit fields
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically sets audit fields before saving
    /// </summary>
    private void SetAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.CreatedBy = CurrentUserId;
                entity.UpdatedBy = CurrentUserId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = CurrentUserId;
            }
        }
    }

    /// <summary>
    /// Sets the current tenant context
    /// </summary>
    public void SetTenantContext(Guid tenantId, string userId)
    {
        CurrentTenantId = tenantId;
        CurrentUserId = userId;
    }
}