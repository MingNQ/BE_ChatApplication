using Ardalis.Specification;
using Domain.Common.Enums;
using Domain.Entities.Social;

namespace Application.Cqrs.Social.Friendships.Specs;

public class RelatedFriendsSpec : Specification<FriendshipRequest>
{
    public RelatedFriendsSpec(long userId)
    {
        Query.Where(x => x.UserId == userId || x.FriendId == userId);

        Query.Where(x => x.Status != FriendshipEnum.Canceled && x.Status != FriendshipEnum.Rejected);
    }
}