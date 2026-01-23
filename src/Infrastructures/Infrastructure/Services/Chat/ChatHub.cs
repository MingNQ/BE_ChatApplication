using Application.Cqrs.Chat.Conversations.Commands;
using Application.Cqrs.Chat.Conversations.Queries;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services.Chat;

[Authorize]
public class ChatHub(
    IMediator mediator,
    IPresenceService presenceService) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userIdStr = Context.UserIdentifier;

        if (userIdStr == null)
        {
            return;
        }
        var userId = long.Parse(userIdStr);

        await presenceService.UserConnectedAsync(userId, Context.ConnectionId);

        var conversationIds = await mediator.Send(
            new GetUserConversationIdsQuery() { UserId = userId });

        foreach (var conversationId in conversationIds)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                conversationId.ToString());
        }

        await Clients.Others.SendAsync(
            "UserOnline",
            userId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdStr = Context.UserIdentifier;

        if (userIdStr == null)
            return;

        var userId = long.Parse(userIdStr);

        var isOffline = await presenceService.UserDisconnectedAsync(
            userId,
            Context.ConnectionId
        );

        if (isOffline)
        {
            await Clients.Others.SendAsync(
                "UserOffline",
                userId
            );
        }
    }

    public async Task HeartBeat()
    {
        var userId = Context.UserIdentifier;

        if (userId == null)
        {
            return;
        }

        await presenceService.HeartBeatAsync(long.Parse(userId));
    }

    public async Task Typing(long conversationId)
    {
        var userId = long.Parse(Context.UserIdentifier!);

        await Clients
            .GroupExcept(conversationId.ToString(), Context.ConnectionId)
            .SendAsync("UserTyping", conversationId, userId);
    }

    public async Task SendMessage(SendMessageCommand request)
    {
        await mediator.Send(request);
    }

    public async Task JoinConversation(long conversationId)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            conversationId.ToString());
    }

    public async Task LeaveConversation(long conversationId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            conversationId.ToString());
    }
}