using Ardalis.Specification;
using Domain.Entities.Chat;

namespace Application.Cqrs.Chat.Conversations.Specs;

public class UserConversationIdsSpec : Specification<Conversation>
{
    public UserConversationIdsSpec(long userId)
    {
        Query.Where(x => x.Members.All(m => m.UserId == userId));
    }
}