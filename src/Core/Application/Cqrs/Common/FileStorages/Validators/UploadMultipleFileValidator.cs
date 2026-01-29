using Application.Cqrs.Common.FileStorages.Commands;
using FluentValidation;

namespace Application.Cqrs.Common.FileStorages.Validators;

public class UploadMultipleFileValidator : AbstractValidator<UploadMultipleFileCommand>
{
    public UploadMultipleFileValidator()
    {
        RuleFor(p => p.Files)
            .NotNull()
            .NotEmpty()
            .WithMessage("Files is required");
    }
}