using Application.Cqrs.Common.FileStorages.Commands;
using FluentValidation;

namespace Application.Cqrs.Common.FileStorages.Validators;

public class UploadSingleFileValidator : AbstractValidator<UploadSingleFileCommand>
{
    public UploadSingleFileValidator()
    {
        RuleFor(p => p.FileData)
            .NotNull()
            .WithMessage("File is required")
            .Must(x => x is { Length: > 0 })
            .WithMessage("File cannot be empty")
            .When(x => x.FileData != null);
    }
}