using Ardalis.Specification;
using Domain.Entities.Social;

namespace Application.Cqrs.Social.Friendships.Specs;

public class MyFriendshipReceivedSpec : Specification<FriendshipRequest>
{
    public MyFriendshipReceivedSpec(long currentUserId)
    {
        Query.Where(x => x.FriendId == currentUserId);

        Query.Where(x => x.Status == Domain.Common.Enums.FriendshipEnum.Pending);

        Query.Include(x => x.User);

        Query.Include(x => x.Friend);
    }
}