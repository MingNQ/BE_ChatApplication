using Application.Dto.Feed;
using Ardalis.Specification;
using Domain.Entities.Feed;

namespace Application.Cqrs.Feed.Posts.Specs;

public class PostByUserIdSpec : Specification<Post, PostDto>
{
    public PostByUserIdSpec(long userId)
    {
        Query.Where(x => x.AuthorId == userId);

        Query.Include(x => x.Attachments);
        Query.Include(x => x.Reactions);
        Query.Include(x => x.Comments);

        Query.OrderBy(x => x.CreatedOn);
    }
}