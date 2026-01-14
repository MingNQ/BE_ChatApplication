using Application.Cqrs.Feed.PostComments.Commands;
using Application.Cqrs.Feed.PostReactions.Commands;
using Application.Cqrs.Feed.Posts.Commands;
using Application.Cqrs.Feed.Posts.Queries;
using Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Shared.Constants;

namespace Host.Controllers.Client;

[ControllerName("posts")]
[Tags("Feed|Posts")]
public class PostController : BaseClientAuthController
{
    [HttpPost("search")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetAsync([FromBody] GetPostQuery request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.GetDataSuccess);
    }

    [HttpGet("{id:long}")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var result = await Mediator.Send(new GetPostByIdQuery
        {
            Id = id
        });
        return Ok(result, MessageCommon.GetDataSuccess);
    }

    [HttpGet("user/{userId:long}")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetByUserIdAsync(long userId)
    {
        var result = await Mediator.Send(new GetPostByUserIdQuery
        {
            UserId = userId
        });
        return Ok(result, MessageCommon.GetDataSuccess);
    }

    [HttpGet("me")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetMyPostAsync()
    {
        var result = await Mediator.Send(new GetMyPostQuery());
        return Ok(result, MessageCommon.GetDataSuccess);
    }

    [HttpPost("")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> CreatePostAsync(CreatePostCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.CreateSuccess);
    }

    [HttpDelete("{id:long}")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> DeletePostAsync([FromRoute] long id)
    {
        var request = new DeletePostCommand();
        request.SetId(id);
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.DeleteSuccess);
    }

    [HttpPut("{id:long}")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> UpdatePostAsync(long id, UpdatePostCommand request)
    {
        request.SetId(id);
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.UpdateSuccess);
    }

    [HttpPost("comment")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> CreateCommentAsync(CreateCommentCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.CreateSuccess);
    }

    [HttpPut("comment")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> UpdateCommentAsync(UpdateCommentCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.UpdateSuccess);
    }

    [HttpDelete("comment")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> DeleteCommentAsync(DeleteCommentCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.DeleteSuccess);
    }

    [HttpPost("reaction")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> CreateReactionAsync(CreateReactionCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.CreateSuccess);
    }

    [HttpPut("reaction")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> UpdateReactionAsync(UpdateReactionCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.UpdateSuccess);
    }

    [HttpDelete("reaction")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> DeleteReactionAsync(DeleteReactionCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.DeleteSuccess);
    }
}