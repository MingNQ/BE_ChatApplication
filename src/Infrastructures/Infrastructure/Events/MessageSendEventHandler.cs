using Application.Common.Events;
using Domain.Events;
using Infrastructure.Services.Chat;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Application.Cqrs.Chat.Conversations.Events;

public class MessageSendEventHandler(IHubContext<ChatHub> hub)
    : INotificationHandler<EventNotification<MessageSentEvent>>
{
    public async Task Handle(EventNotification<MessageSentEvent> @event, CancellationToken cancellationToken)
    {
        var message = @event.Event.Message;

        await hub
            .Clients
            .Group(message.ConversationId.ToString())
            .SendAsync("MessageReceived", new
            {
                message.Id,
                message.ConversationId,
                message.SenderId,
                message.SentAt,
                message.Content,
                message.Attachments
            }, cancellationToken: cancellationToken);
    }
}