using Application.Common.Exceptions;
using Application.Common.Repositories;
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

public class DeleteCommentCommandHandler(IUnitOfWork unitOfWork)
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
                .Include(p => p.Author!),
            disableTracking: false)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Post), request.PostId));
        var postComment = post.Comments.Where(x => x.Id == request.CommentId).FirstOrDefault();

        if (postComment != null)
        {
            post.RemoveComment(postComment);
        }

        _postRepository.Update(post);
        await unitOfWork.SaveChangesAsync();

        return post.Adapt<PostDto>();
    }
}