using Application.Cqrs.Social.Friendships.Commands;
using Application.Cqrs.Social.Friendships.Queries;
using Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Shared.Constants;
using System.Threading.Tasks;

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

    [HttpPost("accept")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> AcceptFriendRequestAsync(AcceptFriendRequestCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.UpdateSuccess);
    }

    [HttpPost("cancel")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> CancelFriendRequestAsync(CancelFriendRequestCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.UpdateSuccess);
    }
}