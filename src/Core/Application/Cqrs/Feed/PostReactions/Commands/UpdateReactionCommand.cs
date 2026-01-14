using Application.Common.Exceptions;
using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Dto.Feed;
using Domain.Entities.Feed;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;

namespace Application.Cqrs.Feed.PostReactions.Commands;

public class UpdateReactionCommand : BaseReactionCommand, IRequest<PostDto>
{
    public long Id { get; set; }
}

public class UpdateReactionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateReactionCommand, PostDto>
{
    private readonly IWriteRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();

    public async Task<PostDto> Handle(UpdateReactionCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == request.PostId,
            include: x => x.Include(p => p.Reactions),
            disableTracking: false)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Post), request.PostId));

        foreach (var reaction in post.Reactions)
        {
            if (reaction.Id == request.PostId)
            {
                reaction.Update(request.Type);
            }
        }

        _postRepository.Update(post);
        await unitOfWork.SaveChangesAsync();

        return post.Adapt<PostDto>();
    }
}