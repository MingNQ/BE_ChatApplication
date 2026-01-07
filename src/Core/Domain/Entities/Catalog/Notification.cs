using Domain.Common.Contracts;
using Domain.Common.Enums;

namespace Domain.Entities.Catalog;

public class Notification : AuditableEntity<long>, IAggregateRoot
{
    public long? PostId { get; private set; }
    public long? ConversationId { get; private set; }
    public long UserId { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public string? RedirectUrl { get; private set; }
    public bool IsRead { get; private set; }
    public NotificationActivityEnum Activity { get; private set; }

    public static Notification Create(long userId, string message, NotificationActivityEnum activity, long? postId = null, long? conversationId = null, string? redirectUrl = null)
    {
        return new Notification
        {
            PostId = postId,
            ConversationId = conversationId,
            UserId = userId,
            Message = message,
            RedirectUrl = redirectUrl,
            Activity = activity
        };
    }
}