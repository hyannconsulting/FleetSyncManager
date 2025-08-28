using MediatR;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Application.Commands.Incidents;

/// <summary>
/// Command to create a new incident
/// </summary>
public class CreateIncidentCommand : IRequest<Result<IncidentDto>>
{
    /// <summary>
    /// Vehicle ID involved in the incident
    /// </summary>
    public int VehicleId { get; set; }
    
    /// <summary>
    /// Driver ID involved in the incident (optional if no driver assigned)
    /// </summary>
    public int? DriverId { get; set; }
    
    /// <summary>
    /// Type of incident
    /// </summary>
    public string IncidentType { get; set; } = string.Empty;
    
    /// <summary>
    /// Severity level (Low, Medium, High, Critical)
    /// </summary>
    public string Severity { get; set; } = "Medium";
    
    /// <summary>
    /// Date and time when the incident occurred
    /// </summary>
    public DateTime IncidentDate { get; set; }
    
    /// <summary>
    /// Location where the incident occurred
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Detailed description of the incident
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Weather conditions at the time
    /// </summary>
    public string? WeatherConditions { get; set; }
    
    /// <summary>
    /// Road conditions at the time
    /// </summary>
    public string? RoadConditions { get; set; }
    
    /// <summary>
    /// Whether police were involved
    /// </summary>
    public bool PoliceInvolved { get; set; }
    
    /// <summary>
    /// Police report number if applicable
    /// </summary>
    public string? PoliceReportNumber { get; set; }
    
    /// <summary>
    /// Whether there were injuries
    /// </summary>
    public bool InjuriesReported { get; set; }
    
    /// <summary>
    /// Estimated damage cost
    /// </summary>
    public decimal? EstimatedDamageCost { get; set; }
    
    /// <summary>
    /// Insurance claim number
    /// </summary>
    public string? InsuranceClaimNumber { get; set; }
    
    /// <summary>
    /// Witness information
    /// </summary>
    public string? WitnessInformation { get; set; }
    
    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Incident status
    /// </summary>
    public string Status { get; set; } = "Open";

    /// <summary>
    /// Alias for EstimatedDamageCost (for compatibility)
    /// </summary>
    public decimal? EstimatedCost
    {
        get => EstimatedDamageCost;
        set => EstimatedDamageCost = value;
    }

    /// <summary>
    /// Date when incident was resolved
    /// </summary>
    public DateTime? ResolvedDate { get; set; }

    /// <summary>
    /// Person who investigated the incident
    /// </summary>
    public string? InvestigatedBy { get; set; }

    /// <summary>
    /// Investigation notes
    /// </summary>
    public string? InvestigationNotes { get; set; }
}

/// <summary>
/// Command to update an existing incident
/// </summary>
public class UpdateIncidentCommand : IRequest<Result<IncidentDto>>
{
    /// <summary>
    /// Incident ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Type of incident
    /// </summary>
    public string IncidentType { get; set; } = string.Empty;
    
    /// <summary>
    /// Severity level (Low, Medium, High, Critical)
    /// </summary>
    public string Severity { get; set; } = "Medium";
    
    /// <summary>
    /// Date and time when the incident occurred
    /// </summary>
    public DateTime IncidentDate { get; set; }
    
    /// <summary>
    /// Location where the incident occurred
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Detailed description of the incident
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Weather conditions at the time
    /// </summary>
    public string? WeatherConditions { get; set; }
    
    /// <summary>
    /// Road conditions at the time
    /// </summary>
    public string? RoadConditions { get; set; }
    
    /// <summary>
    /// Whether police were involved
    /// </summary>
    public bool PoliceInvolved { get; set; }
    
    /// <summary>
    /// Police report number if applicable
    /// </summary>
    public string? PoliceReportNumber { get; set; }
    
    /// <summary>
    /// Whether there were injuries
    /// </summary>
    public bool InjuriesReported { get; set; }
    
    /// <summary>
    /// Estimated damage cost
    /// </summary>
    public decimal? EstimatedDamageCost { get; set; }
    
    /// <summary>
    /// Actual repair cost
    /// </summary>
    public decimal? ActualRepairCost { get; set; }
    
    /// <summary>
    /// Insurance claim number
    /// </summary>
    public string? InsuranceClaimNumber { get; set; }
    
    /// <summary>
    /// Witness information
    /// </summary>
    public string? WitnessInformation { get; set; }
    
    /// <summary>
    /// Status of the incident
    /// </summary>
    public string Status { get; set; } = "Open";
    
    /// <summary>
    /// Resolution date if resolved
    /// </summary>
    public DateTime? ResolvedDate { get; set; }
    
    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Vehicle ID involved in the incident
    /// </summary>
    public int VehicleId { get; set; }

    /// <summary>
    /// Driver ID involved in the incident
    /// </summary>
    public int? DriverId { get; set; }

    /// <summary>
    /// Alias for EstimatedDamageCost (for compatibility)
    /// </summary>
    public decimal? EstimatedCost
    {
        get => EstimatedDamageCost;
        set => EstimatedDamageCost = value;
    }

    /// <summary>
    /// Person who investigated the incident
    /// </summary>
    public string? InvestigatedBy { get; set; }

    /// <summary>
    /// Investigation notes
    /// </summary>
    public string? InvestigationNotes { get; set; }
}

/// <summary>
/// Command to delete an incident
/// </summary>
public class DeleteIncidentCommand : IRequest<Result>
{
    /// <summary>
    /// Incident ID to delete
    /// </summary>
    public int Id { get; set; }
    
    public DeleteIncidentCommand(int id)
    {
        Id = id;
    }
}

/// <summary>
/// Command to resolve an incident
/// </summary>
public class ResolveIncidentCommand : IRequest<Result<IncidentDto>>
{
    /// <summary>
    /// Incident ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Resolution date
    /// </summary>
    public DateTime ResolvedDate { get; set; }
    
    /// <summary>
    /// Actual repair cost
    /// </summary>
    public decimal? ActualRepairCost { get; set; }
    
    /// <summary>
    /// Resolution notes
    /// </summary>
    public string? ResolutionNotes { get; set; }
    
    public ResolveIncidentCommand(int id)
    {
        Id = id;
        ResolvedDate = DateTime.Now;
    }
}
