using Application.Dto.Chat.Conversations;
using Ardalis.Specification;
using Domain.Entities.Chat;

namespace Application.Cqrs.Chat.Conversations.Specs;

public class ConversationByFriendIdSpec : Specification<Conversation, ConversationDto>
{
    public ConversationByFriendIdSpec(long userId, long friendId)
    {
        Query.Where(x => x.Members.All(x =>
            (x.UserId == userId && x.AddedByUserId == friendId) ||
            (x.UserId == friendId && x.AddedByUserId == userId)));
    }
}