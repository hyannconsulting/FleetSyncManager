namespace Laroche.FleetManager.Application.DTOs;

/// <summary>
/// Maintenance statistics DTO
/// </summary>
public class MaintenanceStatisticsDto
{
    /// <summary>
    /// Total maintenance records
    /// </summary>
    public int TotalMaintenances { get; set; }

    /// <summary>
    /// Completed maintenances
    /// </summary>
    public int CompletedMaintenances { get; set; }

    /// <summary>
    /// Pending maintenances
    /// </summary>
    public int PendingMaintenances { get; set; }

    /// <summary>
    /// Overdue maintenances
    /// </summary>
    public int OverdueMaintenances { get; set; }

    /// <summary>
    /// Total cost
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Average cost per maintenance
    /// </summary>
    public decimal AverageCost { get; set; }

    /// <summary>
    /// Most common maintenance type
    /// </summary>
    public string? MostCommonType { get; set; }

    /// <summary>
    /// Maintenance by type
    /// </summary>
    public Dictionary<string, int> MaintenanceByType { get; set; } = new();
}
