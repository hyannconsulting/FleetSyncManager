using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Application.Models;
using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Domain.Enums;
using Laroche.FleetManager.Infrastructure.Data;

namespace Laroche.FleetManager.Infrastructure.Services;

/// <summary>
/// Service d'audit des connexions selon TASK-002
/// Détection automatique d'activités suspectes et gestion complète des logs
/// </summary>
public class LoginAuditService : ILoginAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LoginAuditService> _logger;

    public LoginAuditService(ApplicationDbContext context, ILogger<LoginAuditService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<LoginAudit> LogLoginAttemptAsync(string? userId, string email, LoginResult result, 
        string ipAddress, string userAgent, string? sessionId = null)
    {
        try
        {
            var loginAudit = new LoginAudit
            {
                UserId = userId,
                EmailAttempted = email,
                Result = result,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                AttemptedAt = DateTime.UtcNow,
                SessionId = sessionId,
                IsSuspicious = false // Sera mis à jour par DetectSuspiciousActivity si nécessaire
            };

            _context.LoginAudits.Add(loginAudit);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Tentative de connexion loggée: {Email} depuis {IpAddress} avec résultat {Result}", 
                email, ipAddress, result);

            return loginAudit;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'enregistrement de tentative de connexion pour {Email}", email);
            throw;
        }
    }

    public async Task<LoginAudit> LogLogoutAsync(string email, string ipAddress, string userAgent, bool isSuccess, string reason)
    {
        try
        {
            var loginAudit = new LoginAudit
            {
                UserId = null, // Sera résolu plus tard si nécessaire
                EmailAttempted = email,
                Result = isSuccess ? LoginResult.Logout : LoginResult.SystemError,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                AttemptedAt = DateTime.UtcNow,
                SessionId = null,
                ResultMessage = reason,
                IsSuspicious = false
            };

            _context.LoginAudits.Add(loginAudit);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Déconnexion loggée: {Email} depuis {IpAddress} - {Reason}", 
                email, ipAddress, reason);

            return loginAudit;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'enregistrement de déconnexion pour {Email}", email);
            throw;
        }
    }

    public async Task<bool> LogSessionEndAsync(string sessionId, string endReason)
    {
        try
        {
            var session = await _context.LoginAudits
                .Where(la => la.SessionId == sessionId && la.SessionEndAt == null)
                .FirstOrDefaultAsync();

            if (session != null)
            {
                session.SessionEndAt = DateTime.UtcNow;
                session.SessionDuration = (int)(session.SessionEndAt.Value - session.AttemptedAt).TotalMinutes;
                session.ResultMessage = endReason;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Session {SessionId} terminée: {EndReason}", sessionId, endReason);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la fin de session {SessionId}", sessionId);
            return false;
        }
    }

    public async Task<bool> DetectSuspiciousActivityAsync(string userId, string ipAddress, TimeSpan? timeWindow = null)
    {
        var window = timeWindow ?? TimeSpan.FromHours(24);
        var cutoffTime = DateTime.UtcNow.Subtract(window);

        try
        {
            // Vérifier si nouvelle IP pour cet utilisateur
            var knownIps = await _context.LoginAudits
                .Where(la => la.UserId == userId && la.Result == LoginResult.Success)
                .Select(la => la.IpAddress)
                .Distinct()
                .ToListAsync();

            bool isNewIp = !knownIps.Contains(ipAddress);

            // Compter les échecs récents depuis cette IP
            var recentFailuresFromIp = await _context.LoginAudits
                .Where(la => la.IpAddress == ipAddress && 
                           la.AttemptedAt >= cutoffTime &&
                           la.Result != LoginResult.Success)
                .CountAsync();

            // Compter les tentatives multiples rapides
            var rapidAttempts = await _context.LoginAudits
                .Where(la => la.UserId == userId &&
                           la.AttemptedAt >= DateTime.UtcNow.AddMinutes(-5))
                .CountAsync();

            // Détecter géolocalisation improbable (simplifiée - basée sur IP différentes dans temps court)
            var recentIps = await _context.LoginAudits
                .Where(la => la.UserId == userId &&
                           la.AttemptedAt >= DateTime.UtcNow.AddHours(-2) &&
                           la.Result == LoginResult.Success)
                .Select(la => la.IpAddress)
                .Distinct()
                .ToListAsync();

            bool improbableLocation = recentIps.Count > 1 && recentIps.Contains(ipAddress);

            // Critères de suspicion selon TASK-002
            bool isSuspicious = isNewIp || recentFailuresFromIp >= 3 || rapidAttempts >= 3 || improbableLocation;

            if (isSuspicious)
            {
                _logger.LogWarning("Activité suspecte détectée pour utilisateur {UserId} depuis {IpAddress}: " +
                    "NouvelleIP={IsNewIp}, EchecsRecents={RecentFailures}, TentativesRapides={RapidAttempts}",
                    userId, ipAddress, isNewIp, recentFailuresFromIp, rapidAttempts);
            }

            return isSuspicious;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la détection d'activité suspecte pour {UserId}", userId);
            return false; // En cas d'erreur, ne pas bloquer l'authentification
        }
    }

    public async Task<bool> MarkAsSuspiciousAsync(int loginAuditId, string reason)
    {
        try
        {
            var loginAudit = await _context.LoginAudits.FindAsync(loginAuditId);
            if (loginAudit != null)
            {
                loginAudit.IsSuspicious = true;
                loginAudit.ResultMessage = reason;
                await _context.SaveChangesAsync();

                _logger.LogWarning("Tentative marquée comme suspecte: ID {LoginAuditId}, Raison: {Reason}", 
                    loginAuditId, reason);
                
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du marquage comme suspect pour {LoginAuditId}", loginAuditId);
            return false;
        }
    }

    public async Task<(IEnumerable<LoginAudit> LoginHistory, int TotalCount)> GetUserLoginHistoryAsync(
        string userId, int pageNumber = 1, int pageSize = 20, bool onlySuccessful = false)
    {
        try
        {
            var query = _context.LoginAudits
                .Where(la => la.UserId == userId)
                .AsQueryable();

            if (onlySuccessful)
            {
                query = query.Where(la => la.Result == LoginResult.Success);
            }

            var totalCount = await query.CountAsync();

            var history = await query
                .OrderByDescending(la => la.AttemptedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (history, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'historique pour {UserId}", userId);
            return (Enumerable.Empty<LoginAudit>(), 0);
        }
    }

    public async Task<(IEnumerable<LoginAudit> SuspiciousAttempts, int TotalCount)> GetSuspiciousAttemptsAsync(
        int hoursBack = 24, int pageNumber = 1, int pageSize = 50)
    {
        try
        {
            var cutoffTime = DateTime.UtcNow.AddHours(-hoursBack);

            var query = _context.LoginAudits
                .Where(la => la.AttemptedAt >= cutoffTime && la.IsSuspicious)
                .Include(la => la.User);

            var totalCount = await query.CountAsync();

            var suspiciousAttempts = await query
                .OrderByDescending(la => la.AttemptedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (suspiciousAttempts, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des tentatives suspectes");
            return (Enumerable.Empty<LoginAudit>(), 0);
        }
    }

    public async Task<LoginStatistics> GetLoginStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var query = _context.LoginAudits
                .Where(la => la.AttemptedAt >= startDate && la.AttemptedAt <= endDate);

            var totalAttempts = await query.CountAsync();
            var successfulLogins = await query.CountAsync(la => la.Result == LoginResult.Success);
            var failedAttempts = totalAttempts - successfulLogins;
            var uniqueUsers = await query.Select(la => la.UserId).Distinct().CountAsync();
            var suspiciousActivities = await query.CountAsync(la => la.IsSuspicious);

            // Top raisons d'échec
            var topFailureReasons = await query
                .Where(la => la.Result != LoginResult.Success)
                .GroupBy(la => la.Result)
                .Select(g => new { Reason = g.Key.ToString(), Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToDictionaryAsync(x => x.Reason, x => x.Count);

            // Distribution horaire
            var hourlyDistribution = await query
                .GroupBy(la => la.AttemptedAt.Hour)
                .Select(g => new { Hour = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Hour, x => x.Count);

            return new LoginStatistics
            {
                Period = $"{startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}",
                TotalAttempts = totalAttempts,
                SuccessfulLogins = successfulLogins,
                FailedAttempts = failedAttempts,
                UniqueUsers = uniqueUsers,
                SuspiciousActivities = suspiciousActivities,
                TopFailureReasons = topFailureReasons,
                HourlyDistribution = hourlyDistribution
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du calcul des statistiques de connexion");
            return new LoginStatistics(); // Retourner statistiques vides
        }
    }

    public async Task<IEnumerable<LoginAudit>> GetActiveSessionsAsync(TimeSpan? maxSessionAge = null)
    {
        try
        {
            var maxAge = maxSessionAge ?? TimeSpan.FromMinutes(30); // 30min selon TASK-002
            var cutoffTime = DateTime.UtcNow.Subtract(maxAge);

            var activeSessions = await _context.LoginAudits
                .Where(la => la.Result == LoginResult.Success &&
                           la.SessionId != null &&
                           la.SessionEndAt == null &&
                           la.AttemptedAt >= cutoffTime)
                .Include(la => la.User)
                .OrderByDescending(la => la.AttemptedAt)
                .ToListAsync();

            return activeSessions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des sessions actives");
            return Enumerable.Empty<LoginAudit>();
        }
    }

    public async Task<int> ForceLogoutAllSessionsAsync(string userId, string reason)
    {
        try
        {
            var activeSessions = await _context.LoginAudits
                .Where(la => la.UserId == userId &&
                           la.SessionId != null &&
                           la.SessionEndAt == null)
                .ToListAsync();

            foreach (var session in activeSessions)
            {
                session.SessionEndAt = DateTime.UtcNow;
                session.SessionDuration = (int)(session.SessionEndAt.Value - session.AttemptedAt).TotalMinutes;
                session.ResultMessage = reason;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Déconnexion forcée de {SessionCount} sessions pour utilisateur {UserId}: {Reason}",
                activeSessions.Count, userId, reason);

            return activeSessions.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la déconnexion forcée pour {UserId}", userId);
            return 0;
        }
    }

    public async Task<int> CleanupOldAuditLogsAsync(int olderThanDays = 365)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);

            var oldLogs = await _context.LoginAudits
                .Where(la => la.AttemptedAt < cutoffDate)
                .ToListAsync();

            _context.LoginAudits.RemoveRange(oldLogs);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Nettoyage des logs d'audit: {Count} entrées supprimées (plus anciennes que {Days} jours)",
                oldLogs.Count, olderThanDays);

            return oldLogs.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du nettoyage des logs d'audit");
            return 0;
        }
    }

    public async Task<bool> HasExceededFailedAttemptsAsync(string userId, TimeSpan? timeWindow = null, int maxAttempts = 5)
    {
        try
        {
            var window = timeWindow ?? TimeSpan.FromMinutes(15); // 15min selon TASK-002
            var cutoffTime = DateTime.UtcNow.Subtract(window);

            var failedAttempts = await _context.LoginAudits
                .Where(la => la.UserId == userId &&
                           la.AttemptedAt >= cutoffTime &&
                           la.Result != LoginResult.Success)
                .CountAsync();

            return failedAttempts >= maxAttempts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification des tentatives échouées pour {UserId}", userId);
            return false; // En cas d'erreur, ne pas bloquer
        }
    }

    public async Task<IEnumerable<(string IpAddress, int Count, DateTime LastUsed)>> GetUserTopIpAddressesAsync(
        string userId, int topCount = 10)
    {
        try
        {
            var topIps = await _context.LoginAudits
                .Where(la => la.UserId == userId && la.Result == LoginResult.Success)
                .GroupBy(la => la.IpAddress)
                .Select(g => new 
                {
                    IpAddress = g.Key,
                    Count = g.Count(),
                    LastUsed = g.Max(la => la.AttemptedAt)
                })
                .OrderByDescending(x => x.Count)
                .Take(topCount)
                .ToListAsync();

            return topIps.Select(ip => (ip.IpAddress, ip.Count, ip.LastUsed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des IP principales pour {UserId}", userId);
            return Enumerable.Empty<(string, int, DateTime)>();
        }
    }
}
