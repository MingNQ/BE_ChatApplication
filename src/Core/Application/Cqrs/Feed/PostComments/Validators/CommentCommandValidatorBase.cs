using Application.Cqrs.Feed.PostComments.Commands;
using FluentValidation;

namespace Application.Cqrs.Feed.PostComments.Validators;

public class CommentCommandValidatorBase<T> : AbstractValidator<T> where T : BaseCommentCommand
{
    public CommentCommandValidatorBase()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("This field is required");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("This field is required");
    }
}