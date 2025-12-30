using Application.Common.Interfaces;
using Application.Dto.Persistence.Catalog.User;

namespace Application.Dto.Chat.Conversations;

public class ConversationMemberDto : IDto
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public long UserId { get; set; }
    public long ConversationRoleId { get; set; }
    public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
    public long? AddedByUserId { get; set; }
    public SortUserInfo? User { get; set; }
    public SortUserInfo? AddedByUser { get; set; }
}