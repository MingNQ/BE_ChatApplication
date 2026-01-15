using Application.Cqrs.Feed.Posts.Commands;
using FluentValidation;

namespace Application.Cqrs.Feed.Posts.Validators;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Post content must not be empty.");

        RuleFor(x => x.Visibility)
            .IsInEnum()
            .WithMessage("Invalid post visibility.");
    }
}