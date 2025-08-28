namespace Laroche.FleetManager.Domain.Enums;

/// <summary>
/// User roles in the system
/// </summary>
public enum UserRoleEnums
{
    /// <summary>
    /// System administrator
    /// </summary>
    Administrator = 1,

    /// <summary>
    /// Fleet manager
    /// </summary>
    FleetManager = 2,

    /// <summary>
    /// Driver
    /// </summary>
    Driver = 3,

    /// <summary>
    /// Maintenance technician
    /// </summary>
    MaintenanceTechnician = 4,

    /// <summary>
    /// Read-only user
    /// </summary>
    ReadOnlyUser = 5
}
