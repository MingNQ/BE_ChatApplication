using Application.Common.Persistence;
using Application.Cqrs.Social.Friendships.Specs;
using Application.Interfaces.Services;
using Domain.Entities.Identity;
using Domain.Entities.Social;

namespace Infrastructure.Services.Social;

public class FriendshipService(
    IReadRepository<FriendshipRequest> friendshipRequestRepository)
    : IFriendshipService
{
    public async Task<List<User>> GetFriendsAsync(long userId, CancellationToken cancellationToken)
    {
        var friendsSpec = new FriendsSpec(userId);
        var friendships = await friendshipRequestRepository.ListAsync(friendsSpec, cancellationToken);
        var friends = friendships.Select(x => (x.UserId == userId) ? x.Friend! : x.User!).ToList();

        return friends;
    }
}