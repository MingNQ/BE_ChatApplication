using Application.Common.Exceptions;
using Application.Common.Persistence;
using Application.Cqrs.Feed.Posts.Specs;
using Application.Dto.Feed;
using Domain.Entities.Feed;
using MediatR;
using Shared.Constants;

namespace Application.Cqrs.Feed.Posts.Queries;

public class GetPostByIdQuery : IRequest<PostDto>
{
    public long Id { get; set; }
}

public class GetPostByIdQueryHandler(IReadRepository<Post> postRepository)
    : IRequestHandler<GetPostByIdQuery, PostDto>
{
    public async Task<PostDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new PostByIdSpec(request.Id);
        var post = await postRepository.FirstOrDefaultAsync(spec, cancellationToken)
            ?? throw new NotFoundException(MessageCommon.SetEntityNotFound(nameof(Post), request.Id));

        return post;
    }
}