using Application.Common.Interfaces;
using Application.Dto.Chat.Messages;
using Domain.Common.Enums;

namespace Application.Dto.Chat.Conversations;

public class ConversationDto : IDto
{
    public long Id { get; set; }
    public ConversationType Type { get; private set; }
    public string? Name { get; set; }
    public List<MessageDto>? Messages { get; set; }
}