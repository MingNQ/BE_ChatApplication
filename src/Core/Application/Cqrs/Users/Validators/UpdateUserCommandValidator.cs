using Application.Cqrs.Users.Commands;
using FluentValidation;

namespace Application.Cqrs.Users.Validators;

public class UpdateUserCommandValidator : UserValidatorBase<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");
    }
}