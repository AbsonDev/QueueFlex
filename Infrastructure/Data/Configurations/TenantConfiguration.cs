using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for Tenant entity using Fluent API
/// </summary>
public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        // Primary key
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Properties
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(t => t.Subdomain)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("subdomain");

        builder.Property(t => t.TimeZone)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("time_zone");

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        builder.Property(t => t.Plan)
            .IsRequired()
            .HasColumnName("plan")
            .HasConversion<int>();

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

        // Indexes
        builder.HasIndex(t => t.Subdomain)
            .IsUnique()
            .HasDatabaseName("ix_tenants_subdomain");

        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("ix_tenants_is_active");

        builder.HasIndex(t => t.Plan)
            .HasDatabaseName("ix_tenants_plan");

        // Global query filter for soft delete
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}