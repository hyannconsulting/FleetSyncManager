using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Application.DTOs;
/// <summary>
/// DTO pour la création d'un incident
/// </summary>
public class CreateIncidentDto
{
    /// <summary>Identifiant du véhicule</summary>
    public int VehicleId { get; set; }
    /// <summary>Identifiant du conducteur</summary>
    public int? DriverId { get; set; }
    /// <summary>Titre de l'incident</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>Description de l'incident</summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>Gravité</summary>
    public IncidentSeverityEnums Severity { get; set; }
    /// <summary>Date de l'incident</summary>
    public DateTime IncidentDate { get; set; }
    /// <summary>Localisation</summary>
    public string? Location { get; set; }
    /// <summary>Coût estimé</summary>
    public decimal? EstimatedCost { get; set; }
    /// <summary>Type d'incident</summary>
    public string IncidentType { get; set; } = string.Empty;
}

/// <summary>
/// DTO pour la mise à jour d'un incident
/// </summary>
public class UpdateIncidentDto : CreateIncidentDto
{
    /// <summary>Identifiant de l'incident</summary>
    public int Id { get; set; }
    /// <summary>Résolu</summary>
    public bool IsResolved { get; set; }
    /// <summary>Date de résolution</summary>
    public DateTime? ResolutionDate { get; set; }
    /// <summary>Coût réel</summary>
    public decimal? ActualCost { get; set; }
    /// <summary>Statut</summary>
    public string Status { get; set; } = "Open";
    /// <summary>Investigateur</summary>
    public string? InvestigatedBy { get; set; }
    /// <summary>Notes d'investigation</summary>
    public string? InvestigationNotes { get; set; }
}

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
