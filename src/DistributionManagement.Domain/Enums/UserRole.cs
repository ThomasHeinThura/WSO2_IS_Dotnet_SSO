namespace DistributionManagement.Domain.Enums;

/// <summary>
/// Defines the user roles in the system matching WSO2 IS roles
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Admin role with full access
    /// </summary>
    YksAdmin = 1,
    
    /// <summary>
    /// User role with edit and view access
    /// </summary>
    YksUser = 2,
    
    /// <summary>
    /// Test role with view-only access
    /// </summary>
    YksTest = 3
}