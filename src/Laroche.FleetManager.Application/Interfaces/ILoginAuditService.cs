using Laroche.FleetManager.Application.Models;
using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Application.Interfaces;

/// <summary>
/// Service d'audit des connexions et de détection d'activité suspecte
/// Selon TASK-002: tracking complet avec détection automatique des anomalies
/// </summary>
public interface ILoginAuditService
{
    /// <summary>
    /// Enregistre une tentative de connexion (succès ou échec)
    /// </summary>
    /// <param name="userId">ID utilisateur (null si utilisateur inexistant)</param>
    /// <param name="email">Email saisi lors de la tentative</param>
    /// <param name="result">Résultat de la tentative</param>
    /// <param name="ipAddress">Adresse IP source</param>
    /// <param name="userAgent">User-Agent du navigateur</param>
    /// <param name="sessionId">ID de session générée (si connexion réussie)</param>
    /// <returns>LoginAudit créé</returns>
    Task<LoginAudit> LogLoginAttemptAsync(string? userId, string email, LoginResult result, 
        string ipAddress, string userAgent, string? sessionId = null);

    /// <summary>
    /// Enregistre une déconnexion utilisateur
    /// </summary>
    /// <param name="email">Email de l'utilisateur</param>
    /// <param name="ipAddress">Adresse IP source</param>
    /// <param name="userAgent">User-Agent du navigateur</param>
    /// <param name="isSuccess">Indique si la déconnexion a réussi</param>
    /// <param name="reason">Raison de la déconnexion</param>
    /// <returns>LoginAudit créé pour la déconnexion</returns>
    Task<LoginAudit> LogLogoutAsync(string email, string ipAddress, string userAgent, bool isSuccess, string reason);

    /// <summary>
    /// Enregistre la fin de session utilisateur
    /// </summary>
    /// <param name="sessionId">ID de session à terminer</param>
    /// <param name="endReason">Raison de fin: Logout, Timeout, ForceLogout</param>
    /// <returns>True si session terminée</returns>
    Task<bool> LogSessionEndAsync(string sessionId, string endReason);

    /// <summary>
    /// Détecte automatiquement les activités suspectes
    /// - Tentatives depuis nouvelles IP
    /// - Échecs multiples rapides
    /// - Connexions géographiquement improbables
    /// </summary>
    /// <param name="userId">ID utilisateur à analyser</param>
    /// <param name="ipAddress">IP actuelle</param>
    /// <param name="timeWindow">Fenêtre d'analyse (défaut: 24h)</param>
    /// <returns>True si activité suspecte détectée</returns>
    Task<bool> DetectSuspiciousActivityAsync(string userId, string ipAddress, 
        TimeSpan? timeWindow = null);

    /// <summary>
    /// Marque une tentative comme suspecte et déclenche alertes
    /// </summary>
    /// <param name="loginAuditId">ID du LoginAudit à marquer</param>
    /// <param name="reason">Raison de la suspicion</param>
    /// <returns>True si marquage effectué</returns>
    Task<bool> MarkAsSuspiciousAsync(int loginAuditId, string reason);

    /// <summary>
    /// Récupère l'historique des connexions d'un utilisateur
    /// </summary>
    /// <param name="userId">ID utilisateur</param>
    /// <param name="pageNumber">Numéro de page (défaut: 1)</param>
    /// <param name="pageSize">Taille de page (défaut: 20)</param>
    /// <param name="onlySuccessful">Filtrer uniquement les succès</param>
    /// <returns>Historique paginé</returns>
    Task<(IEnumerable<LoginAudit> LoginHistory, int TotalCount)> GetUserLoginHistoryAsync(
        string userId, int pageNumber = 1, int pageSize = 20, bool onlySuccessful = false);

    /// <summary>
    /// Récupère toutes les tentatives suspectes récentes
    /// </summary>
    /// <param name="hoursBack">Nombre d'heures à analyser (défaut: 24h)</param>
    /// <param name="pageNumber">Numéro de page</param>
    /// <param name="pageSize">Taille de page</param>
    /// <returns>Liste des tentatives suspectes</returns>
    Task<(IEnumerable<LoginAudit> SuspiciousAttempts, int TotalCount)> GetSuspiciousAttemptsAsync(
        int hoursBack = 24, int pageNumber = 1, int pageSize = 50);

    /// <summary>
    /// Génère les statistiques de connexion pour le tableau de bord
    /// </summary>
    /// <param name="startDate">Date de début d'analyse</param>
    /// <param name="endDate">Date de fin d'analyse</param>
    /// <returns>Statistiques complètes</returns>
    Task<LoginStatistics> GetLoginStatisticsAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Récupère les sessions actuellement actives
    /// </summary>
    /// <param name="maxSessionAge">Âge maximum des sessions (défaut: 30min)</param>
    /// <returns>Liste des sessions actives</returns>
    Task<IEnumerable<LoginAudit>> GetActiveSessionsAsync(TimeSpan? maxSessionAge = null);

    /// <summary>
    /// Force la déconnexion de toutes les sessions d'un utilisateur
    /// </summary>
    /// <param name="userId">ID utilisateur</param>
    /// <param name="reason">Raison de déconnexion forcée</param>
    /// <returns>Nombre de sessions fermées</returns>
    Task<int> ForceLogoutAllSessionsAsync(string userId, string reason);

    /// <summary>
    /// Nettoie les anciens logs d'audit (tâche de maintenance)
    /// </summary>
    /// <param name="olderThanDays">Supprimer les logs plus anciens que X jours</param>
    /// <returns>Nombre de logs supprimés</returns>
    Task<int> CleanupOldAuditLogsAsync(int olderThanDays = 365);

    /// <summary>
    /// Vérifie si un utilisateur a dépassé le nombre d'échecs autorisés
    /// </summary>
    /// <param name="userId">ID utilisateur</param>
    /// <param name="timeWindow">Fenêtre temporelle d'analyse (défaut: 15min)</param>
    /// <param name="maxAttempts">Nombre max d'échecs (défaut: 5 selon TASK-002)</param>
    /// <returns>True si limite dépassée</returns>
    Task<bool> HasExceededFailedAttemptsAsync(string userId, TimeSpan? timeWindow = null, 
        int maxAttempts = 5);

    /// <summary>
    /// Récupère les adresses IP les plus utilisées par un utilisateur
    /// </summary>
    /// <param name="userId">ID utilisateur</param>
    /// <param name="topCount">Nombre d'IP à retourner (défaut: 10)</param>
    /// <returns>Liste des IP avec compteurs</returns>
    Task<IEnumerable<(string IpAddress, int Count, DateTime LastUsed)>> GetUserTopIpAddressesAsync(
        string userId, int topCount = 10);
}
