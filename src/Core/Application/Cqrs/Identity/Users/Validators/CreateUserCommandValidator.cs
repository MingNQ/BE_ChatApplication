using Application.Cqrs.Identity.Users.Commands;
using FluentValidation;

namespace Application.Cqrs.Identity.Users.Validators;

public class CreateUserCommandValidator : UserValidatorBase<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required for new users")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters");
    }
}