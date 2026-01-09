using Application.Dto.Chat.Conversations;
using Ardalis.Specification;
using Domain.Entities.Chat;

namespace Application.Cqrs.Chat.Conversations.Specs;

public class ConversationByIdSpec : Specification<Conversation, ConversationDto>
{
    public ConversationByIdSpec(long id)
    {
        Query.Where(x => x.Id == id);

        Query.Include(x => x.Messages);

        Query.Include(x => x.Members);
    }
}