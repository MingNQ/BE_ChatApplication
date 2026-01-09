using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Social.Friendships.Specs;
using Application.Dto.Persistence.Catalog.User;
using Domain.Entities.Social;
using Mapster;
using MediatR;

namespace Application.Cqrs.Social.Friendships.Queries;

public class GetFriendsQuery : IRequest<List<SortUserInfo>>;

public class GetFriendsQueryHandler(
    IReadRepository<FriendshipRequest> friendshipRequestRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetFriendsQuery, List<SortUserInfo>>
{
    public async Task<List<SortUserInfo>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
    {
        var spec = new FriendsSpec(currentUser.UserId);
        var friendships = await friendshipRequestRepository.ListAsync(spec, cancellationToken);

        var friends = friendships.Select(x => (x.UserId == currentUser.UserId) ? x.Friend : x.User).ToList();

        return friends.Adapt<List<SortUserInfo>>();
    }
}