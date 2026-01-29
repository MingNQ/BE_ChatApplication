using Domain.Common.Contracts;
using Domain.Entities.Common;

namespace Domain.Entities.Feed;

public class PostAttachment : AuditableEntity<long>
{
    public long PostId { get; private set; }
    public long AttachmentId { get; private set; }
    public virtual Post? Post { get; private set; }
    public virtual FileStorage? Attachment { get; private set; }

    public static PostAttachment Create(long postId, long attachmentId)
    {
        return new PostAttachment
        {
            PostId = postId,
            AttachmentId = attachmentId
        };
    }
}