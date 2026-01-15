using Application.Common.Interfaces;
using Application.Dto.Persistence.Catalog.User;

namespace Application.Dto.Feed;

public class PostCommentDto : IDto
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long UserId { get; set; }
    public long? RootId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    public SortUserInfo? User { get; set; }
}