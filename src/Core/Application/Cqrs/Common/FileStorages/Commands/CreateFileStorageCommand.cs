using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Persistence.Catalog.FileStorages;
using Domain.Entities.Common;
using Mapster;
using MediatR;

namespace Application.Cqrs.Common.FileStorages.Commands;

public class CreateFileStorageCommand : FileStorageBaseCommand, IRequest<FileStorageDto>;

public class CreateFileStorageCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateFileStorageCommand, FileStorageDto>
{
    private readonly IWriteRepository<FileStorage> _fileStorageRepository = unitOfWork.GetRepository<FileStorage>();

    public async Task<FileStorageDto> Handle(CreateFileStorageCommand request, CancellationToken cancellationToken)
    {
        var fileStorage = FileStorage.Create(
            request.FileName,
            request.FileUniqueName,
            request.Size,
            request.Type,
            request.Path,
            request.Extension);

        await _fileStorageRepository.InsertAsync(fileStorage, cancellationToken);
        await unitOfWork.SaveChangesAsync();

        return fileStorage.Adapt<FileStorageDto>();
    }
}