using Application.Common.Interfaces;
using Application.Cqrs.Identity.Users.Commands;
using Application.Cqrs.Identity.Users.Queries;
using Host.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Shared.Constants;

namespace Host.Controllers.Manager;

[ControllerName("user")]
[Tags("Admin|User")]
public class UserController(ICurrentUser currentUser) : BaseAdminAuthController
{
    [Authorize(Roles = $"{AppConsts.SuperAdminRoleName}")]
    [HttpPost]
    [OpenApiOperation("Create a new user", "")]
    public async Task<IActionResult> CreateAsync(CreateUserCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Roles = $"{AppConsts.SuperAdminRoleName}")]
    [HttpPut("{id:int}")]
    [OpenApiOperation("Update a user", "")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateUserCommand request)
    {
        request.SetId(id);
        var result = await Mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Roles = $"{AppConsts.SuperAdminRoleName}")]
    [HttpDelete("{id:int}")]
    [OpenApiOperation("Delete user", "")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        return Ok();
    }

    [Authorize(Roles = $"{AppConsts.SuperAdminRoleName}")]
    [HttpPost("search")]
    [OpenApiOperation("Get users by conditions", "")]
    public async Task<IActionResult> GetAsync(GetUserByConditionQuery request)
    {
        var result = await Mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Roles = $"{AppConsts.SuperAdminRoleName}")]
    [HttpGet("{id:int}")]
    [OpenApiOperation("Get user details by id", "")]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await Mediator.Send(new GetUserByIdQuery
        {
            Id = id
        }));
    }

    [Authorize(Roles = $"{AppConsts.SuperAdminRoleName}")]
    [HttpGet("current-user")]
    [OpenApiOperation("Get current user detail", "")]
    public async Task<IActionResult> GetCurrentUserDetail()
    {
        return Ok(await Mediator.Send(new GetUserByIdQuery
        {
            Id = currentUser.UserId
        }));
    }

    [Authorize(Roles = $"{AppConsts.SuperAdminRoleName}")]
    [HttpGet("users-by-role")]
    [OpenApiOperation("Get users by role", "")]
    public async Task<IActionResult> GetUserByRole([FromQuery] GetUserByRoleQuery request)
    {
        return Ok(await Mediator.Send(request));
    }

    [Authorize(Roles = $"{AppConsts.SuperAdminRoleName}")]
    [HttpPost("{id:long}/change-password")]
    [OpenApiOperation("Update a user", "")]
    public async Task<IActionResult> UpdateAsync(long id, UpdatePasswordCommand request)
    {
        request.SetUserId(id);
        var result = await Mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Roles = $"{AppConsts.SuperAdminRoleName}")]
    [HttpPost("change-password")]
    [OpenApiOperation("Update current user password", "")]
    public async Task<IActionResult> UpdateCurrentUserPasswordAsync(UpdatePasswordCommand request)
    {
        request.SetUserId(currentUser.UserId);
        var result = await Mediator.Send(request);
        return Ok(result);
    }
}