using Application.Common.Services;
using Application.Dto.Persistence.Catalog.FileStorages;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Cqrs.Common.FileStorages.Commands;

public class UploadMultipleFileCommand : IRequest<List<FileStorageDto>>
{
    public IReadOnlyList<IFormFile> Files { get; set; } = [];
}

public class UploadMultipleFileCommandHandler(IFileStorageService fileStorageService)
    : IRequestHandler<UploadMultipleFileCommand, List<FileStorageDto>>
{
    public async Task<List<FileStorageDto>> Handle(UploadMultipleFileCommand request, CancellationToken cancellationToken)
    {
        var fileStorages = new List<FileStorageDto>();

        foreach (var file in request.Files)
        {
            var fileStorageDto = await fileStorageService.UploadFileAsync(file);
            fileStorages.Add(fileStorageDto);
        }

        return fileStorages;
    }
}