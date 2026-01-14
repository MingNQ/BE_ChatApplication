using Application.Cqrs.Feed.Posts.Commands;
using FluentValidation;

namespace Application.Cqrs.Feed.Posts.Validators;

public class DeletePostCommandValidator : AbstractValidator<DeletePostCommand>
{
    public DeletePostCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Post ID is required.")
            .GreaterThan(0).WithMessage("Post ID must be greater than 0.");
    }
}