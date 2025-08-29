namespace Laroche.FleetManager.Domain.Constants;

/// <summary>
/// Constantes pour les rôles utilisateur
/// </summary>
public static class UserRoles
{
    /// <summary>
    /// Administrateur système - accès complet
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// Gestionnaire de flotte - gestion opérationnelle
    /// </summary>
    public const string FleetManager = "FleetManager";

    /// <summary>
    /// Conducteur - consultation de ses données
    /// </summary>
    public const string Driver = "Driver";

    /// <summary>
    /// Liste de tous les rôles disponibles
    /// </summary>
    public static readonly string[] AllRoles = { Admin, FleetManager, Driver };

    /// <summary>
    /// Rôles avec privilèges administratifs
    /// </summary>
    public static readonly string[] AdminRoles = { Admin, FleetManager };
}
