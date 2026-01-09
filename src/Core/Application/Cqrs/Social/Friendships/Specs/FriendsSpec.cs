using Ardalis.Specification;
using Domain.Entities.Social;

namespace Application.Cqrs.Social.Friendships.Specs;

public class FriendsSpec : Specification<FriendshipRequest>
{
    public FriendsSpec(long userId)
    {
        Query.Where(x => x.FriendId == userId || x.UserId == userId);

        Query.Where(x => x.Status == Domain.Common.Enums.FriendshipEnum.Accepted);

        Query.Include(x => x.User);

        Query.Include(x => x.Friend);
    }
}