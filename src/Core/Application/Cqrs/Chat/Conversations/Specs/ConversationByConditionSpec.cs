using Application.Cqrs.Chat.Conversations.Params;
using Application.Dto.Chat.Conversations;
using Ardalis.Specification;
using Domain.Entities.Chat;

namespace Application.Cqrs.Chat.Conversations.Specs;

public class ConversationByConditionSpec : Specification<Conversation, ConversationDto>
{
    public ConversationByConditionSpec(ConversationSearchParam param)
    {
        Query.Where(conversation => conversation.Members.Any(member => member.UserId == param.UserId));

        Query.Include(conversation => conversation.Members);

        Query.Include(conversation => conversation.Messages);
    }
}