using Application.Cqrs.Social.Friendships.Commands;
using FluentValidation;

namespace Application.Cqrs.Social.Friendships.Validators;

public class AcceptFriendRequestCommandValidator : AbstractValidator<AcceptFriendRequestCommand>
{
    public AcceptFriendRequestCommandValidator()
    {
        RuleFor(x => x.FriendshipRequestId)
            .NotEmpty()
            .WithMessage("This field is required");
    }
}