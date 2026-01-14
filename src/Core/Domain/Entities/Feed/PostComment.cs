using Domain.Common.Contracts;
using Domain.Entities.Identity;

namespace Domain.Entities.Feed;

public class PostComment : AuditableEntity<long>
{
    public long PostId { get; private set; }
    public long UserId { get; private set; }
    public long? RootId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public virtual Post? Post { get; private set; }
    public virtual User? User { get; private set; }
    public virtual PostComment? Root { get; private set; }

    public static PostComment Create(long postId, long userId, string content, long? rootId = null)
    {
        return new PostComment
        {
            PostId = postId,
            UserId = userId,
            Content = content,
            RootId = rootId
        };
    }

    public void Update(string content)
    {
        Content = content;
    }
}