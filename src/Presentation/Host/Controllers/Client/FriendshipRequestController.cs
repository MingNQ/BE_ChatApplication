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

    [HttpGet("received")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetMyFriendshipRequestAsync()
    {
        var result = await Mediator.Send(new GetMyFriendshipRequestQuery());
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
    public async Task<IActionResult> AcceptFriendRequestAsync(long id)
    {
        var request = new AcceptFriendRequestCommand();
        request.SetId(id);
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.UpdateSuccess);
    }

    [HttpPost("{id:long}/cancel")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> CancelFriendRequestAsync(long id)
    {
        var request = new CancelFriendRequestCommand();
        request.SetId(id);
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.UpdateSuccess);
    }

    [HttpPost("{id:long}/reject")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> RejectFriendRequestAsync(long id)
    {
        var request = new RejectFriendRequestCommand();
        request.SetId(id);
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.UpdateSuccess);
    }
}