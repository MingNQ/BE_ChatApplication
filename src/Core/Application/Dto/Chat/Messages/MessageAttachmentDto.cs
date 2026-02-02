using Application.Dto.Persistence.Catalog.FileStorages;

namespace Application.Dto.Chat.Messages;

public class MessageAttachmentDto
{
    public long Id { get; set; }
    public long MessageId { get; set; }
    public long FileStorageId { get; set; }
    public FileStorageDto? FileStorage { get; set; }
}