using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for Unit entity using Fluent API
/// </summary>
public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("units");

        // Primary key
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Properties
        builder.Property(u => u.TenantId)
            .IsRequired()
            .HasColumnName("tenant_id");

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(u => u.Code)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("code");

        builder.Property(u => u.Status)
            .IsRequired()
            .HasColumnName("status")
            .HasConversion<int>();

        // Audit properties
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(u => u.CreatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("created_by");

        builder.Property(u => u.UpdatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("updated_by");

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasColumnName("is_deleted");

        builder.Property(u => u.RowVersion)
            .HasColumnName("row_version")
            .IsRowVersion();

        // Relationships
        builder.HasOne(u => u.Tenant)
            .WithMany(t => t.Units)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(u => new { u.TenantId, u.Code })
            .IsUnique()
            .HasDatabaseName("ix_units_tenant_code");

        builder.HasIndex(u => u.Status)
            .HasDatabaseName("ix_units_status");

        // Global query filter for multi-tenancy and soft delete
        builder.HasQueryFilter(u => u.TenantId == EF.Property<Guid>(u, "CurrentTenantId") && !u.IsDeleted);
    }
}