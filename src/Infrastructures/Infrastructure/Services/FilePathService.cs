using Application.Common.Services;
using Application.Dto.Persistence.Catalog.FileStorages;
using Application.Dto.Persistence.Catalog.User;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class FilePathService(IConfiguration configuration) : IFilePathService
{
    public string GetFileStorageFullPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        if (path.StartsWith("http://", StringComparison.Ordinal) || path.StartsWith("https://", StringComparison.Ordinal))
        {
            return path;
        }

        string baseUri = configuration["FileStorageSettings:Uri"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(baseUri))
        {
            throw new InvalidOperationException("FileStorageSettings:Uri is not configured.");
        }

        return baseUri + path;
    }

    public TDto? BindFullPaths<TDto>(TDto? dto) where TDto : class
    {
        if (dto == null)
        {
            return dto;
        }

        if (dto is FileStorageDto fileStorageDto)
        {
            string path = fileStorageDto.Path ?? string.Empty;
            fileStorageDto.FullPathUrl = GetFileStorageFullPath(path);
            return fileStorageDto as TDto;
        }

        if (dto is UserDto userDto)
        {
            userDto.Avatar = BindFullPaths(userDto.Avatar);
            return userDto as TDto;
        }

        // Add handling for other DTOs if needed

        return dto;
    }

    public List<TDto?> BindFullPaths<TDto>(List<TDto> dtos) where TDto : class
    {
        return dtos.Select(BindFullPaths).ToList();
    }
}