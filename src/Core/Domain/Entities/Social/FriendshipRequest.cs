using Domain.Common.Contracts;
using Domain.Common.Enums;
using Domain.Entities.Identity;

namespace Domain.Entities.Social;

public class FriendshipRequest : AuditableEntity<long>, IAggregateRoot
{
    public long UserId { get; private set; }
    public long FriendId { get; private set; }
    public FriendshipEnum Status { get; private set; }
    public virtual User? User { get; private set; }
    public virtual User? Friend { get; private set; }

    public static FriendshipRequest Create(long userId, long friendId, FriendshipEnum status)
    {
        return new FriendshipRequest
        {
            UserId = userId,
            FriendId = friendId,
            Status = status
        };
    }

    public void UpdateStatus(FriendshipEnum status)
    {
        Status = status;
    }
}