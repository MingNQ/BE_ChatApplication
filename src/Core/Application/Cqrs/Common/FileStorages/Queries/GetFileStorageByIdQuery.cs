using Application.Common.Exceptions;
using Application.Common.Persistence;
using Application.Common.Services;
using Application.Cqrs.Common.FileStorages.Specs;
using Application.Dto.Persistence.Catalog.FileStorages;
using Domain.Entities.Common;
using MediatR;
using Shared.Constants;

namespace Application.Cqrs.Common.FileStorages.Queries;

public class GetFileStorageByIdQuery : IRequest<FileStorageDto>
{
    public long Id { get; set; }
}

public class GetFileStorageByIdQueryHandler(
    IReadRepository<FileStorage> fileStorageRepository,
    IFilePathService filePathService) : IRequestHandler<GetFileStorageByIdQuery, FileStorageDto>
{
    public async Task<FileStorageDto> Handle(GetFileStorageByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new FileStorageByIdSpec(request.Id);
        var fileStorage = await fileStorageRepository.FirstOrDefaultAsync(spec, cancellationToken)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(FileStorage), request.Id));
        filePathService.BindFullPaths(fileStorage);

        return fileStorage;
    }
}