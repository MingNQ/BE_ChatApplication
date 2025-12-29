using Domain.Common.Contracts;
using Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Chat;

public class Conversation : AuditableEntity<long>, IAggregateRoot
{
    public ConversationType Type { get; private set; }

    [MaxLength(256)]
    public string? Name { get; private set; }

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
}