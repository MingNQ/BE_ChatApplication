using Application.Cqrs.Chat.Conversations.Params;
using Application.Dto.Chat.Messages;
using Application.Dto.Social;
using Application.Interfaces.Infrastructures.Repositories;
using Application.Interfaces.Services;
using MediatR;

namespace Application.Cqrs.Chat.Conversations.Queries;

public class GetMessagesByConditionQuery : MessageSearchParam, IRequest<MessagesConversationResponse>;

public class GetMessagesByConditionQueryHandler(
    IMessageReadRepository messageRepository,
    IPresenceService presenceService)
    : IRequestHandler<GetMessagesByConditionQuery, MessagesConversationResponse>
{
    public async Task<MessagesConversationResponse> Handle(GetMessagesByConditionQuery request, CancellationToken cancellationToken)
    {
        var result = await messageRepository.GetMessagesAsync(request.ConversationId, request.Cursor, request.Limit);

        var presences = await presenceService.GetPresencesAsync(result.Members!.Select(m => m.UserId));

        result.Presences = presences.Values.Select(p => new PresenceDto
        {
            UserId = p.UserId,
            LastActiveAt = p.LastActiveAt,
            Status = p.Status
        }).ToList();

        return result;
    }
}