using Application.Common.Interfaces;
using Application.Dto.Chat.Conversations;
using Application.Dto.Persistence.Catalog.User;
using Application.Dto.Social;
using Domain.Common.Enums;

namespace Application.Dto.Chat.Messages;

public class MessageDto : IDto
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public long SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset SentAt { get; set; }
    public List<MessageAttachmentDto> Attachments { get; set; } = [];
    public SortUserInfo? Sender { get; set; }
}

public class MessagesConversationResponse
{
    public long Id { get; set; }
    public ConversationType Type { get; set; }
    public string? Name { get; set; }
    public List<MessageDto>? Messages { get; set; }
    public List<ConversationMemberDto>? Members { get; set; }
    public List<PresenceDto>? Presences { get; set; }
    public DateTimeOffset? NextCursor { get; set; }
    public bool HasMore { get; set; }
}