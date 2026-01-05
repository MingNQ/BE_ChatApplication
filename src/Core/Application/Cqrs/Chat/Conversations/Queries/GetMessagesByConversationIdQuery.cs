using Application.Common.Exceptions;
using Application.Common.Persistence;
using Application.Cqrs.Chat.Conversations.Specs;
using Application.Dto.Chat.Messages;
using Domain.Entities.Chat;
using MediatR;
using Shared.Constants;

namespace Application.Cqrs.Chat.Conversations.Queries;

public class GetMessagesByConversationIdQuery : IRequest<List<MessageDto>>
{
    public long ConversationId { get; set; }
}

public class GetMessagesByConversationIdQueryHandler(IReadRepository<Conversation> conversationRepository)
    : IRequestHandler<GetMessagesByConversationIdQuery, List<MessageDto>>
{
    public async Task<List<MessageDto>> Handle(GetMessagesByConversationIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new MessagesByConversationIdSpec(request.ConversationId);
        var conversation = await conversationRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (conversation == null)
        {
            throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(conversation), request.ConversationId));
        }

        return conversation.Messages ?? [];
    }
}