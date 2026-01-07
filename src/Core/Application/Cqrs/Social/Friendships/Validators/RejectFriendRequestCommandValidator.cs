using Application.Cqrs.Social.Friendships.Commands;
using FluentValidation;

namespace Application.Cqrs.Social.Friendships.Validators;

public class RejectFriendRequestCommandValidator : AbstractValidator<RejectFriendRequestCommand>
{
    public RejectFriendRequestCommandValidator()
    {
        RuleFor(x => x.FriendshipRequestId)
            .NotEmpty()
            .WithMessage("This field is required");
    }
}