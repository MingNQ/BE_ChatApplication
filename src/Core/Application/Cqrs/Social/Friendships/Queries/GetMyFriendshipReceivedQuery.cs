using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Social.Friendships.Specs;
using Application.Dto.Social;
using Domain.Entities.Social;
using Mapster;
using MediatR;

namespace Application.Cqrs.Social.Friendships.Queries;

public class GetMyFriendshipReceivedQuery : IRequest<List<FriendshipRequestDto>>;

public class GetMyFriendshipReceivedQueryHandler(
    IReadRepository<FriendshipRequest> friendshipRequestRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetMyFriendshipReceivedQuery, List<FriendshipRequestDto>>
{
    public async Task<List<FriendshipRequestDto>> Handle(GetMyFriendshipReceivedQuery request, CancellationToken cancellationToken)
    {
        var spec = new MyFriendshipReceivedSpec(currentUser.UserId);
        var friendshipReceived = await friendshipRequestRepository.ListAsync(spec, cancellationToken);

        return friendshipReceived.Adapt<List<FriendshipRequestDto>>();
    }
}