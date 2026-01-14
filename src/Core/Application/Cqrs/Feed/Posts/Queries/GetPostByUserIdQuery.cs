using Application.Common.Persistence;
using Application.Cqrs.Feed.Posts.Specs;
using Application.Dto.Feed;
using Domain.Entities.Feed;
using MediatR;

namespace Application.Cqrs.Feed.Posts.Queries;

public class GetPostByUserIdQuery : IRequest<List<PostDto>>
{
    public long UserId { get; set; }
}

public class GetPostByUserIdQueryHandler(IReadRepository<Post> postRepository)
    : IRequestHandler<GetPostByUserIdQuery, List<PostDto>>
{
    public async Task<List<PostDto>> Handle(GetPostByUserIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new PostByUserIdSpec(request.UserId);
        var posts = await postRepository.ListAsync(spec, cancellationToken);

        return posts;
    }
}