using Application.Cqrs.Feed.PostReactions.Commands;
using FluentValidation;

namespace Application.Cqrs.Feed.PostReactions.Validators;

public class ReactionCommandValidatorBase<T> : AbstractValidator<T> where T : BaseReactionCommand
{
    public ReactionCommandValidatorBase()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("This field is required");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("This field is required");
    }
}