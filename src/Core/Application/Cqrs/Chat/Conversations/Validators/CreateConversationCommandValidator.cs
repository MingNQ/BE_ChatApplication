using Application.Cqrs.Chat.Conversations.Commands;
using Domain.Common.Enums;
using FluentValidation;

namespace Application.Cqrs.Chat.Conversations.Validators;

public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Conversation name is required.")
            .When(x => x.Type == ConversationType.Group);

        RuleFor(x => x.MemberIds)
            .Must(memberIds => memberIds == null || memberIds.Count >= 2)
            .WithMessage("At least two members are required for a conversation.")
            .When(x => x.Type == ConversationType.Group)
            .Must(memberIds => memberIds == null || memberIds.Distinct().Count() == memberIds.Count)
            .WithMessage("Member IDs must be unique.");
    }
}