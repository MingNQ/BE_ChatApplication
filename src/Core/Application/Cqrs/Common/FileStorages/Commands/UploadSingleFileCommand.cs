using Application.Common.Services;
using Application.Dto.Persistence.Catalog.FileStorages;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Cqrs.Common.FileStorages.Commands;

public class UploadSingleFileCommand : IRequest<FileStorageDto>
{
    public IFormFile FileData { get; set; } = default!;

    public UploadSingleFileCommand()
    { }

    public UploadSingleFileCommand(IFormFile fileData)
    {
        FileData = fileData;
    }
}

public class UploadSingleFileCommandHandler(IFileStorageService fileStorageService)
    : IRequestHandler<UploadSingleFileCommand, FileStorageDto>
{
    public async Task<FileStorageDto> Handle(UploadSingleFileCommand request, CancellationToken cancellationToken)
    {
        var fileStorage = await fileStorageService.UploadFileAsync(request.FileData);

        return fileStorage;
    }
}