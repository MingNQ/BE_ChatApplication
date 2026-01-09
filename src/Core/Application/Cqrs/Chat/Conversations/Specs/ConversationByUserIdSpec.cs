using Application.Dto.Chat.Conversations;
using Ardalis.Specification;
using Domain.Entities.Chat;

namespace Application.Cqrs.Chat.Conversations.Specs;

public class ConversationByUserIdSpec : Specification<Conversation, ConversationDto>
{
    public ConversationByUserIdSpec(long userId)
    {
        Query.Where(conversation => conversation.Members.Any(member => member.UserId == userId));

        Query.Include(conversation => conversation.Members);

        Query.Include(conversation => conversation.Messages);
    }
}