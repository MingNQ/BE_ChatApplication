using Application.Cqrs.Social.Friendships.Commands;
using FluentValidation;

namespace Application.Cqrs.Social.Friendships.Validators;

public class CancelFriendRequestCommandValidator : AbstractValidator<CancelFriendRequestCommand>
{
    public CancelFriendRequestCommandValidator()
    {
        RuleFor(x => x.FriendshipRequestId)
            .NotEmpty()
            .WithMessage("This field is required");
    }
}