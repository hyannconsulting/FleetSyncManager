namespace Laroche.FleetManager.Application.DTOs;

/// <summary>
/// Incident statistics DTO
/// </summary>
public class IncidentStatisticsDto
{
    /// <summary>
    /// Total incidents
    /// </summary>
    public int TotalIncidents { get; set; }

    /// <summary>
    /// Open incidents
    /// </summary>
    public int OpenIncidents { get; set; }

    /// <summary>
    /// Resolved incidents
    /// </summary>
    public int ResolvedIncidents { get; set; }

    /// <summary>
    /// Total estimated cost
    /// </summary>
    public decimal TotalEstimatedCost { get; set; }

    /// <summary>
    /// Total actual cost
    /// </summary>
    public decimal TotalActualCost { get; set; }

    /// <summary>
    /// Average resolution time in days
    /// </summary>
    public double AverageResolutionDays { get; set; }

    /// <summary>
    /// Most common incident type
    /// </summary>
    public string? MostCommonType { get; set; }

    /// <summary>
    /// Incidents by severity
    /// </summary>
    public Dictionary<string, int> IncidentsBySeverity { get; set; } = new();

    /// <summary>
    /// Incidents by type
    /// </summary>
    public Dictionary<string, int> IncidentsByType { get; set; } = new();
}
