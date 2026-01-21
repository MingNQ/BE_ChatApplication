using Application.Common.Persistence;
using Application.Cqrs.Social.Friendships.Specs;
using Application.Dto.Persistence.Catalog.User;
using Domain.Entities.Social;
using Mapster;
using MediatR;

namespace Application.Cqrs.Identity.Users.Queries;

public class GetFriendByUserIdQuery : IRequest<List<SortUserInfo>>
{
    public long Id { get; set; }
}

public class GetFriendByUserIdQueryHandler(
    IReadRepository<FriendshipRequest> friendshipRequestRepository) : IRequestHandler<GetFriendByUserIdQuery, List<SortUserInfo>>
{
    public async Task<List<SortUserInfo>> Handle(GetFriendByUserIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new FriendsSpec(request.Id);
        var friendships = await friendshipRequestRepository.ListAsync(spec, cancellationToken);

        var friends = friendships.Select(x => (x.UserId == request.Id) ? x.Friend : x.User).ToList();

        return friends.Adapt<List<SortUserInfo>>();
    }
}