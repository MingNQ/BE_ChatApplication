using Application.Common.Interfaces;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Social;
using Domain.Common.Enums;
using Domain.Entities.Social;
using Mapster;
using MediatR;

namespace Application.Cqrs.Social.Friendships.Commands;

public class AddFriendRequestCommand : IRequest<FriendshipRequestDto>
{
    public long FriendId { get; set; }
}

public class AddFriendRequestCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<AddFriendRequestCommand, FriendshipRequestDto>
{
    private readonly IWriteRepository<FriendshipRequest> _friendshipRepository = unitOfWork.GetRepository<FriendshipRequest>();

    public async Task<FriendshipRequestDto> Handle(AddFriendRequestCommand request, CancellationToken cancellationToken)
    {
        // If user has been sent a request yet, update request
        var friendshipRequest = await _friendshipRepository.GetFirstOrDefaultAsync(
            predicate: x => x.UserId == currentUser.UserId && x.FriendId == request.FriendId,
            disableTracking: false);

        if (friendshipRequest == null)
        {
            friendshipRequest = FriendshipRequest.Create(currentUser.UserId, request.FriendId, FriendshipEnum.Pending);
            await _friendshipRepository.InsertAsync(friendshipRequest, cancellationToken);
            await unitOfWork.SaveChangesAsync();

            return friendshipRequest.Adapt<FriendshipRequestDto>();
        }

        friendshipRequest.UpdateStatus(FriendshipEnum.Pending);
        _friendshipRepository.Update(friendshipRequest);
        await unitOfWork.SaveChangesAsync();

        return friendshipRequest.Adapt<FriendshipRequestDto>();
    }
}