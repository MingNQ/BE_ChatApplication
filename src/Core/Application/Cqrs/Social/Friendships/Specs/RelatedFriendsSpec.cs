using Ardalis.Specification;
using Domain.Common.Enums;
using Domain.Entities.Social;

namespace Application.Cqrs.Social.Friendships.Specs;

public class RelatedFriendsSpec : Specification<FriendshipRequest>
{
    public RelatedFriendsSpec(long userId)
    {
        Query.Where(f =>
            (f.UserId == userId || f.FriendId == userId) &&
            (f.Status == FriendshipEnum.Accepted || f.Status == FriendshipEnum.Pending));
    }
}