using Application.Common.Exceptions;
using Application.Dto.Chat.Conversations;
using Application.Dto.Chat.Messages;
using Application.Dto.Persistence.Catalog.User;
using Application.Interfaces.Infrastructures.Repositories;
using Domain.Entities.Chat;
using EfCore.Persistence.Context;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;

namespace Infrastructure.Repositories;

public class MessageReadRepository : IMessageReadRepository
{
    private readonly ApplicationDbContext _db;

    public MessageReadRepository(ApplicationDbContext db) => _db = db;

    public async Task<MessagesConversationResponse> GetMessagesAsync(long conversationId, DateTimeOffset? before, int pageSize)
    {
        var conversation = _db.Conversations.Include(m => m.Members)
            .FirstOrDefault(c => c.Id == conversationId);

        if (conversation == null)
        {
            throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Conversation), conversationId));
        }

        var query = _db.Messages.Where(m => m.ConversationId == conversationId);

        if (before != null)
        {
            query = query.Where(m => m.CreatedOn < before);
        }

        var messages = await query.OrderByDescending(m => m.CreatedOn)
            .Take(pageSize + 1)
            .Include(m => m.Sender)
            .Include(m => m.Attachments)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,
                Content = m.Content,
                SentAt = m.SentAt,
                Attachments = m.Attachments.Adapt<List<MessageAttachmentDto>>(),
                Sender = m.Sender.Adapt<SortUserInfo>()
            }).ToListAsync();

        var hasMore = messages.Count > pageSize;

        if (hasMore)
        {
            messages.RemoveAt(messages.Count - 1);
        }

        return new MessagesConversationResponse
        {
            Id = conversation.Id,
            Type = conversation.Type,
            Name = conversation.Name,
            Messages = messages,
            Members = conversation.Members.Adapt<List<ConversationMemberDto>>(),
            NextCursor = messages.LastOrDefault()?.SentAt,
            HasMore = hasMore
        };
    }
}