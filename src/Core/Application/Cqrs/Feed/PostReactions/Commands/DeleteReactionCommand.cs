using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Domain.Entities.Feed;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;

namespace Application.Cqrs.Feed.PostReactions.Commands;

public class DeleteReactionCommand : IRequest<bool>
{
    public long Id { get; set; }
    public long PostId { get; set; }

    public DeleteReactionCommand(long id, long postId)
    {
        Id = id;
        PostId = postId;
    }
}

public class DeleteReactionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteReactionCommand, bool>
{
    private readonly IWriteRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();

    public async Task<bool> Handle(DeleteReactionCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == request.PostId,
            include: x => x.Include(p => p.Reactions),
            disableTracking: false)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Post), request.PostId));

        post.RemoveReaction(post.Reactions.FirstOrDefault(x => x.Id == request.Id)!);

        _postRepository.Update(post);
        await unitOfWork.SaveChangesAsync();

        return true;
    }
}