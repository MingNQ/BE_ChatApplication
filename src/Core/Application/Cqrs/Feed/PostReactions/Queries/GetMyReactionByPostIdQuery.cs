using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Feed.PostReactions.Specs;
using Application.Dto.Feed;
using Domain.Entities.Feed;
using Mapster;
using MediatR;
using Shared.Constants;

namespace Application.Cqrs.Feed.PostReactions.Queries;

public class GetMyReactionByPostIdQuery : IRequest<PostReactionDto>
{
    public long Id { get; set; }
}

public class GetMyReactionByPostIdQueryHandler(
    IReadRepository<Post> postRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetMyReactionByPostIdQuery, PostReactionDto>
{
    public async Task<PostReactionDto> Handle(GetMyReactionByPostIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new MyReactionByPostIdSpec(request.Id);
        var post = await postRepository.FirstOrDefaultAsync(spec, cancellationToken)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Post), request.Id));
        var myReaction = post.Reactions.FirstOrDefault(x => x.UserId == currentUser.UserId);

        return myReaction.Adapt<PostReactionDto>();
    }
}