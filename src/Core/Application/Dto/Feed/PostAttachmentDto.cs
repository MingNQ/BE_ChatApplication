using Application.Common.Interfaces;

namespace Application.Dto.Feed;

public class PostAttachmentDto : IDto
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long AttachmentId { get; set; }
}