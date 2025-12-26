using Shared.Events;

namespace Domain.Common.Contracts;

public class DomainEvent : IEvent
{
    public DateTime TriggeredOn { get; protected set; } = DateTime.UtcNow;
}