using Application.Common.Interfaces;
using Domain.Common.Enums;

namespace Application.Dto.Feed;

public class PostDto : IDto
{
    public long Id { get; set; }
    public long AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public PostVisibilityEnum Visibility { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<PostAttachmentDto> Attachments { get; set; } = [];
    public List<PostReactionDto> Reactions { get; set; } = [];
    public List<PostCommentDto> Comments { get; set; } = [];
}