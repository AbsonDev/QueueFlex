using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for Service entity using Fluent API
/// </summary>
public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services");

        // Primary key
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Properties
        builder.Property(s => s.TenantId)
            .IsRequired()
            .HasColumnName("tenant_id");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("code");

        builder.Property(s => s.Description)
            .HasMaxLength(1000)
            .HasColumnName("description");

        builder.Property(s => s.EstimatedDurationMinutes)
            .IsRequired()
            .HasColumnName("estimated_duration_minutes");

        builder.Property(s => s.Color)
            .IsRequired()
            .HasMaxLength(7)
            .HasColumnName("color");

        builder.Property(s => s.RequiresResource)
            .IsRequired()
            .HasColumnName("requires_resource");

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        builder.Property(s => s.ConfigurationJson)
            .HasMaxLength(4000)
            .HasColumnName("configuration_json");

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
            .WithMany(t => t.Services)
            .HasForeignKey(s => s.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(s => new { s.TenantId, s.Code })
            .IsUnique()
            .HasDatabaseName("ix_services_tenant_code");

        builder.HasIndex(s => s.IsActive)
            .HasDatabaseName("ix_services_is_active");

        builder.HasIndex(s => s.RequiresResource)
            .HasDatabaseName("ix_services_requires_resource");

        // Global query filter for multi-tenancy and soft delete
        builder.HasQueryFilter(s => s.TenantId == EF.Property<Guid>(s, "CurrentTenantId") && !s.IsDeleted);
    }
}