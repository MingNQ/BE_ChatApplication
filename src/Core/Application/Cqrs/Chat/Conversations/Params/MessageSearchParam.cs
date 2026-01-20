namespace Application.Cqrs.Chat.Conversations.Params;

public class MessageSearchParam
{
    public long ConversationId { get; set; }
    public DateTimeOffset? Cursor { get; set; }
    public int Limit { get; set; }
}