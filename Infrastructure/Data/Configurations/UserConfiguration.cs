using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for User entity using Fluent API
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

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

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("email");

        builder.Property(u => u.EmployeeCode)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("employee_code");

        builder.Property(u => u.Role)
            .IsRequired()
            .HasColumnName("role")
            .HasConversion<int>();

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
            .WithMany(t => t.Users)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(u => new { u.TenantId, u.Email })
            .IsUnique()
            .HasDatabaseName("ix_users_tenant_email");

        builder.HasIndex(u => new { u.TenantId, u.EmployeeCode })
            .IsUnique()
            .HasDatabaseName("ix_users_tenant_employee_code");

        builder.HasIndex(u => u.Role)
            .HasDatabaseName("ix_users_role");

        builder.HasIndex(u => u.Status)
            .HasDatabaseName("ix_users_status");

        // Global query filter for multi-tenancy and soft delete
        builder.HasQueryFilter(u => u.TenantId == EF.Property<Guid>(u, "CurrentTenantId") && !u.IsDeleted);
    }
}