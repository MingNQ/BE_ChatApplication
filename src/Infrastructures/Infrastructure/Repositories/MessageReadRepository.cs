using Application.Common.Exceptions;
using Application.Common.Services;
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

public class MessageReadRepository(
    ApplicationDbContext _db,
    IFilePathService filePathService)
    : IMessageReadRepository
{
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
                .ThenInclude(a => a.FileStorage)
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

        foreach (var message in messages)
        {
            if (message.Attachments.Count > 0)
            {
                var attachments = message.Attachments.Select(a => a.FileStorage!).ToList();
                filePathService.BindFullPaths(attachments);
            }
        }

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