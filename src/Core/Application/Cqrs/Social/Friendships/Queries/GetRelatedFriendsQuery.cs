using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Social.Friendships.Specs;
using Application.Dto.Persistence.Catalog.User;
using Domain.Entities.Identity;
using Domain.Entities.Social;
using Mapster;
using MediatR;

namespace Application.Cqrs.Social.Friendships.Queries;

public class GetRelatedFriendsQuery : IRequest<List<SortUserInfo>>;

public class GetRelatedFriendsQueryHandler(
    IReadRepository<User> userRepository,
    IReadRepository<FriendshipRequest> friendshipRequestRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetRelatedFriendsQuery, List<SortUserInfo>>
{
    public async Task<List<SortUserInfo>> Handle(GetRelatedFriendsQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.ListAsync(cancellationToken);
        var spec = new RelatedFriendsSpec(currentUser.UserId);
        var friendRequest = await friendshipRequestRepository.ListAsync(spec, cancellationToken);

        var relatedFriends = users.Where(x => friendRequest.All(f => f.FriendId != x.Id && f.UserId != x.Id) && x.Id != currentUser.UserId).ToList();

        return relatedFriends.Adapt<List<SortUserInfo>>();
    }
}