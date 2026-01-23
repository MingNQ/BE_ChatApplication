using Domain.Common.Contracts;
using Domain.Common.Enums;

namespace Domain.Entities.Social;

public class UserPresenceSnapshot : BaseEntity<long>, IAggregateRoot
{
    public long UserId { get; private set; }
    public UserPresenceStatusEnum Status { get; private set; }
    public DateTimeOffset LastActiveAt { get; private set; }

    public static UserPresenceSnapshot Create(long userId, UserPresenceStatusEnum status, DateTimeOffset lastActiveAt)
    {
        return new UserPresenceSnapshot
        {
            UserId = userId,
            Status = status,
            LastActiveAt = lastActiveAt
        };
    }

    public void Update(UserPresenceStatusEnum status, DateTimeOffset lastActiveAt)
    {
        Status = status;
        LastActiveAt = lastActiveAt;
    }
}