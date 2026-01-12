using Domain.Common.Contracts;
using Domain.Entities.Chat;

namespace Domain.Events;

public class MessageSentEvent(string ClientTempId, Message Message) : DomainEvent
{
    public Message Message { get; private set; } = Message;
    public string ClientTempId { get; private set; } = ClientTempId;
}