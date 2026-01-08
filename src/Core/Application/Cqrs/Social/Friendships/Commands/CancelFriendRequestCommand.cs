using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Social;
using Domain.Common.Enums;
using Domain.Entities.Social;
using Mapster;
using MediatR;
using Shared.Constants;
using System.Text.Json.Serialization;

namespace Application.Cqrs.Social.Friendships.Commands;

public class CancelFriendRequestCommand : IRequest<FriendshipRequestDto>
{
    [JsonIgnore]
    public long FriendshipRequestId { get; private set; }

    public void SetId(long friendshipRequestId)
    {
        FriendshipRequestId = friendshipRequestId;
    }
}

public class CancelFriendRequestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CancelFriendRequestCommand, FriendshipRequestDto>
{
    private readonly IWriteRepository<FriendshipRequest> _friendshipRequestRepository = unitOfWork.GetRepository<FriendshipRequest>();

    public async Task<FriendshipRequestDto> Handle(CancelFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var friendRequest = await _friendshipRequestRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == request.FriendshipRequestId,
            disableTracking: false) ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(FriendshipRequest), request.FriendshipRequestId));

        friendRequest.UpdateStatus(FriendshipEnum.Canceled);

        _friendshipRequestRepository.Update(friendRequest);
        await unitOfWork.SaveChangesAsync();

        return friendRequest.Adapt<FriendshipRequestDto>();
    }
}