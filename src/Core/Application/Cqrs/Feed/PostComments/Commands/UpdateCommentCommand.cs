using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Feed;
using Domain.Entities.Feed;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;

namespace Application.Cqrs.Feed.PostComments.Commands;

public class UpdateCommentCommand : BaseCommentCommand, IRequest<PostDto>
{
    public long CommentId { get; set; }
}

public class UpdateCommentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCommentCommand, PostDto>
{
    private readonly IWriteRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();

    public async Task<PostDto> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == request.PostId,
            include: x => x.Include(p => p.Comments),
            disableTracking: false) ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Post), request.PostId));

        var postComment = post.Comments.Where(x => x.Id == request.CommentId);

        _postRepository.Update(post);
        await unitOfWork.SaveChangesAsync();

        return post.Adapt<PostDto>();
    }
}