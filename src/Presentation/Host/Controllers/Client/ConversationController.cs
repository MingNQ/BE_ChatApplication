using Application.Cqrs.Chat.Conversations.Commands;
using Application.Cqrs.Chat.Conversations.Queries;
using Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;

namespace Host.Controllers.Client;

[ControllerName("conversations")]
[Tags("Client|Conversation")]
public class ConversationController : BaseAuthController
{
    [HttpPost("search")]
    public async Task<IActionResult> GetConversationAsync([FromBody] GetConversationByConditionQuery request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.GetDataSuccess);
    }

    [HttpPost("{conversationId:long}")]
    public async Task<IActionResult> AddMemberAsync(long conversationId, AddMemberCommand request)
    {
        request.SetConversationId(conversationId);
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.CreateSuccess);
    }
}