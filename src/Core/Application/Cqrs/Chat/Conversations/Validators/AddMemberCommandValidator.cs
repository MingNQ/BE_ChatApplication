using Application.Cqrs.Chat.Conversations.Commands;
using FluentValidation;

namespace Application.Cqrs.Chat.Conversations.Validators;

public class AddMemberCommandValidator : AbstractValidator<AddMemberCommand>;