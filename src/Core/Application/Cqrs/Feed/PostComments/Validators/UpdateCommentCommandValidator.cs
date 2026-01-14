using Application.Cqrs.Feed.PostComments.Commands;
using FluentValidation;

namespace Application.Cqrs.Feed.PostComments.Validators;

public class UpdateCommentCommandValidator : CommentCommandValidatorBase<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.CommentId)
            .NotEmpty()
            .WithMessage("This field is required");
    }
}