using Application.Common.Interfaces;

namespace Application.Dto.Feed;

public class PostCommentDto : IDto
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long UserId { get; set; }
    public long? RootId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}