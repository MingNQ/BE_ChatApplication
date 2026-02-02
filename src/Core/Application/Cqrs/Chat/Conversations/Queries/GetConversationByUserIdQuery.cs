using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Chat.Conversations.Specs;
using Application.Dto.Chat.Conversations;
using Application.Dto.Chat.Messages;
using Domain.Entities.Chat;
using MediatR;

namespace Application.Cqrs.Chat.Conversations.Queries;

public class GetConversationByUserIdQuery : IRequest<List<RecentConversationDto>>;

public class GetConversationQueryHandler(
    IReadRepository<Conversation> conversationRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetConversationByUserIdQuery, List<RecentConversationDto>>
{
    public async Task<List<RecentConversationDto>> Handle(GetConversationByUserIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new ConversationByUserIdSpec(currentUser.UserId);
        var conversations = await conversationRepository.ListAsync(spec, cancellationToken);

        var result = conversations
            .Select(c =>
            {
                var lastMessage = c.Messages?.LastOrDefault();
                var lastMessageSentAt = lastMessage?.SentAt ?? default;
                var (lastMessageContent, lastMessageContentkey) = lastMessage is null ? (string.Empty, null) : GetLastMessageContent(lastMessage);

                return new RecentConversationDto
                {
                    Id = c.Id,
                    Type = c.Type,
                    Name = c.Name,
                    LastMessageContent = lastMessageContent,
                    LastMessageSentAt = lastMessageSentAt,
                    LastMessageContentKey = lastMessageContentkey,
                    UnreadMessagesCount = 0,
                    Members = c.Members
                };
            })
            .OrderByDescending(x => x.LastMessageSentAt)
            .ToList();

        return result;
    }

    private static (string, string) GetLastMessageContent(MessageDto lastMessage)
    {
        var attachmentType = lastMessage.Attachments?.LastOrDefault()?.FileStorage?.Type;

        if (!string.IsNullOrEmpty(attachmentType))
        {
            return attachmentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)
                ? ("Have sent an image", "chat.sentImage")
                : ("Have sent an attachment", "chat.sentAttachment");
        }

        return !string.IsNullOrEmpty(lastMessage.Content) ? (lastMessage.Content, "") : (string.Empty, "");
    }
}