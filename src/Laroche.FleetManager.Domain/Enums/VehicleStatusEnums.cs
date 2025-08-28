namespace Laroche.FleetManager.Domain.Enums;

/// <summary>
/// Vehicle status enumeration
/// </summary>
public enum VehicleStatusEnums
{
    /// <summary>
    /// Vehicle is active and available
    /// </summary>
    Active = 1,

    /// <summary>
    /// Vehicle is under maintenance
    /// </summary>
    Maintenance = 2,

    /// <summary>
    /// Vehicle is out of service
    /// </summary>
    OutOfService = 3,

    /// <summary>
    /// Vehicle is sold or disposed
    /// </summary>
    Disposed = 4
}