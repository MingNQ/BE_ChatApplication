using Application.Dto.Persistence.Catalog.User;
using Application.Interfaces.Services;
using Mapster;
using MediatR;

namespace Application.Cqrs.Identity.Users.Queries;

public class GetFriendByUserIdQuery : IRequest<List<SortUserInfo>>
{
    public long Id { get; set; }
}

public class GetFriendByUserIdQueryHandler(
    IFriendshipService friendshipService)
    : IRequestHandler<GetFriendByUserIdQuery, List<SortUserInfo>>
{
    public async Task<List<SortUserInfo>> Handle(GetFriendByUserIdQuery request, CancellationToken cancellationToken)
    {
        var friendships = await friendshipService.GetFriendsAsync(request.Id, cancellationToken);

        return friendships.Adapt<List<SortUserInfo>>();
    }
}