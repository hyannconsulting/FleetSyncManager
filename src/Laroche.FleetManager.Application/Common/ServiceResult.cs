namespace Laroche.FleetManager.Application.Common;

/// <summary>
/// Représente le résultat d'une opération de service
/// </summary>
public class ServiceResult
{
    /// <summary>
    /// Indique si l'opération a réussi
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Message associé au résultat
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Erreurs détaillées (optionnel)
    /// </summary>
    public List<string> Errors { get; private set; }

    /// <summary>
    /// Constructeur protégé pour contrôler la création et permettre l'héritage
    /// </summary>
    protected ServiceResult(bool isSuccess, string message, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors ?? new List<string>();
    }

    /// <summary>
    /// Crée un résultat de succès
    /// </summary>
    /// <param name="message">Message de succès</param>
    /// <returns>Résultat de succès</returns>
    public static ServiceResult Success(string message = "Opération réussie")
    {
        return new ServiceResult(true, message);
    }

    /// <summary>
    /// Crée un résultat d'échec
    /// </summary>
    /// <param name="message">Message d'erreur</param>
    /// <param name="errors">Erreurs détaillées</param>
    /// <returns>Résultat d'échec</returns>
    public static ServiceResult Failed(string message, List<string>? errors = null)
    {
        return new ServiceResult(false, message, errors);
    }

    /// <summary>
    /// Crée un résultat d'échec avec une seule erreur
    /// </summary>
    /// <param name="message">Message d'erreur</param>
    /// <param name="error">Erreur détaillée</param>
    /// <returns>Résultat d'échec</returns>
    public static ServiceResult Failed(string message, string error)
    {
        return new ServiceResult(false, message, new List<string> { error });
    }
}

/// <summary>
/// Représente le résultat d'une opération de service avec données
/// </summary>
/// <typeparam name="T">Type des données retournées</typeparam>
public class ServiceResult<T> : ServiceResult
{
    /// <summary>
    /// Données retournées par l'opération
    /// </summary>
    public T? Data { get; private set; }

    /// <summary>
    /// Constructeur privé pour contrôler la création
    /// </summary>
    private ServiceResult(bool isSuccess, string message, T? data = default, List<string>? errors = null)
        : base(isSuccess, message, errors)
    {
        Data = data;
    }

    /// <summary>
    /// Crée un résultat de succès avec données
    /// </summary>
    /// <param name="data">Données à retourner</param>
    /// <param name="message">Message de succès</param>
    /// <returns>Résultat de succès avec données</returns>
    public static ServiceResult<T> Success(T data, string message = "Opération réussie")
    {
        return new ServiceResult<T>(true, message, data);
    }

    /// <summary>
    /// Crée un résultat de succès sans données
    /// </summary>
    /// <param name="message">Message de succès</param>
    /// <returns>Résultat de succès sans données</returns>
    public new static ServiceResult<T> Success(string message = "Opération réussie")
    {
        return new ServiceResult<T>(true, message);
    }

    /// <summary>
    /// Crée un résultat d'échec
    /// </summary>
    /// <param name="message">Message d'erreur</param>
    /// <param name="errors">Erreurs détaillées</param>
    /// <returns>Résultat d'échec</returns>
    public new static ServiceResult<T> Failed(string message, List<string>? errors = null)
    {
        return new ServiceResult<T>(false, message, default, errors);
    }

    /// <summary>
    /// Crée un résultat d'échec avec une seule erreur
    /// </summary>
    /// <param name="message">Message d'erreur</param>
    /// <param name="error">Erreur détaillée</param>
    /// <returns>Résultat d'échec</returns>
    public new static ServiceResult<T> Failed(string message, string error)
    {
        return new ServiceResult<T>(false, message, default, new List<string> { error });
    }
}
