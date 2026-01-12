using Domain.Common.Contracts;
using Domain.Common.Enums;
using Domain.Events;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Chat;

public class Conversation : AuditableEntity<long>, IAggregateRoot
{
    public ConversationType Type { get; private set; }

    [MaxLength(256)]
    public string? Name { get; private set; }

    public long ConversationReadStateId { get; set; }
    private readonly List<ConversationMember> _members = [];
    public virtual IReadOnlyCollection<ConversationMember> Members => _members.AsReadOnly();
    private readonly List<Message> _messages = [];
    public virtual IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    public static Conversation Create(ConversationType type, string? name = null)
    {
        if (type == ConversationType.Group && string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Group conversations must have a name.");
        }
        return new Conversation
        {
            Type = type,
            Name = name
        };
    }

    public void Update(string? name)
    {
        Name = name;
    }

    public void AddMember(ConversationMember member)
    {
        _members.Add(member);
    }

    public void AddMembers(IEnumerable<ConversationMember> members)
    {
        _members.AddRange(members);
    }

    public void RemoveMember(ConversationMember member)
    {
        _members.Remove(member);
    }

    public void RemoveMembers(IEnumerable<ConversationMember> members)
    {
        foreach (var member in members)
        {
            _members.Remove(member);
        }
    }

    public Message SendMessage(long senderId, string content, string clientTempId)
    {
        if (!_members.Any(m => m.UserId == senderId))
        {
            throw new InvalidOperationException("Sender is not a member of the conversation.");
        }

        var message = Message.Create(Id, senderId, content);
        _messages.Add(message);

        DomainEvents.Add(new MessageSentEvent(clientTempId, message));

        return message;
    }
}