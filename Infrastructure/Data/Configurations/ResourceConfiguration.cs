using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for Resource entity using Fluent API
/// </summary>
public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("resources");

        // Primary key
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Properties
        builder.Property(r => r.TenantId)
            .IsRequired()
            .HasColumnName("tenant_id");

        builder.Property(r => r.UnitId)
            .IsRequired()
            .HasColumnName("unit_id");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(r => r.Code)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("code");

        builder.Property(r => r.Type)
            .IsRequired()
            .HasColumnName("type")
            .HasConversion<int>();

        builder.Property(r => r.Location)
            .HasMaxLength(200)
            .HasColumnName("location");

        builder.Property(r => r.Status)
            .IsRequired()
            .HasColumnName("status")
            .HasConversion<int>();

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        // Audit properties
        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(r => r.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(r => r.CreatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("created_by");

        builder.Property(r => r.UpdatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("updated_by");

        builder.Property(r => r.IsDeleted)
            .IsRequired()
            .HasColumnName("is_deleted");

        builder.Property(r => r.RowVersion)
            .HasColumnName("row_version")
            .IsRowVersion();

        // Relationships
        builder.HasOne(r => r.Tenant)
            .WithMany()
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Unit)
            .WithMany(u => u.Resources)
            .HasForeignKey(r => r.UnitId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(r => new { r.TenantId, r.UnitId, r.Code })
            .IsUnique()
            .HasDatabaseName("ix_resources_tenant_unit_code");

        builder.HasIndex(r => r.Type)
            .HasDatabaseName("ix_resources_type");

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("ix_resources_status");

        builder.HasIndex(r => r.IsActive)
            .HasDatabaseName("ix_resources_is_active");

        // Global query filter for multi-tenancy and soft delete
        builder.HasQueryFilter(r => r.TenantId == EF.Property<Guid>(r, "CurrentTenantId") && !r.IsDeleted);
    }
}