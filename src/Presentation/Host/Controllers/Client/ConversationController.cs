using Application.Cqrs.Chat.Conversations.Commands;
using Application.Cqrs.Chat.Conversations.Queries;
using Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Shared.Constants;

namespace Host.Controllers.Client;

[ControllerName("conversations")]
[Tags("Client|Conversation")]
public class ConversationController : BaseClientAuthController
{
    [HttpGet("")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetConversationAsync()
    {
        var result = await Mediator.Send(new GetConversationByUserIdQuery());
        return Ok(result, MessageCommon.GetDataSuccess);
    }

    [HttpGet("{id:long}")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetConversationByIdAsync(long id)
    {
        var result = await Mediator.Send(new GetConversationByIdQuery()
        {
            Id = id
        });
        return Ok(result, MessageCommon.GetDataSuccess);
    }

    [HttpGet("friends/{id:long}")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetConversationByFriendAsync(long id)
    {
        var result = await Mediator.Send(new GetConversationByFriendIdQuery
        {
            FriendId = id
        });
        return Ok(result, MessageCommon.GetDataSuccess);
    }

    [HttpPost]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> CreateConversationAsync(CreateConversationCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.CreateSuccess);
    }

    [HttpPost("{conversationId:long}")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> AddMemberAsync(long conversationId, AddMemberCommand request)
    {
        request.SetConversationId(conversationId);
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.CreateSuccess);
    }

    [HttpGet("{conversationId:long}/messages")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetMessagesByConditionAsync(long conversationId, [FromQuery] DateTimeOffset? cursor, [FromQuery] int limit = 20)
    {
        var result = await Mediator.Send(new GetMessagesByConditionQuery
        {
            ConversationId = conversationId,
            Cursor = cursor,
            Limit = limit
        });

        return Ok(result, MessageCommon.GetDataSuccess);
    }
}