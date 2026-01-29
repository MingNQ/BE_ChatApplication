using Application.Common.Interfaces;
using Application.Dto.Persistence.Catalog.FileStorages;

namespace Application.Dto.Feed;

public class PostAttachmentDto : IDto
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long AttachmentId { get; set; }
    public FileStorageDto? Attachment { get; set; }
}