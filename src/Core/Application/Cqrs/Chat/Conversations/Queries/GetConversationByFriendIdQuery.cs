using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Chat.Conversations.Specs;
using Application.Dto.Chat.Conversations;
using Domain.Entities.Chat;
using Domain.Entities.Identity;
using MediatR;
using Shared.Constants;

namespace Application.Cqrs.Chat.Conversations.Queries;

public class GetConversationByFriendIdQuery : IRequest<ConversationDto>
{
    public long FriendId { get; set; }
}

public class GetConversationByFriendIdQueryHandler(
    IReadRepository<Conversation> conversationRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetConversationByFriendIdQuery, ConversationDto>
{
    public async Task<ConversationDto> Handle(GetConversationByFriendIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new ConversationByFriendIdSpec(currentUser.UserId, request.FriendId);
        var conversation = await conversationRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (conversation == null)
        {
            throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(User), nameof(request.FriendId)));
        }

        return conversation!;
    }
}