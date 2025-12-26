using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Application.Common.Repositories;
using Application.Common.Services;
using Application.Common.UnitOfWork;
using Application.Configurations;
using Application.Dto.Persistence.Catalog.FileStorages;
using Domain.Entities.Catalog;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class FileStorageService(
    IUnitOfWork unitOfWork,
    IFilePathService filePathService,
    IWebHostEnvironment environment,
    IOptions<FileStorageSettingsOptions> fileStorageSettingsOptions) : IFileStorageService
{
    private readonly FileStorageSettingsOptions _fileStorageSettingsOptions = fileStorageSettingsOptions.Value;
    private readonly IWriteRepository<FileStorage> _fileStorageRepository = unitOfWork.GetRepository<FileStorage>();
    private readonly HttpClient _httpClient = new();

    public async Task<FileStorageDto> UploadFileAsync(IFormFile file, string module)
    {
        FileUploadValidator(file);

        string uploadFolder = _fileStorageSettingsOptions.FullPath;
        string basePath = _fileStorageSettingsOptions.Path;

        if (environment.IsDevelopment())
        {
            string contentRoot = environment.ContentRootPath;
            uploadFolder = Path.Combine(contentRoot, basePath.TrimStart('/'));

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }
        }

        if (string.IsNullOrWhiteSpace(uploadFolder))
        {
            throw new BadHttpRequestException("FullPath is not configured.");
        }

        string uniqueFileName = HandleFileUniqueName(Path.GetFileNameWithoutExtension(file.FileName), Path.GetExtension(file.FileName).ToLowerInvariant());

        // Create directory structure: module/{year}/{month}
        var now = DateTime.UtcNow;
        string year = now.Year.ToString();
        string month = now.Month.ToString("D2"); // Ensure 2-digit month format

        // Create the relative path structure
        string relativePath = Path.Combine(module, year, month);
        string fullDirectoryPath = Path.Combine(uploadFolder, relativePath);

        // Ensure directory exists
        Directory.CreateDirectory(fullDirectoryPath);

        // Full file path
        string filePath = Path.Combine(fullDirectoryPath, uniqueFileName);

        // Save file to disk
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Create the path for database storage
        string dbPath = $"{basePath}/{relativePath}".Replace("\\", "/");

        var result = await CreateFileStorageAsync(file, uniqueFileName, dbPath, module);

        filePathService.BindFullPaths(result);

        return result;
    }

    private void FileUploadValidator(IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new BadHttpRequestException("No file uploaded.");
        }

        // Validate file size (5MB limit)
        const long maxFileSize = 5 * 1024 * 1024; // 5MB in bytes
        if (file.Length > maxFileSize)
        {
            throw new BadHttpRequestException("File size exceeds 5MB limit.");
        }

        string[] allowedTypes = ["image/jpeg", "image/jpg", "image/png", "image/webp"];
        if (!allowedTypes.Contains(file.ContentType))
        {
            throw new BadHttpRequestException("Only JPG, PNG and WebP image files are allowed.");
        }
    }

    private async Task<FileStorageDto> CreateFileStorageAsync(IFormFile file, string uniqueName, string path, string module)
    {
        var fileStorage = FileStorage.Create(
            file?.FileName,
            uniqueName,
            file?.Length,
            file?.ContentType,
            $"{path}/{uniqueName}",
            Path.GetExtension(file?.FileName),
            module,
            module);

        var result = await _fileStorageRepository.InsertAsync(fileStorage);
        await unitOfWork.SaveChangesAsync();

        return result.Entity.Adapt<FileStorageDto>();
    }

    private string HandleFileUniqueName(string originalFileName, string extension)
    {
        string normalized = originalFileName.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (char c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        string noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

        noDiacritics = noDiacritics.ToLowerInvariant();

        string safeName = Regex.Replace(noDiacritics, @"[^a-z0-9]+", "-");

        safeName = safeName.Trim('-');

        string datePart = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        string uniqueFileName = $"{safeName}-{datePart}{extension}";

        return uniqueFileName;
    }

    public async Task<FileStorageDto> CreateFileStorageFromUrlAsync(string linkUrl, string module)
    {
        if (string.IsNullOrWhiteSpace(linkUrl))
        {
            throw new BadHttpRequestException("Link URL cannot be empty.");
        }
        if (!System.Uri.TryCreate(linkUrl, UriKind.Absolute, out var uri))
        {
            throw new BadHttpRequestException("Invalid URL format.");
        }

        try
        {
            // Get file information from URL
            using var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            string fileName = GetFileNameFromUrl(uri);
            string contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            long fileSize = response.Content.Headers.ContentLength ?? 0;
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            // Generate unique filename
            string uniqueFileName = HandleFileUniqueName(fileName, extension);

            var fileStorage = FileStorage.Create(
                fileName,
                uniqueFileName,
                fileSize,
                contentType,
                linkUrl,
                extension,
                module,
                module);

            var result = await _fileStorageRepository.InsertAsync(fileStorage);
            await unitOfWork.SaveChangesAsync();

            var fileStorageDto = result.Entity.Adapt<FileStorageDto>();

            return fileStorageDto;
        }
        catch (HttpRequestException ex)
        {
            throw new BadHttpRequestException($"Failed to access the URL: {ex.Message}");
        }
    }

    private string GetFileNameFromUrl(System.Uri uri)
    {
        string fileName = Path.GetFileName(uri.LocalPath);

        if (string.IsNullOrWhiteSpace(fileName) || !Path.HasExtension(fileName))
        {
            // If no filename or extension, generate a default name
            fileName = $"file_{DateTime.UtcNow:yyyyMMdd_HHmmss}.jpg";
        }

        return fileName;
    }
}