using Domain.Common.Contracts;
using Domain.Entities.Identity;

namespace Domain.Entities.Chat;

public class ConversationMember : AuditableEntity<long>
{
    public long ConversationId { get; private set; }
    public long UserId { get; private set; }
    public long ConversationRoleId { get; private set; }
    public DateTimeOffset JoinedAt { get; private set; } = DateTimeOffset.UtcNow;
    public long? AddedByUserId { get; private set; }
    public virtual User? User { get; set; }
    public virtual User? AddedByUser { get; set; }
    public virtual Conversation? Conversation { get; set; }
    public virtual ConversationRole? ConversationRole { get; set; }

    public static ConversationMember Create(long conversationId, long userId, long? addedByUserId)
    {
        return new ConversationMember
        {
            ConversationId = conversationId,
            UserId = userId,
            AddedByUserId = addedByUserId,
            JoinedAt = DateTimeOffset.UtcNow
        };
    }

    public void AssignRole(long conversationRoleId)
    {
        ConversationRoleId = conversationRoleId;
    }

    public void UpdateRole(long conversationRoleId)
    {
        ConversationRoleId = conversationRoleId;
    }
}