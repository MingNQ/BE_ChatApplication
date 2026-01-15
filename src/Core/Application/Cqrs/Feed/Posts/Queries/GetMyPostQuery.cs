using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Feed.Posts.Specs;
using Application.Dto.Feed;
using Domain.Entities.Feed;
using MediatR;

namespace Application.Cqrs.Feed.Posts.Queries;

public class GetMyPostQuery : IRequest<List<PostDto>>;

public class GetMyPostQueryHandler(
    IReadRepository<Post> postRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetMyPostQuery, List<PostDto>>
{
    public async Task<List<PostDto>> Handle(GetMyPostQuery request, CancellationToken cancellationToken)
    {
        var spec = new PostByUserIdSpec(currentUser.UserId);
        var posts = await postRepository.ListAsync(spec, cancellationToken);
        return posts;
    }
}