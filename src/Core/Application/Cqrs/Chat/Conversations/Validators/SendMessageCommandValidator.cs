using Application.Cqrs.Chat.Conversations.Commands;
using FluentValidation;

namespace Application.Cqrs.Chat.Conversations.Validators;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("This field is required");

        RuleFor(x => x.SenderId)
            .NotEmpty()
            .WithMessage("This field is required");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("This field is required")
            .When(x => x.AttachmentIds != null && x.AttachmentIds.Count <= 0);
    }
}