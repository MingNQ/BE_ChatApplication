using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Persistence;
using Application.Common.Services;
using Application.Cqrs.Chat.Conversations.Params;
using Application.Cqrs.Chat.Conversations.Specs;
using Application.Dto.Chat.Conversations;
using Domain.Entities.Chat;
using MediatR;

namespace Application.Cqrs.Chat.Conversations.Queries;

public class GetConversationByConditionQuery : ConversationSearchParam, IRequest<PaginationResponse<ConversationDto>>;

public class GetConversationByConditionQueryHandler(
    IReadRepository<Conversation> conversationRepository,
    IPaginationService paginationService,
    ICurrentUser currentUser)
    : IRequestHandler<GetConversationByConditionQuery, PaginationResponse<ConversationDto>>
{
    public async Task<PaginationResponse<ConversationDto>> Handle(GetConversationByConditionQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId == null)
        {
            request.UserId = currentUser.UserId;
        }

        var spec = new ConversationByConditionSpec(request);
        var conversations = await paginationService.PaginatedListAsync(
            conversationRepository,
            spec,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return conversations;
    }
}