using Laroche.FleetManager.Domain.Common;
using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Domain.Entities;

/// <summary>
/// Incident entity
/// </summary>
public class Incident : BaseEntity
{
    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int VehicleId { get; set; }
    
    /// <summary>
    /// Driver ID (optional)
    /// </summary>
    public int? DriverId { get; set; }
    
    /// <summary>
    /// Incident title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Incident description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Incident severity
    /// </summary>
    public IncidentSeverityEnums Severity { get; set; }
    
    /// <summary>
    /// Incident date and time
    /// </summary>
    public DateTime IncidentDate { get; set; }
    
    /// <summary>
    /// Location where incident occurred
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// GPS coordinates (latitude)
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// GPS coordinates (longitude)
    /// </summary>
    public double? Longitude { get; set; }
    
    /// <summary>
    /// Estimated cost of incident
    /// </summary>
    public decimal? EstimatedCost { get; set; }
    
    /// <summary>
    /// Actual cost of incident
    /// </summary>
    public decimal? ActualCost { get; set; }
    
    /// <summary>
    /// Insurance claim number
    /// </summary>
    public string? InsuranceClaimNumber { get; set; }
    
    /// <summary>
    /// Police report number
    /// </summary>
    public string? PoliceReportNumber { get; set; }
    
    /// <summary>
    /// Third party involved
    /// </summary>
    public bool ThirdPartyInvolved { get; set; }
    
    /// <summary>
    /// Third party details
    /// </summary>
    public string? ThirdPartyDetails { get; set; }
    
    /// <summary>
    /// Incident status (resolved/pending)
    /// </summary>
    public bool IsResolved { get; set; }
    
    /// <summary>
    /// Resolution date
    /// </summary>
    public DateTime? ResolutionDate { get; set; }
    
    /// <summary>
    /// Resolution notes
    /// </summary>
    public string? ResolutionNotes { get; set; }
    
    /// <summary>
    /// Navigation property to Vehicle
    /// </summary>
    public virtual Vehicle Vehicle { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to Driver
    /// </summary>
    public virtual Driver? Driver { get; set; }
}
