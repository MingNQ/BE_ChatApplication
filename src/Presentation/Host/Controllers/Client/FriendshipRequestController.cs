using Application.Cqrs.Social.Friendships.Commands;
using Application.Cqrs.Social.Friendships.Queries;
using Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Shared.Constants;

namespace Host.Controllers.Client;

[ControllerName("friend-requests")]
[Tags("Social|Friendship Request")]
public class FriendshipRequestController : BaseClientAuthController
{
    [HttpPost("search")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetAsync([FromBody] GetFriendshipRequestByConditionQuery request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.GetDataSuccess);
    }

    [HttpPost()]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> CreateFriendRequestAsync(AddFriendRequestCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.CreateSuccess);
    }

    [HttpPost("{id:long}/accept")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> AcceptFriendRequestAsync(long id, AcceptFriendRequestCommand request)
    {
        request.SetId(id);
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.UpdateSuccess);
    }

    [HttpPost("{id:long}/cancel")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> CancelFriendRequestAsync(long id, CancelFriendRequestCommand request)
    {
        request.SetId(id);
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.UpdateSuccess);
    }

    [HttpPost("{id:long}/reject")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> RejectFriendRequestAsync(long id, RejectFriendRequestCommand request)
    {
        request.SetId(id);
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.UpdateSuccess);
    }
}