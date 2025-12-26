using Application.Common.Interfaces;
using Domain.Common.Enums;

namespace Application.Dto.Persistence.Catalog.FileStorages;

public class FileStorageDto : IDto
{
    public long Id { get; set; }
    public string? FileName { get; set; }
    public string? FileUniqueName { get; set; }
    public decimal? Size { get; set; }
    public string? Type { get; set; }
    public string? Path { get; set; }
    public string? FullPathUrl { get; set; }
    public string? Extension { get; set; }
    public string? Module { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public FileStorageStatus Status { get; set; }
}

public class FileStorageDetailDto : FileStorageDto
{
    public DateTimeOffset CreatedOn { get; set; }
    // Additional properties can be added here if needed for detailed view
}