using Application.Common.Interfaces;

namespace Application.Dto.Chat.Conversations;

public class ConversationRoleDto : IDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
}