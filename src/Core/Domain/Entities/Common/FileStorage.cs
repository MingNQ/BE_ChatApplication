using System.ComponentModel.DataAnnotations;
using Domain.Common.Contracts;
using Domain.Common.Enums;

namespace Domain.Entities.Catalog;

public class FileStorage : AuditableEntity<long>, IAggregateRoot
{
    [MaxLength(255)]
    public string? FileName { get; set; }

    [MaxLength(255)]
    public string? FileUniqueName { get; set; }

    public decimal? Size { get; set; }

    [MaxLength(255)]
    public string? Type { get; set; }

    [MaxLength(255)]
    public string? Path { get; set; }

    [MaxLength(255)]
    public string? Extension { get; set; }

    [MaxLength(255)]
    public string? Module { get; set; }

    [MaxLength(255)]
    public string? DocumentType { get; set; }

    public FileStorageStatus Status { get; set; }

    public static FileStorage Create(string? fileName, string? fileUniqueName, decimal? size,
        string? type, string? path, string? extension, string? module, string? documentType)
    {
        return new FileStorage
        {
            FileName = fileName,
            FileUniqueName = fileUniqueName,
            Size = size,
            Type = type,
            Path = path,
            Extension = extension,
            Module = module,
            DocumentType = documentType,
            Status = FileStorageStatus.Draft
        };
    }

    public void Update(string? fileName, string? fileUniqueName, decimal? size, string? type,
        string? path, string? extension, string? module, string? documentType)
    {
        FileName = fileName;
        FileUniqueName = fileUniqueName;
        Size = size;
        Type = type;
        Path = path;
        Extension = extension;
        Module = module;
        DocumentType = documentType;
    }

    public void UpdateStatus(FileStorageStatus status)
    {
        Status = status;
    }
}