using Application.Cqrs.Identity.Users.Queries;
using Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;

namespace Host.Controllers.Client;

[ControllerName("users")]
[Tags("Client|User")]
public class UserController : BaseClientAuthController
{
    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await Mediator.Send(new GetCurrentUserQuery());
        return Ok(result, MessageCommon.GetDataSuccess);
    }
}