using Application.Cqrs.Identity.Users.Queries;
using Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Shared.Constants;

namespace Host.Controllers.Client;

[ControllerName("users")]
[Tags("Client|User")]
public class UserController : BaseClientAuthController
{
    [HttpGet("current-user")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await Mediator.Send(new GetCurrentUserQuery());
        return Ok(result, MessageCommon.GetDataSuccess);
    }

    [HttpPost("search")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetUserAsync([FromBody] GetUserExternalByConditionQuery request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.GetDataSuccess);
    }

    [HttpGet("{id:long}")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetUserByIdAsync(long id)
    {
        var result = await Mediator.Send(new GetUserExternalByIdQuery
        {
            Id = id
        });
        return Ok(result, MessageCommon.GetDataSuccess);
    }

    [HttpGet("{id:long}/friends")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetFriendByUserIdAsync(long id)
    {
        var result = await Mediator.Send(new GetFriendByUserIdQuery
        {
            Id = id
        });
        return Ok(result, MessageCommon.GetDataSuccess);
    }
}