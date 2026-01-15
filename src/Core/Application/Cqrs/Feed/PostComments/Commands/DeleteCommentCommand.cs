using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Domain.Entities.Feed;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using System.Text.Json.Serialization;

namespace Application.Cqrs.Feed.PostComments.Commands;

public class DeleteCommentCommand : IRequest<bool>
{
    public long PostId { get; set; }

    [JsonIgnore]
    public long CommentId { get; private set; }

    public void SetId(long id)
    {
        CommentId = id;
    }
}

public class DeleteCommentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCommentCommand, bool>
{
    private readonly IWriteRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();

    public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == request.PostId,
            include: x => x.Include(p => p.Comments),
            disableTracking: false)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Post), request.PostId));
        var postComment = post.Comments.Where(x => x.Id == request.CommentId).FirstOrDefault();

        if (postComment != null)
        {
            post.RemoveComment(postComment);
        }
        else
        {
            return false;
        }

        return true;
    }
}