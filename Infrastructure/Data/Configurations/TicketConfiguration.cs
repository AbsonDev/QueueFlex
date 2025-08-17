using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for Ticket entity using Fluent API
/// </summary>
public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");

        // Primary key
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Properties
        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("tenant_id");

        builder.Property(t => t.Number)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("number");

        builder.Property(t => t.QueueId)
            .IsRequired()
            .HasColumnName("queue_id");

        builder.Property(t => t.ServiceId)
            .IsRequired()
            .HasColumnName("service_id");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasColumnName("status")
            .HasConversion<int>();

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasColumnName("priority")
            .HasConversion<int>();

        builder.Property(t => t.IssuedAt)
            .IsRequired()
            .HasColumnName("issued_at");

        builder.Property(t => t.CalledAt)
            .HasColumnName("called_at");

        builder.Property(t => t.StartedAt)
            .HasColumnName("started_at");

        builder.Property(t => t.CompletedAt)
            .HasColumnName("completed_at");

        builder.Property(t => t.CustomerName)
            .HasMaxLength(200)
            .HasColumnName("customer_name");

        builder.Property(t => t.CustomerDocument)
            .HasMaxLength(20)
            .HasColumnName("customer_document");

        builder.Property(t => t.CustomerPhone)
            .HasMaxLength(20)
            .HasColumnName("customer_phone");

        builder.Property(t => t.Notes)
            .HasMaxLength(1000)
            .HasColumnName("notes");

        builder.Property(t => t.CompletionNotes)
            .HasMaxLength(1000)
            .HasColumnName("completion_notes");

        // Audit properties
        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(t => t.CreatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("created_by");

        builder.Property(t => t.UpdatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("updated_by");

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasColumnName("is_deleted");

        builder.Property(t => t.RowVersion)
            .HasColumnName("row_version")
            .IsRowVersion();

        // Relationships
        builder.HasOne(t => t.Tenant)
            .WithMany()
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Queue)
            .WithMany(q => q.Tickets)
            .HasForeignKey(t => t.QueueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Service)
            .WithMany(s => s.Tickets)
            .HasForeignKey(t => t.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => new { t.TenantId, t.QueueId, t.Number })
            .IsUnique()
            .HasDatabaseName("ix_tickets_tenant_queue_number");

        builder.HasIndex(t => new { t.TenantId, t.Status, t.Priority, t.IssuedAt })
            .HasDatabaseName("ix_tickets_tenant_status_priority_issued");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("ix_tickets_status");

        builder.HasIndex(t => t.Priority)
            .HasDatabaseName("ix_tickets_priority");

        builder.HasIndex(t => t.IssuedAt)
            .HasDatabaseName("ix_tickets_issued_at");

        // Global query filter for multi-tenancy and soft delete
        builder.HasQueryFilter(t => t.TenantId == EF.Property<Guid>(t, "CurrentTenantId") && !t.IsDeleted);
    }
}