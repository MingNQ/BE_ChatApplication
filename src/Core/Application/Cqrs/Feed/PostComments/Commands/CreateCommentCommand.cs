using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Feed;
using Domain.Entities.Feed;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;

namespace Application.Cqrs.Feed.PostComments.Commands;

public class CreateCommentCommand : BaseCommentCommand, IRequest<PostDto>
{
    public long? RootId { get; set; }
}

public class CreateCommentCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<CreateCommentCommand, PostDto>
{
    private readonly IWriteRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();

    public async Task<PostDto> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var postComment = PostComment.Create(request.PostId, currentUser.UserId, request.Content, request.RootId);
        var post = await _postRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == request.PostId,
            include: x => x.Include(p => p.Comments).ThenInclude(c => c.User)
                    .Include(p => p.Reactions)
                    .Include(p => p.Author!),
            disableTracking: false) ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Post), request.PostId));

        post.AddComment(postComment);

        _postRepository.Update(post);
        await unitOfWork.SaveChangesAsync();

        return post.Adapt<PostDto>();
    }
}