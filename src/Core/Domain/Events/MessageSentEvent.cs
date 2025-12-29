using Domain.Common.Contracts;
using Domain.Entities.Chat;

namespace Domain.Events;

public class MessageSentEvent(Message Message) : DomainEvent
{
    public Message Message { get; private set; } = Message;
}