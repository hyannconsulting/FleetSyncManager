namespace Laroche.FleetManager.Application.Models;

/// <summary>
/// Statistiques des connexions pour le tableau de bord
/// </summary>
public class LoginStatistics
{
    public string Period { get; set; } = string.Empty;
    public int TotalAttempts { get; set; }
    public int SuccessfulLogins { get; set; }
    public int FailedAttempts { get; set; }
    public int UniqueUsers { get; set; }
    public int SuspiciousActivities { get; set; }
    public Dictionary<string, int> TopFailureReasons { get; set; } = new();
    public Dictionary<int, int> HourlyDistribution { get; set; } = new();
    
    public double SuccessRate => TotalAttempts > 0 ? (double)SuccessfulLogins / TotalAttempts * 100 : 0;
}
