using Application.Cqrs.Users.Commands;
using FluentValidation;

namespace Application.Cqrs.Users.Validators;

public class UserValidatorBase<T> : AbstractValidator<T> where T : UserBaseCommand
{
    public UserValidatorBase()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address")
            .MaximumLength(256)
            .WithMessage("Email must not exceed 256 characters");

        RuleFor(x => x.FirstName)
            .MaximumLength(50)
            .WithMessage("First name must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(50)
            .WithMessage("Last name must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.LastName));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(15)
            .WithMessage("Phone number must not exceed 15 characters")
            .Matches(@"^\+?[0-9\s\-\(\)]+$")
            .WithMessage("Phone number format is invalid")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.RegisterProvider)
            .MaximumLength(50)
            .WithMessage("Register provider must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.RegisterProvider));
    }
}