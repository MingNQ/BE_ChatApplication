using Application.Common.Services;
using Application.Dto.Feed;
using Application.Interfaces.Services;
using Mapster;
using MediatR;

namespace Application.Cqrs.Feed.Posts.Queries;

public class GetRelevantPostQuery : IRequest<List<PostDto>>;

public class GetRelevantPostQueryHandler(
    IPostService postService,
    IFilePathService filePathService)
    : IRequestHandler<GetRelevantPostQuery, List<PostDto>>
{
    public async Task<List<PostDto>> Handle(GetRelevantPostQuery request, CancellationToken cancellationToken)
    {
        var posts = await postService.GetRelevantPostAsync(cancellationToken);

        var postDtos = posts.Adapt<List<PostDto>>();

        foreach (var postDto in postDtos)
        {
            if (postDto.Attachments.Count != 0)
            {
                filePathService.BindFullPaths(postDto.Attachments);
            }
        }

        return postDtos;
    }
}