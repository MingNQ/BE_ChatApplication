using Ardalis.Specification;
using Domain.Entities.Social;

namespace Application.Cqrs.Social.Friendships.Specs;

public class MyFriendshipRequestSpec : Specification<FriendshipRequest>
{
    public MyFriendshipRequestSpec(long currentUserId)
    {
        Query.Where(x => x.FriendId == currentUserId);

        Query.Include(x => x.User);

        Query.Include(x => x.Friend);
    }
}