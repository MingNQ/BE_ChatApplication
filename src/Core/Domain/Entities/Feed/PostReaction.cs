using Domain.Common.Contracts;
using Domain.Common.Enums;
using Domain.Entities.Identity;

namespace Domain.Entities.Feed;

public class PostReaction : AuditableEntity<long>
{
    public long PostId { get; private set; }
    public long UserId { get; private set; }
    public ReactionTypeEnum Type { get; private set; }
    public virtual Post? Post { get; private set; }
    public virtual User? User { get; private set; }

    public static PostReaction Create(long postId, long userId, ReactionTypeEnum type)
    {
        return new PostReaction
        {
            PostId = postId,
            UserId = userId,
            Type = type
        };
    }

    public void Update(ReactionTypeEnum type)
    {
        Type = type;
    }
}