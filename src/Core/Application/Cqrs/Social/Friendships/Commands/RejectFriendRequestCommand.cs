using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Social;
using Domain.Common.Enums;
using Domain.Entities.Social;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using System.Text.Json.Serialization;

namespace Application.Cqrs.Social.Friendships.Commands;

public class RejectFriendRequestCommand : IRequest<FriendshipRequestDto>
{
    [JsonIgnore]
    public long FriendshipRequestId { get; private set; }

    public void SetId(long friendshipRequestId)
    {
        FriendshipRequestId = friendshipRequestId;
    }
}

public class RejectFriendRequestCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RejectFriendRequestCommand, FriendshipRequestDto>
{
    private readonly IWriteRepository<FriendshipRequest> _friendshipRequestRepository = unitOfWork.GetRepository<FriendshipRequest>();

    public async Task<FriendshipRequestDto> Handle(RejectFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var friendRequest = await _friendshipRequestRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == request.FriendshipRequestId,
            include: x => x.Include(x => x.User!)
                .Include(x => x.Friend!),
            disableTracking: false) ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(FriendshipRequest), request.FriendshipRequestId));

        friendRequest.UpdateStatus(FriendshipEnum.Rejected);

        _friendshipRequestRepository.Update(friendRequest);
        await unitOfWork.SaveChangesAsync();

        return friendRequest.Adapt<FriendshipRequestDto>();
    }
}