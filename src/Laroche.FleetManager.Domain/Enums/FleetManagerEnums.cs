namespace Laroche.FleetManager.Domain.Enums;

/// <summary>
/// Vehicle fuel types
/// </summary>
public enum FuelType
{
    /// <summary>
    /// Gasoline fuel
    /// </summary>
    Gasoline = 1,
    
    /// <summary>
    /// Diesel fuel
    /// </summary>
    Diesel = 2,
    
    /// <summary>
    /// Electric vehicle
    /// </summary>
    Electric = 3,
    
    /// <summary>
    /// Hybrid vehicle
    /// </summary>
    Hybrid = 4,
    
    /// <summary>
    /// LPG (Liquefied Petroleum Gas)
    /// </summary>
    LPG = 5,
    
    /// <summary>
    /// CNG (Compressed Natural Gas)
    /// </summary>
    CNG = 6
}

/// <summary>
/// Vehicle status enumeration
/// </summary>
public enum VehicleStatus
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

/// <summary>
/// Driver license types
/// </summary>
public enum LicenseType
{
    /// <summary>
    /// Standard car license
    /// </summary>
    B = 1,
    
    /// <summary>
    /// Truck license
    /// </summary>
    C = 2,
    
    /// <summary>
    /// Heavy truck license
    /// </summary>
    CE = 3,
    
    /// <summary>
    /// Bus license
    /// </summary>
    D = 4,
    
    /// <summary>
    /// Professional driver license
    /// </summary>
    Professional = 5
}

/// <summary>
/// Maintenance types
/// </summary>
public enum MaintenanceType
{
    /// <summary>
    /// Preventive maintenance
    /// </summary>
    Preventive = 1,
    
    /// <summary>
    /// Corrective maintenance
    /// </summary>
    Corrective = 2,
    
    /// <summary>
    /// Emergency repair
    /// </summary>
    Emergency = 3,
    
    /// <summary>
    /// Periodic inspection
    /// </summary>
    Inspection = 4
}

/// <summary>
/// Incident severity levels
/// </summary>
public enum IncidentSeverity
{
    /// <summary>
    /// Low severity incident
    /// </summary>
    Low = 1,
    
    /// <summary>
    /// Medium severity incident
    /// </summary>
    Medium = 2,
    
    /// <summary>
    /// High severity incident
    /// </summary>
    High = 3,
    
    /// <summary>
    /// Critical incident
    /// </summary>
    Critical = 4
}

/// <summary>
/// User roles in the system
/// </summary>
public enum UserRole
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
