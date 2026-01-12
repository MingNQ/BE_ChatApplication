using Application.Common.Events;
using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Chat.Messages;
using Domain.Entities.Chat;
using Domain.Events;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;

namespace Application.Cqrs.Chat.Conversations.Commands;

public class SendMessageCommand : IRequest<MessageDto>
{
    public long ConversationId { get; set; }
    public long SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ClientTempId { get; set; } = string.Empty;
    public List<MessageAttachmentDto>? Attachments { get; set; } = [];
}

public class SendMessageCommandHandler(IUnitOfWork unitOfWork, IEventPublisher eventPublisher)
    : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IWriteRepository<Conversation> _conversationRepository = unitOfWork.GetRepository<Conversation>();

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetFirstOrDefaultAsync(
            predicate: c => c.Id == request.ConversationId,
            include: c => c.Include(c => c.Members).Include(c => c.Messages),
            disableTracking: false)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Conversation), request.ConversationId));

        var message = conversation.SendMessage(request.SenderId, request.Content, request.ClientTempId);

        if (request.Attachments is not null && request.Attachments.Any())
        {
            foreach (var attachment in request.Attachments)
            {
                message.AddAttachment(MessageAttachment.Create(attachment.MessageId, attachment.FileStorageId, attachment.AttachmentType));
            }
        }

        _conversationRepository.Update(conversation);
        await unitOfWork.SaveChangesAsync();

        var messageSentEvent = new MessageSentEvent(request.ClientTempId, message);
        await eventPublisher.PublishAsync(messageSentEvent);

        return message.Adapt<MessageDto>();
    }
}