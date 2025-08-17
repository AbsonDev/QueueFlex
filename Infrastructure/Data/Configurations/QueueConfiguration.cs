using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for Queue entity using Fluent API
/// </summary>
public class QueueConfiguration : IEntityTypeConfiguration<Queue>
{
    public void Configure(EntityTypeBuilder<Queue> builder)
    {
        builder.ToTable("queues");

        // Primary key
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Properties
        builder.Property(q => q.TenantId)
            .IsRequired()
            .HasColumnName("tenant_id");

        builder.Property(q => q.UnitId)
            .IsRequired()
            .HasColumnName("unit_id");

        builder.Property(q => q.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(q => q.Code)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("code");

        builder.Property(q => q.DisplayName)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("display_name");

        builder.Property(q => q.MaxCapacity)
            .IsRequired()
            .HasColumnName("max_capacity");

        builder.Property(q => q.Status)
            .IsRequired()
            .HasColumnName("status")
            .HasConversion<int>();

        builder.Property(q => q.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        // Audit properties
        builder.Property(q => q.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(q => q.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(q => q.CreatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("created_by");

        builder.Property(q => q.UpdatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("updated_by");

        builder.Property(q => q.IsDeleted)
            .IsRequired()
            .HasColumnName("is_deleted");

        builder.Property(q => q.RowVersion)
            .HasColumnName("row_version")
            .IsRowVersion();

        // Relationships
        builder.HasOne(q => q.Tenant)
            .WithMany()
            .HasForeignKey(q => q.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(q => q.Unit)
            .WithMany(u => u.Queues)
            .HasForeignKey(q => q.UnitId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(q => new { q.TenantId, q.UnitId, q.Code })
            .IsUnique()
            .HasDatabaseName("ix_queues_tenant_unit_code");

        builder.HasIndex(q => q.Status)
            .HasDatabaseName("ix_queues_status");

        builder.HasIndex(q => q.IsActive)
            .HasDatabaseName("ix_queues_is_active");

        builder.HasIndex(q => q.MaxCapacity)
            .HasDatabaseName("ix_queues_max_capacity");

        // Global query filter for multi-tenancy and soft delete
        builder.HasQueryFilter(q => q.TenantId == EF.Property<Guid>(q, "CurrentTenantId") && !q.IsDeleted);
    }
}