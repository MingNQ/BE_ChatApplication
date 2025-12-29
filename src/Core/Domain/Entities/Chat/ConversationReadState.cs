using Domain.Common.Contracts;
using Domain.Entities.Identity;

namespace Domain.Entities.Chat;

public class ConversationReadState : AuditableEntity<long>
{
    public long ConversationId { get; private set; }
    public long UserId { get; private set; }
    public long LastReadMessageId { get; private set; }
    public DateTimeOffset ReadAt { get; private set; } = DateTimeOffset.UtcNow;
    public virtual Conversation? Conversation { get; set; }
    public virtual User? User { get; set; }

    public static ConversationReadState Create(long conversationId, long userId, long lastReadMessageId)
    {
        return new ConversationReadState
        {
            ConversationId = conversationId,
            UserId = userId,
            LastReadMessageId = lastReadMessageId,
            ReadAt = DateTimeOffset.UtcNow
        };
    }
}