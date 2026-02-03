using Application.Cqrs.Social.Presences.Queries;
using Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Shared.Constants;

namespace Host.Controllers.Client;

[ControllerName("presences")]
[Tags("Social|Presence")]
public class PresenceController : BaseClientAuthController
{
    [HttpGet("")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetPresencesAsync()
    {
        var result = await Mediator.Send(new GetPresencesQuery());

        return Ok(result, MessageCommon.GetDataSuccess);
    }
}