using Application.Common.Specification;
using Application.Cqrs.Chat.Conversations.Params;
using Ardalis.Specification;
using Domain.Entities.Chat;

namespace Application.Cqrs.Chat.Conversations.Specs;

public class MessagesByConditionSpec : BaseSpec<Conversation>
{
    public MessagesByConditionSpec(MessageSearchParam param)
    {
        Query.Take(param.Limit);
    }
}