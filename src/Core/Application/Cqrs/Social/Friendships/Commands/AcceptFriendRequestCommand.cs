using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Social;
using Domain.Common.Enums;
using Domain.Entities.Chat;
using Domain.Entities.Social;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using System.Text.Json.Serialization;

namespace Application.Cqrs.Social.Friendships.Commands;

public class AcceptFriendRequestCommand : IRequest<FriendshipRequestDto>
{
    [JsonIgnore]
    public long FriendshipRequestId { get; private set; }

    public void SetId(long friendshipRequestId)
    {
        FriendshipRequestId = friendshipRequestId;
    }
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

        friendshipRequest.UpdateStatus(FriendshipEnum.Accepted);

        _friendshipRequestRepository.Update(friendshipRequest);

        var conversation = await _conversationRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Members.All(x => x.UserId == friendshipRequest.UserId && x.UserId == friendshipRequest.FriendId)
                && x.Type == ConversationType.Private,
            include: x => x.Include(x => x.Members),
            disableTracking: false);

        if (conversation == null)
        {
            var memberRole = await _conversationRoleRepository.GetFirstOrDefaultAsync(
                predicate: r => r.NormalizedName == AppConsts.MemberConversationRoleName.ToUpperInvariant(),
                disableTracking: true
                ) ?? throw new NotFoundException(CustomResponseMessage.RoleDoesNotExist);

            conversation = Conversation.Create(ConversationType.Private);

            await _conversationRepository.InsertAsync(conversation, cancellationToken);
            await unitOfWork.SaveChangesAsync();

            var user = ConversationMember.Create(conversation.Id, friendshipRequest.UserId, null);
            user.AssignRole(memberRole.Id);
            conversation.AddMember(user);
            var friend = ConversationMember.Create(conversation.Id, friendshipRequest.UserId, null);
            friend.AssignRole(memberRole.Id);
            conversation.AddMember(friend);
        }

        await unitOfWork.SaveChangesAsync();

        return friendshipRequest.Adapt<FriendshipRequestDto>();
    }
}