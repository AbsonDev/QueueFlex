namespace QueueManagement.Domain.Enums;

/// <summary>
/// Represents the subscription plan for a tenant
/// </summary>
public enum TenantPlan
{
    /// <summary>
    /// Free tier with limited features
    /// </summary>
    Free = 1,
    
    /// <summary>
    /// Professional tier with enhanced features
    /// </summary>
    Pro = 2,
    
    /// <summary>
    /// Enterprise tier with full features and support
    /// </summary>
    Enterprise = 3
}