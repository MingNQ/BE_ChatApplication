using Application.Common.Interfaces;

namespace Application.Dto.Chat.Conversations;

public class ConversationReadStateDto : IDto
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public long UserId { get; set; }
    public long LastReadMessageId { get; set; }
    public DateTimeOffset ReadAt { get; set; } = DateTimeOffset.UtcNow;
}