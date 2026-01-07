using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Social;
using Domain.Common.Enums;
using Domain.Entities.Chat;
using Domain.Entities.Social;
using Mapster;
using MediatR;
using Shared.Constants;

namespace Application.Cqrs.Social.Friendships.Commands;

public class AcceptFriendRequestCommand : IRequest<FriendshipRequestDto>
{
    public long FriendshipRequestId { get; set; }
}

public class AcceptFriendRequestCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AcceptFriendRequestCommand, FriendshipRequestDto>
{
    private readonly IWriteRepository<FriendshipRequest> _friendshipRequestRepository = unitOfWork.GetRepository<FriendshipRequest>();
    private readonly IWriteRepository<Conversation> _conversationRepository = unitOfWork.GetRepository<Conversation>();
    private readonly IWriteRepository<ConversationRole> _conversationRoleRepository = unitOfWork.GetRepository<ConversationRole>();

    public async Task<FriendshipRequestDto> Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var friendshipRequest = await _friendshipRequestRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == request.FriendshipRequestId,
            disableTracking: false) ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(FriendshipRequest), request.FriendshipRequestId));

        var memberRole = await _conversationRoleRepository.GetFirstOrDefaultAsync(
            predicate: r => r.NormalizedName == AppConsts.MemberConversationRoleName.ToUpperInvariant(),
            disableTracking: true
            ) ?? throw new NotFoundException(CustomResponseMessage.RoleDoesNotExist);

        friendshipRequest.UpdateStatus(FriendshipEnum.Accepted);

        _friendshipRequestRepository.Update(friendshipRequest);

        var conversation = Conversation.Create(ConversationType.Private);

        await _conversationRepository.InsertAsync(conversation, cancellationToken);
        await unitOfWork.SaveChangesAsync();

        var user = ConversationMember.Create(conversation.Id, friendshipRequest.UserId, null);
        user.AssignRole(memberRole.Id);
        conversation.AddMember(user);
        var friend = ConversationMember.Create(conversation.Id, friendshipRequest.UserId, null);
        friend.AssignRole(memberRole.Id);
        conversation.AddMember(friend);

        await unitOfWork.SaveChangesAsync();

        return friendshipRequest.Adapt<FriendshipRequestDto>();
    }
}