using Domain.Common.Contracts;
using Domain.Entities.Common;

namespace Domain.Entities.Chat;

public class MessageAttachment : AuditableEntity<long>
{
    public long MessageId { get; private set; }
    public long FileStorageId { get; private set; }
    public virtual Message? Message { get; set; }
    public virtual FileStorage? FileStorage { get; set; }

    public static MessageAttachment Create(long messageId, long fileStorageId)
    {
        return new MessageAttachment
        {
            MessageId = messageId,
            FileStorageId = fileStorageId
        };
    }
}