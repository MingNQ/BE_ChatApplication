using Application.Common.Models;

namespace Application.Cqrs.Chat.Conversations.Params;

public class ConversationSearchParam : PaginationFilter
{
    public long? UserId { get; set; }
}