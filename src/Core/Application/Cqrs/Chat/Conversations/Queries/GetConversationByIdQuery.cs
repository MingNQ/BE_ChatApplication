using Application.Common.Persistence;
using Application.Cqrs.Chat.Conversations.Specs;
using Application.Dto.Chat.Conversations;
using Domain.Entities.Chat;
using MediatR;

namespace Application.Cqrs.Chat.Conversations.Queries;

public class GetConversationByIdQuery : IRequest<ConversationDto>
{
    public long Id { get; set; }
}

public class GetConversationByIdQueryHandler(IReadRepository<Conversation> conversationRepository)
    : IRequestHandler<GetConversationByIdQuery, ConversationDto>
{
    public async Task<ConversationDto> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new ConversationByIdSpec(request.Id);
        var conversation = await conversationRepository.FirstOrDefaultAsync(spec, cancellationToken);
        return conversation!;
    }
}