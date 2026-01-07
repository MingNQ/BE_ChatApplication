using Domain.Common.Contracts;
using Domain.Entities.Identity;

namespace Domain.Entities.Social;

public class Follow : AuditableEntity<long>, IAggregateRoot
{
    public long FollowerId { get; private set; }
    public long FollowingId { get; private set; }
    public virtual User? Follower { get; private set; }
    public virtual User? Following { get; private set; }

    public static Follow Create(long followerId, long followingId)
    {
        return new Follow
        {
            FollowerId = followerId,
            FollowingId = followingId
        };
    }
}