using Application.Common.Models;
using Application.Common.Persistence;
using Application.Common.Services;
using Application.Cqrs.Feed.Posts.Params;
using Application.Cqrs.Feed.Posts.Specs;
using Application.Dto.Feed;
using Domain.Entities.Feed;
using MediatR;

namespace Application.Cqrs.Feed.Posts.Queries;

public class GetPostQuery : SearchPostParam, IRequest<PaginationResponse<PostDto>>;

public class GetPostQueryHandler(
    IReadRepository<Post> postRepository,
    IPaginationService paginationService)
    : IRequestHandler<GetPostQuery, PaginationResponse<PostDto>>
{
    public async Task<PaginationResponse<PostDto>> Handle(GetPostQuery request, CancellationToken cancellationToken)
    {
        var spec = new SearchPostSpec(request);
        var posts = await paginationService.PaginatedListAsync(
            postRepository,
            spec,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
        return posts;
    }
}