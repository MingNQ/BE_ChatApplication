using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.Services;
using Application.Common.UnitOfWork;
using Application.Dto.Feed;
using Domain.Entities.Feed;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using System.Text.Json.Serialization;

namespace Application.Cqrs.Feed.PostComments.Commands;

public class DeleteCommentCommand : IRequest<PostDto>
{
    [JsonIgnore]
    public long PostId { get; private set; }

    [JsonIgnore]
    public long CommentId { get; private set; }

    public void SetId(long postId, long commentId)
    {
        CommentId = commentId;
        PostId = postId;
    }
}

public class DeleteCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IFilePathService filePathService)
    : IRequestHandler<DeleteCommentCommand, PostDto>
{
    private readonly IWriteRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();

    public async Task<PostDto> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == request.PostId,
            include: x => x.Include(p => p.Comments)
                        .ThenInclude(c => c.User)
                    .Include(p => p.Reactions)
                    .Include(p => p.Author!)
                    .Include(p => p.Attachments)
                        .ThenInclude(a => a.Attachment!),
            disableTracking: false)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Post), request.PostId));
        var postComment = post.Comments.Where(x => x.Id == request.CommentId).FirstOrDefault();

        if (postComment != null)
        {
            post.RemoveComment(postComment);
        }

        _postRepository.Update(post);
        await unitOfWork.SaveChangesAsync();
        var postDto = post.Adapt<PostDto>();

        if (postDto.Attachments.Count > 0)
        {
            var attachments = postDto.Attachments.Select(a => a.Attachment!).ToList();
            filePathService.BindFullPaths(attachments);
        }
        return postDto;
    }
}