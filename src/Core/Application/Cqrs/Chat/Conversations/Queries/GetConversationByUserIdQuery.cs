using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Chat.Conversations.Specs;
using Application.Dto.Chat.Conversations;
using Domain.Entities.Chat;
using MediatR;

namespace Application.Cqrs.Chat.Conversations.Queries;

public class GetConversationByUserIdQuery : IRequest<List<ConversationDto>>;

public class GetConversationQueryHandler(
    IReadRepository<Conversation> conversationRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetConversationByUserIdQuery, List<ConversationDto>>
{
    public async Task<List<ConversationDto>> Handle(GetConversationByUserIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new ConversationByUserIdSpec(currentUser.UserId);
        var conversations = await conversationRepository.ListAsync(spec, cancellationToken);

        return conversations;
    }
}