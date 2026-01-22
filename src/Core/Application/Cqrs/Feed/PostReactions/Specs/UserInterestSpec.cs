using Ardalis.Specification;
using Domain.Entities.Feed;

namespace Application.Cqrs.Feed.PostReactions.Specs;

public class UserInterestSpec : Specification<Post>
{
    public UserInterestSpec(long userId)
    {
        Query.Where(x => x.Reactions.Any(r => r.UserId == userId));

        Query.Include(x => x.Reactions);

        Query.Include(x => x.Comments);
    }
}