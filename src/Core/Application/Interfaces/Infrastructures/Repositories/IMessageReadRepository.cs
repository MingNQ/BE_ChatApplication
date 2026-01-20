using Application.Dto.Chat.Messages;

namespace Application.Interfaces.Infrastructures.Repositories;

public interface IMessageReadRepository
{
    public Task<MessagesConversationResponse> GetMessagesAsync(long conversationId, DateTimeOffset? before, int pageSize);
}