using Application.Dto.Persistence.Catalog.FileStorages;
using Ardalis.Specification;
using Domain.Entities.Common;

namespace Application.Cqrs.Common.FileStorages.Specs;

public class FileStorageByIdSpec : Specification<FileStorage, FileStorageDto>
{
    public FileStorageByIdSpec(long id)
    {
        Query.Where(fs => fs.Id == id);
    }
}