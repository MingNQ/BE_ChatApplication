using Application.Dto.Chat.Conversations;
using Ardalis.Specification;
using Domain.Entities.Chat;

namespace Application.Cqrs.Chat.Conversations.Specs;

public class MessagesByConversationIdSpec : Specification<Conversation, ConversationDto>
{
    public MessagesByConversationIdSpec(long conversationId)
    {
        Query.Where(x => x.Id == conversationId);

        Query.Include(x => x.Members);

        Query.Include(x => x.Messages).ThenInclude(m => m.Attachments);
    }
}