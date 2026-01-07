using Application.Cqrs.Social.Friendships.Commands;
using FluentValidation;

namespace Application.Cqrs.Social.Friendships.Validators;

public class AddFriendRequestCommandValidator : AbstractValidator<AddFriendRequestCommand>
{
    public AddFriendRequestCommandValidator()
    {
        RuleFor(x => x.FriendId)
            .NotEmpty()
            .WithMessage("This field is required");
    }
}