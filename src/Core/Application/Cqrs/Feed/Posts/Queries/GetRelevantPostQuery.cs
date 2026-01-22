using Application.Dto.Feed;
using Application.Interfaces.Services;
using Mapster;
using MediatR;

namespace Application.Cqrs.Feed.Posts.Queries;

public class GetRelevantPostQuery : IRequest<List<PostDto>>;

public class GetRelevantPostQueryHandler(IPostService postService)
    : IRequestHandler<GetRelevantPostQuery, List<PostDto>>
{
    public async Task<List<PostDto>> Handle(GetRelevantPostQuery request, CancellationToken cancellationToken)
    {
        var posts = await postService.GetRelevantPostAsync(cancellationToken);

        return posts.Adapt<List<PostDto>>();
    }
}