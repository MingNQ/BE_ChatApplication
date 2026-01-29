namespace Application.Cqrs.Common.FileStorages.Commands;

public class FileStorageBaseCommand
{
    public string? FileName { get; set; }
    public string? FileUniqueName { get; set; }
    public decimal? Size { get; set; }
    public string? Type { get; set; }
    public string? Path { get; set; }
    public string? FullPath { get; set; }
    public string? Extension { get; set; }
}