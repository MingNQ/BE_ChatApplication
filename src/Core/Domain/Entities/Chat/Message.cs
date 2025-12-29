using Domain.Common.Contracts;
using Domain.Entities.Identity;

namespace Domain.Entities.Chat;

public class Message : AuditableEntity<long>
{
    public long ConversationId { get; private set; }
    public long SenderId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public DateTimeOffset SentAt { get; private set; } = DateTimeOffset.UtcNow;
    private readonly List<MessageAttachment> _attachments = [];
    public virtual IReadOnlyCollection<MessageAttachment> Attachments => _attachments.AsReadOnly();
    public virtual Conversation? Conversation { get; set; }
    public virtual User? Sender { get; set; }

    public static Message Create(long conversationId, long senderId, string content)
    {
        return new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            SentAt = DateTimeOffset.UtcNow
        };
    }

    public void UpdateContent(string content)
    {
        Content = content;
    }
}