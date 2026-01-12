using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Chat.Conversations.Specs;
using Application.Dto.Chat.Conversations;
using Domain.Entities.Chat;
using MediatR;

namespace Application.Cqrs.Chat.Conversations.Queries;

public class GetConversationByUserIdQuery : IRequest<List<RecentConversationDto>>;

public class GetConversationQueryHandler(
    IReadRepository<Conversation> conversationRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetConversationByUserIdQuery, List<RecentConversationDto>>
{
    public async Task<List<RecentConversationDto>> Handle(GetConversationByUserIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new ConversationByUserIdSpec(currentUser.UserId);
        var conversations = await conversationRepository.ListAsync(spec, cancellationToken);

        var result = conversations.Select(c => new RecentConversationDto
        {
            Id = c.Id,
            Type = c.Type,
            Name = c.Name,
            LastMessageContent = c.Messages?.LastOrDefault()?.Content,
            LastMessageSentAt = c.Messages?.LastOrDefault()?.SentAt,
            UnreadMessagesCount = 0,
            Members = c.Members
        }).OrderByDescending(x => x.LastMessageSentAt)
        .ToList();

        return result;
    }
}