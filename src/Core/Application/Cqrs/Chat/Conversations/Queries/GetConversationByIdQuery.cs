using Application.Common.Persistence;
using Application.Cqrs.Chat.Conversations.Specs;
using Application.Dto.Chat.Conversations;
using Application.Dto.Social;
using Application.Interfaces.Services;
using Domain.Entities.Chat;
using MediatR;

namespace Application.Cqrs.Chat.Conversations.Queries;

public class GetConversationByIdQuery : IRequest<ConversationDto>
{
    public long Id { get; set; }
}

public class GetConversationByIdQueryHandler(
    IReadRepository<Conversation> conversationRepository,
    IPresenceService presenceService)
    : IRequestHandler<GetConversationByIdQuery, ConversationDto>
{
    public async Task<ConversationDto> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new ConversationByIdSpec(request.Id);
        var conversation = await conversationRepository.FirstOrDefaultAsync(spec, cancellationToken);

        var presences = await presenceService.GetPresencesAsync(conversation!.Members!.Select(m => m.UserId));

        conversation.Presence = presences.Values.Select(p => new PresenceDto
        {
            UserId = p.UserId,
            LastActiveAt = p.LastActiveAt,
            Status = p.Status
        }).ToList();

        return conversation;
    }
}