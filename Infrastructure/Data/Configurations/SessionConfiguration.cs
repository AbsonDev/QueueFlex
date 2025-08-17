using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for Session entity using Fluent API
/// </summary>
public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        // Primary key
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Properties
        builder.Property(s => s.TenantId)
            .IsRequired()
            .HasColumnName("tenant_id");

        builder.Property(s => s.TicketId)
            .IsRequired()
            .HasColumnName("ticket_id");

        builder.Property(s => s.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(s => s.ResourceId)
            .HasColumnName("resource_id");

        builder.Property(s => s.Status)
            .IsRequired()
            .HasColumnName("status")
            .HasConversion<int>();

        builder.Property(s => s.StartedAt)
            .IsRequired()
            .HasColumnName("started_at");

        builder.Property(s => s.PausedAt)
            .HasColumnName("paused_at");

        builder.Property(s => s.PausedDuration)
            .HasColumnName("paused_duration");

        builder.Property(s => s.CompletedAt)
            .HasColumnName("completed_at");

        builder.Property(s => s.CustomerRating)
            .HasColumnName("customer_rating");

        builder.Property(s => s.CustomerFeedback)
            .HasMaxLength(1000)
            .HasColumnName("customer_feedback");

        builder.Property(s => s.InternalNotes)
            .HasMaxLength(1000)
            .HasColumnName("internal_notes");

        // Audit properties
        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(s => s.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(s => s.CreatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("created_by");

        builder.Property(s => s.UpdatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("updated_by");

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasColumnName("is_deleted");

        builder.Property(s => s.RowVersion)
            .HasColumnName("row_version")
            .IsRowVersion();

        // Relationships
        builder.HasOne(s => s.Tenant)
            .WithMany()
            .HasForeignKey(s => s.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Ticket)
            .WithMany(t => t.Sessions)
            .HasForeignKey(s => s.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Resource)
            .WithMany(r => r.Sessions)
            .HasForeignKey(s => s.ResourceId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(s => new { s.TenantId, s.Status, s.StartedAt })
            .HasDatabaseName("ix_sessions_tenant_status_started");

        builder.HasIndex(s => s.TicketId)
            .HasDatabaseName("ix_sessions_ticket_id");

        builder.HasIndex(s => s.UserId)
            .HasDatabaseName("ix_sessions_user_id");

        builder.HasIndex(s => s.ResourceId)
            .HasDatabaseName("ix_sessions_resource_id");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("ix_sessions_status");

        // Global query filter for multi-tenancy and soft delete
        builder.HasQueryFilter(s => s.TenantId == EF.Property<Guid>(s, "CurrentTenantId") && !s.IsDeleted);
    }
}