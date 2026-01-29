using Application.Common.Persistence;
using Application.Common.Services;
using Application.Cqrs.Feed.Posts.Specs;
using Application.Dto.Feed;
using Domain.Entities.Feed;
using MediatR;

namespace Application.Cqrs.Feed.Posts.Queries;

public class GetPostByUserIdQuery : IRequest<List<PostDto>>
{
    public long UserId { get; set; }
}

public class GetPostByUserIdQueryHandler(
    IReadRepository<Post> postRepository,
    IFilePathService filePathService)
    : IRequestHandler<GetPostByUserIdQuery, List<PostDto>>
{
    public async Task<List<PostDto>> Handle(GetPostByUserIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new PostByUserIdSpec(request.UserId);
        var posts = await postRepository.ListAsync(spec, cancellationToken);

        foreach (var post in posts)
        {
            if (post.Attachments.Count != 0)
            {
                var attachments = post.Attachments.Select(a => a.Attachment!).ToList();
                filePathService.BindFullPaths(attachments);
            }
        }

        return posts;
    }
}