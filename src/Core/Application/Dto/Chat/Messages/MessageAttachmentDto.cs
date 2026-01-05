using Application.Common.Interfaces;
using Domain.Common.Enums;

namespace Application.Dto.Chat.Messages;

public class MessageAttachmentDto : IDto
{
    public long Id { get; set; }
    public long MessageId { get; set; }
    public long FileStorageId { get; set; }
    public AttachmentType AttachmentType { get; set; }
}