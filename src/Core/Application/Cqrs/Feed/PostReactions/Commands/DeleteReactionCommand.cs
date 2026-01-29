using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Domain.Entities.Feed;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using System.Text.Json.Serialization;

namespace Application.Cqrs.Feed.PostReactions.Commands;

public class DeleteReactionCommand : IRequest<bool>
{
    [JsonIgnore]
    public long Id { get; private set; }

    public long PostId { get; set; }

    public void SetId(long id)
    {
        Id = id;
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
            include: x => x.Include(p => p.Comments)
                        .ThenInclude(c => c.User)
                    .Include(p => p.Reactions)
                    .Include(p => p.Author!)
                    .Include(p => p.Attachments)
                        .ThenInclude(a => a.Attachment!),
            disableTracking: false)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Post), request.PostId));

        post.RemoveReaction(post.Reactions.FirstOrDefault(x => x.Id == request.Id)!);

        _postRepository.Update(post);
        await unitOfWork.SaveChangesAsync();

        return true;
    }
}