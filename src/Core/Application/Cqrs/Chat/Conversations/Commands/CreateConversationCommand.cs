using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Chat.Conversations;
using Domain.Common.Enums;
using Domain.Entities.Chat;
using Mapster;
using MediatR;
using Shared.Constants;

namespace Application.Cqrs.Chat.Conversations.Commands;

public class CreateConversationCommand : IRequest<ConversationDto>
{
    public ConversationType Type { get; set; }
    public string? Name { get; set; }
    public List<long>? MemberIds { get; set; }
}

public class CreateConversationCommandHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    : IRequestHandler<CreateConversationCommand, ConversationDto>
{
    private readonly IWriteRepository<Conversation> _conversationRepository = unitOfWork.GetRepository<Conversation>();
    private readonly IWriteRepository<ConversationRole> _conversationRoleRepository = unitOfWork.GetRepository<ConversationRole>();

    public async Task<ConversationDto> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var adminRole = await _conversationRoleRepository.GetFirstOrDefaultAsync(
            predicate: r => r.NormalizedName == AppConsts.AdminConversationRoleName.ToUpperInvariant(),
            disableTracking: true
            ) ?? throw new NotFoundException(CustomResponseMessage.RoleDoesNotExist);

        var memberRole = await _conversationRoleRepository.GetFirstOrDefaultAsync(
            predicate: r => r.NormalizedName == AppConsts.MemberConversationRoleName.ToUpperInvariant(),
            disableTracking: true
            ) ?? throw new NotFoundException(CustomResponseMessage.RoleDoesNotExist);

        var conversation = Conversation.Create(request.Type, request.Name);
        var entity = await _conversationRepository.InsertAsync(conversation, cancellationToken);
        await unitOfWork.SaveChangesAsync();

        var conversationMember = ConversationMember.Create(entity.Entity.Id, currentUser.UserId, null);

        if (request.Type == ConversationType.Group)
        {
            conversationMember.AssignRole(adminRole.Id);
        }
        else
        {
            conversationMember.AssignRole(memberRole.Id);
        }
        conversation.AddMember(conversationMember);

        var members = new List<ConversationMember>();

        foreach (var memberId in request.MemberIds ?? [])
        {
            conversationMember = ConversationMember.Create(entity.Entity.Id, memberId, currentUser.UserId);
            conversationMember.AssignRole(memberRole.Id);
            members.Add(conversationMember);
        }
        conversation.AddMembers(members);

        await unitOfWork.SaveChangesAsync();

        return entity.Entity.Adapt<ConversationDto>();
    }
}