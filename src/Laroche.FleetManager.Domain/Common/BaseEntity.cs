namespace Laroche.FleetManager.Domain.Common;

/// <summary>
/// Base class for all domain entities
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Entity unique identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Entity creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Entity last modification date
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// User who created the entity
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// User who last modified the entity
    /// </summary>
    public string? UpdatedBy { get; set; }
    
    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// Domain events for this entity
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = [];
    
    /// <summary>
    /// Gets the domain events
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    /// <summary>
    /// Add a domain event
    /// </summary>
    /// <param name="domainEvent">The domain event to add</param>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    /// <summary>
    /// Remove a domain event
    /// </summary>
    /// <param name="domainEvent">The domain event to remove</param>
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }
    
    /// <summary>
    /// Clear all domain events
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
