using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for Webhook entity using Fluent API
/// </summary>
public class WebhookConfiguration : IEntityTypeConfiguration<Webhook>
{
    public void Configure(EntityTypeBuilder<Webhook> builder)
    {
        builder.ToTable("webhooks");

        // Primary key
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Properties
        builder.Property(w => w.TenantId)
            .IsRequired()
            .HasColumnName("tenant_id");

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(w => w.Url)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("url");

        builder.Property(w => w.Secret)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("secret");

        builder.Property(w => w.Events)
            .IsRequired()
            .HasMaxLength(1000)
            .HasColumnName("events");

        builder.Property(w => w.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        builder.Property(w => w.RetryCount)
            .IsRequired()
            .HasColumnName("retry_count");

        builder.Property(w => w.LastTriggeredAt)
            .HasColumnName("last_triggered_at");

        // Audit properties
        builder.Property(w => w.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(w => w.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(w => w.CreatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("created_by");

        builder.Property(w => w.UpdatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("updated_by");

        builder.Property(w => w.IsDeleted)
            .IsRequired()
            .HasColumnName("is_deleted");

        builder.Property(w => w.RowVersion)
            .HasColumnName("row_version")
            .IsRowVersion();

        // Relationships
        builder.HasOne(w => w.Tenant)
            .WithMany(t => t.Webhooks)
            .HasForeignKey(w => w.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(w => w.TenantId)
            .HasDatabaseName("ix_webhooks_tenant_id");

        builder.HasIndex(w => w.IsActive)
            .HasDatabaseName("ix_webhooks_is_active");

        builder.HasIndex(w => w.LastTriggeredAt)
            .HasDatabaseName("ix_webhooks_last_triggered");

        // Global query filter for multi-tenancy and soft delete
        builder.HasQueryFilter(w => w.TenantId == EF.Property<Guid>(w, "CurrentTenantId") && !w.IsDeleted);
    }
}