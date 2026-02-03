using Application.Common.Events;
using Application.Interfaces.Services;
using Domain.Common.Enums;
using Domain.Events;
using Infrastructure.Services.Chat;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Events;

public class MessageSendEventHandler(
    IHubContext<ChatHub> hub,
    IPresenceService presenceService)
    : INotificationHandler<EventNotification<MessageSentEvent>>
{
    public async Task Handle(EventNotification<MessageSentEvent> @event, CancellationToken cancellationToken)
    {
        var message = @event.Event.Message;
        var clientTempId = @event.Event.ClientTempId;
        var conversation = message.Conversation;

        var receiverIds = conversation?.Members
            .Where(m => m.UserId != message.SenderId)
            .Select(m => m.UserId)
            .ToList();

        foreach (var userId in receiverIds!)
        {
            await hub.Clients
                .User(userId.ToString())
                .SendAsync("MessageReceived", new
                {
                    message.Id,
                    message.ConversationId,
                    message.SenderId,
                    message.SentAt,
                    message.Content,
                    message.Attachments,
                    ClientTempId = clientTempId
                }, cancellationToken);
        }

        foreach (var receiverId in receiverIds.Where(id => id != message.SenderId))
        {
            var presence = await presenceService.GetPresenceAsync(receiverId);

            if (presence.Status == UserPresenceStatusEnum.Online)
            {
                await hub.Clients
                    .User(receiverId.ToString())
                    .SendAsync("MessageDelivered", new
                    {
                        MessageId = message.Id
                    }, cancellationToken);
            }
        }
    }
}