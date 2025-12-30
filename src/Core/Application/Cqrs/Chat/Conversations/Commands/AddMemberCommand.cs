using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Chat.Conversations;
using Domain.Entities.Chat;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using System.Text.Json.Serialization;

namespace Application.Cqrs.Chat.Conversations.Commands;

public class AddMemberCommand : IRequest<ConversationDto>
{
    [JsonIgnore]
    public long ConversationId { get; private set; }

    public long UserId { get; set; }

    public AddMemberCommand SetConversationId(long conversationId)
    {
        ConversationId = conversationId;
        return this;
    }
}

public class AddMemberCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<AddMemberCommand, ConversationDto>
{
    private readonly IWriteRepository<Conversation> _conversationRepository = unitOfWork.GetRepository<Conversation>();
    private readonly IWriteRepository<ConversationRole> _conversationRoleRepository = unitOfWork.GetRepository<ConversationRole>();

    public async Task<ConversationDto> Handle(AddMemberCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetFirstOrDefaultAsync(
            predicate: c => c.Id == request.ConversationId,
            include: c => c.Include(c => c.Members),
            disableTracking: false) ?? throw new InvalidOperationException("Conversation not found.");

        var memberRole = await _conversationRoleRepository.GetFirstOrDefaultAsync(
            predicate: r => r.NormalizedName == AppConsts.MemberConversationRoleName.ToUpperInvariant(),
            disableTracking: true) ?? throw new NotFoundException(CustomResponseMessage.RoleDoesNotExist);

        var conversationMember = ConversationMember.Create(request.ConversationId, request.UserId, currentUser.GetUserId());
        conversationMember.AssignRole(memberRole.Id);
        conversation.AddMember(conversationMember);

        _conversationRepository.Update(conversation);
        await unitOfWork.SaveChangesAsync();

        return conversation.Adapt<ConversationDto>();
    }
}