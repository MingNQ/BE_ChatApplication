using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Social.Friendships.Specs;
using Application.Dto.Social;
using Domain.Entities.Social;
using Mapster;
using MediatR;

namespace Application.Cqrs.Social.Friendships.Queries;

public class GetMyFriendshipRequestQuery : IRequest<List<FriendshipRequestDto>>;

public class GetMyFriendshipRequestQueryHandler(
    IReadRepository<FriendshipRequest> friendshipRequestRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetMyFriendshipRequestQuery, List<FriendshipRequestDto>>
{
    public async Task<List<FriendshipRequestDto>> Handle(GetMyFriendshipRequestQuery request, CancellationToken cancellationToken)
    {
        var spec = new MyFriendshipRequestSpec(currentUser.UserId);
        var friendshipRequests = await friendshipRequestRepository.ListAsync(spec, cancellationToken);

        return friendshipRequests.Adapt<List<FriendshipRequestDto>>();
    }
}