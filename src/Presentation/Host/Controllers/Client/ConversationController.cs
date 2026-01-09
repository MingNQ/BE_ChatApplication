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
    [HttpPost("search")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetConversationAsync([FromBody] GetConversationByConditionQuery request)
    {
        var result = await Mediator.Send(request);
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
    public async Task<IActionResult> GetMessagesAsync(long conversationId)
    {
        var result = await Mediator.Send(new GetMessagesByConversationIdQuery
        {
            ConversationId = conversationId
        });

        return Ok(result, MessageCommon.GetDataSuccess);
    }
}