using Application.Common.Interfaces;
using Application.Dto.Persistence.Catalog.User;

namespace Application.Dto.Chat.Messages;

public class MessageDto : IDto
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public long SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public List<MessageAttachmentDto> Attachments { get; set; } = [];
    public SortUserInfo? Sender { get; set; }
}