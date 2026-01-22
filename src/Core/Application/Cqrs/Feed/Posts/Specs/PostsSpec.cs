using Ardalis.Specification;
using Domain.Entities.Feed;

namespace Application.Cqrs.Feed.Posts.Specs;

public class PostsSpec : Specification<Post>
{
    public PostsSpec()
    {
        Query.Include(x => x.Attachments);
        Query.Include(x => x.Reactions);
        Query.Include(x => x.Comments);

        Query.OrderByDescending(x => x.CreatedOn);
    }
}