namespace DistributionManagement.Domain.Enums;
/// &lt;summary&gt;
/// Defines the user roles in the system matching WSO2 IS roles
/// &lt;/summary&gt;
public enum UserRole
{
/// &lt;summary&gt;
/// Admin role with full access
/// &lt;/summary&gt;
YksAdmin = 1,
/// &lt;summary&gt;
/// User role with edit and view access
/// &lt;/summary&gt;
YksUser = 2,
/// &lt;summary&gt;
/// Test role with view-only access
/// &lt;/summary&gt;
YksTest = 3
}