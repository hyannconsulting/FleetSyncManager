using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Application.DTOs;

/// <summary>
/// Incident DTO
/// </summary>
public class IncidentDto
{
    /// <summary>
    /// Incident ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int VehicleId { get; set; }

    /// <summary>
    /// Vehicle license plate
    /// </summary>
    public string VehicleLicensePlate { get; set; } = string.Empty;

    /// <summary>
    /// Driver ID
    /// </summary>
    public int? DriverId { get; set; }

    /// <summary>
    /// Driver name
    /// </summary>
    public string? DriverName { get; set; }

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
    /// Incident date
    /// </summary>
    public DateTime IncidentDate { get; set; }

    /// <summary>
    /// Location
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Estimated cost
    /// </summary>
    public decimal? EstimatedCost { get; set; }

    /// <summary>
    /// Actual cost
    /// </summary>
    public decimal? ActualCost { get; set; }

    /// <summary>
    /// Is resolved
    /// </summary>
    public bool IsResolved { get; set; }

    /// <summary>
    /// Resolution date
    /// </summary>
    public DateTime? ResolutionDate { get; set; }

    /// <summary>
    /// Incident type/category
    /// </summary>
    public string IncidentType { get; set; } = string.Empty;

    /// <summary>
    /// Incident status
    /// </summary>
    public string Status { get; set; } = "Open";

    /// <summary>
    /// Alias for ResolutionDate (for compatibility)
    /// </summary>
    public DateTime? ResolvedDate => ResolutionDate;

    /// <summary>
    /// Person who investigated the incident
    /// </summary>
    public string? InvestigatedBy { get; set; }

    /// <summary>
    /// Investigation notes
    /// </summary>
    public string? InvestigationNotes { get; set; }
}
