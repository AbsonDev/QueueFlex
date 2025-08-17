namespace QueueManagement.Domain.Enums;

/// <summary>
/// Represents the role of a user in the system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Basic attendant role
    /// </summary>
    Attendant = 1,
    
    /// <summary>
    /// Supervisor role with oversight capabilities
    /// </summary>
    Supervisor = 2,
    
    /// <summary>
    /// Manager role with administrative capabilities
    /// </summary>
    Manager = 3,
    
    /// <summary>
    /// Administrator role with full system access
    /// </summary>
    Admin = 4
}