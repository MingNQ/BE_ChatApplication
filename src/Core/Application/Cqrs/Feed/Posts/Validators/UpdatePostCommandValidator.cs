using Application.Cqrs.Feed.Posts.Commands;
using FluentValidation;

namespace Application.Cqrs.Feed.Posts.Validators;

public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required.");

        RuleFor(x => x.Visibility)
            .IsInEnum()
            .WithMessage("Visibility must be a valid value.");
    }
}