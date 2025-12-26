using Application.Dto.Persistence.Catalog.FileStorages;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Services;

public interface IFileStorageService
{
    Task<FileStorageDto> UploadFileAsync(IFormFile file, string module);
    Task<FileStorageDto> CreateFileStorageFromUrlAsync(string linkUrl, string module);
}