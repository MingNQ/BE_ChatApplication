using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Feed;
using Domain.Common.Enums;
using Domain.Entities.Feed;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using System.Text.Json.Serialization;

namespace Application.Cqrs.Feed.Posts.Commands;

public class UpdatePostCommand : IRequest<PostDto>
{
    [JsonIgnore]
    public long Id { get; private set; }

    public void SetId(long id) => Id = id;

    public PostVisibilityEnum? Visibility { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<long> AttachmentIds { get; set; } = [];
}

public class UpdatePostCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<UpdatePostCommand, PostDto>
{
    private readonly IWriteRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();

    public async Task<PostDto> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetFirstOrDefaultAsync(
            predicate: p => p.Id == request.Id,
            include: q => q
                .Include(p => p.Attachments)
                .Include(p => p.Reactions)
                .Include(p => p.Comments),
            disableTracking: false);
        if (post == null || post.AuthorId != currentUser.UserId)
        {
            throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Post), request.Id));
        }

        post.Update(
            request.Content,
            request.Visibility ?? post.Visibility);

        var postAttachments = request.AttachmentIds
            .Select(id => PostAttachment.Create(post.Id, id))
            .ToList();

        post.UpdateAttachments(postAttachments);

        await unitOfWork.SaveChangesAsync();
        return post.Adapt<PostDto>();
    }
}