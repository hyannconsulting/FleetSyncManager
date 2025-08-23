using MediatR;

namespace Laroche.FleetManager.Domain.Common;

/// <summary>
/// Base interface for domain events
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// Date and time when the event occurred
    /// </summary>
    DateTime OccurredOn { get; }
}
