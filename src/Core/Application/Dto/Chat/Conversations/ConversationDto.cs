using Application.Common.Interfaces;
using Application.Dto.Chat.Messages;
using Application.Dto.Social;
using Domain.Common.Enums;

namespace Application.Dto.Chat.Conversations;

public class ConversationDto : IDto
{
    public long Id { get; set; }
    public ConversationType Type { get; set; }
    public string? Name { get; set; }
    public List<MessageDto>? Messages { get; set; }
    public List<ConversationMemberDto>? Members { get; set; }
    public List<PresenceDto>? Presence { get; set; }
}

public class RecentConversationDto : IDto
{
    public long Id { get; set; }
    public ConversationType Type { get; set; }
    public string? Name { get; set; }
    public string? LastMessageContent { get; set; }
    public string? LastMessageContentKey { get; set; }
    public DateTimeOffset? LastMessageSentAt { get; set; }
    public int UnreadMessagesCount { get; set; }
    public List<ConversationMemberDto>? Members { get; set; }
}