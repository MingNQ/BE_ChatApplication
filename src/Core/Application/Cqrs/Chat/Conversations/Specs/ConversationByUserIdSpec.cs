using Application.Cqrs.Chat.Conversations.Params;
using Application.Dto.Chat.Conversations;
using Ardalis.Specification;
using Domain.Entities.Chat;

namespace Application.Cqrs.Chat.Conversations.Specs;

public class ConversationByUserIdSpec : Specification<Conversation, ConversationDto>
{
    public ConversationByUserIdSpec(ConversationSearchParam param)
    {
        Query.Where(conversation => conversation.Members.Any(member => member.UserId == param.UserId));

        Query.OrderByDescending(conversation => conversation.LastMessageAt);
    }
}