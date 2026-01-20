using Application.Cqrs.Chat.Conversations.Params;
using Application.Dto.Chat.Messages;
using Application.Interfaces.Infrastructures.Repositories;
using MediatR;

namespace Application.Cqrs.Chat.Conversations.Queries;

public class GetMessagesByConditionQuery : MessageSearchParam, IRequest<MessagesConversationResponse>;

public class GetMessagesByConditionQueryHandler(IMessageReadRepository messageRepository)
    : IRequestHandler<GetMessagesByConditionQuery, MessagesConversationResponse>
{
    public async Task<MessagesConversationResponse> Handle(GetMessagesByConditionQuery request, CancellationToken cancellationToken)
    {
        var result = await messageRepository.GetMessagesAsync(request.ConversationId, request.Cursor, request.Limit);

        return result;
    }
}