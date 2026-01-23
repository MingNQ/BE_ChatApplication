using Application.Common.Persistence;
using Application.Cqrs.Chat.Conversations.Specs;
using Domain.Entities.Chat;
using MediatR;

namespace Application.Cqrs.Chat.Conversations.Queries;

public class GetUserConversationIdsQuery : IRequest<List<long>>
{
    public long UserId { get; set; }
}

public class GetUserConversastionIdsQueryHandler(IReadRepository<Conversation> conversationRepository)
    : IRequestHandler<GetUserConversationIdsQuery, List<long>>
{
    public async Task<List<long>> Handle(GetUserConversationIdsQuery request, CancellationToken cancellationToken)
    {
        var spec = new UserConversationIdsSpec(request.UserId);
        var conversations = await conversationRepository.ListAsync(spec, cancellationToken);

        return conversations.Select(x => x.Id).ToList();
    }
}