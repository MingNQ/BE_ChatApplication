using Application.Cqrs.Common.FileStorages.Commands;
using Application.Cqrs.Common.FileStorages.Queries;
using Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Shared.Constants;

namespace Host.Controllers.Common;

[ControllerName("file-storage")]
[Tags("Common|FileStorage")]
public class FileStorageController : BaseAuthController
{
    [HttpGet("{id:long}")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var result = await Mediator.Send(new GetFileStorageByIdQuery
        {
            Id = id
        });

        return Ok(result, MessageCommon.GetDataSuccess);
    }

    [HttpPost("upload/single")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> UploadSingleFileAsync(UploadSingleFileCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.UploadSuccess);
    }

    [HttpPost("upload/multiple")]
    [OpenApiOperation("", "")]
    public async Task<IActionResult> UploadMultipleFileAsync(UploadMultipleFileCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result, MessageCommon.UploadSuccess);
    }
}