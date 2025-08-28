using Laroche.FleetManager.Domain.Common;

namespace Laroche.FleetManager.Domain.Entities;

/// <summary>
/// Vehicle assignment to driver
/// </summary>
public class VehicleAssignment : BaseEntity
{
    /// <summary>
    /// Driver ID
    /// </summary>
    public int DriverId { get; set; }
    
    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int VehicleId { get; set; }
    
    /// <summary>
    /// Assignment start date
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Assignment end date
    /// </summary>
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// Assignment notes
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Is primary assignment
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Navigation property to Driver
    /// </summary>
    public virtual Driver Driver { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to Vehicle
    /// </summary>
    public virtual Vehicle Vehicle { get; set; } = null!;
    
    /// <summary>
    /// Check if assignment is active
    /// </summary>
    public bool IsActive => EndDate == null || EndDate > DateTime.UtcNow;
}
