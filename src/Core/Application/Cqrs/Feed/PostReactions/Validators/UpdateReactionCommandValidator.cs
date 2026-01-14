using Application.Cqrs.Feed.PostReactions.Commands;
using FluentValidation;

namespace Application.Cqrs.Feed.PostReactions.Validators;

public class UpdateReactionCommandValidator : ReactionCommandValidatorBase<UpdateReactionCommand>
{
    public UpdateReactionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("This field is required");
    }
}