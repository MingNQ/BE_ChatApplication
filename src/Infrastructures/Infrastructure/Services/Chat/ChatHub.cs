using Application.Cqrs.Chat.Conversations.Commands;
using Application.Cqrs.Chat.Conversations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Web;

namespace Infrastructure.Services.Chat;

[Authorize]
public class ChatHub(IMediator mediator) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.GetUserFlowId();

        if (userId == null)
        {
            return;
        }

        var conversationIds = await mediator.Send(
            new GetUserConversationIdsQuery() { UserId = long.Parse(userId) });

        foreach (var conversationId in conversationIds)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                conversationId.ToString());
        }
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