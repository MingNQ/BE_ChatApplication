using Ardalis.Specification;
using Domain.Entities.Feed;

namespace Application.Cqrs.Feed.PostReactions.Specs;

public class MyReactionByPostIdSpec : Specification<Post>
{
    public MyReactionByPostIdSpec(long postId)
    {
        Query.Where(x => x.Id == postId);

        Query.Include(x => x.Reactions);
    }
}