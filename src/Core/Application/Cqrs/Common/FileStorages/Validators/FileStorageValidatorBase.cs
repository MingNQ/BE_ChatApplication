using Application.Cqrs.Common.FileStorages.Commands;
using FluentValidation;

namespace Application.Cqrs.Common.FileStorages.Validators;

public class FileStorageValidatorBase<T> : AbstractValidator<T> where T : FileStorageBaseCommand
{
    public FileStorageValidatorBase()
    {
        CommonRules();
    }

    private void CommonRules()
    {
        RuleFor(x => x.FileName)
            .MaximumLength(255);
        RuleFor(x => x.FileUniqueName)
            .MaximumLength(255);
        RuleFor(x => x.Type)
            .MaximumLength(255);
        RuleFor(x => x.Path)
            .MaximumLength(255);
        RuleFor(x => x.Extension)
            .MaximumLength(255);
    }
}