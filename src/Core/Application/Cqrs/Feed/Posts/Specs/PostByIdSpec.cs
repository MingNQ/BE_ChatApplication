using Application.Dto.Feed;
using Ardalis.Specification;
using Domain.Entities.Feed;

namespace Application.Cqrs.Feed.Posts.Specs;

public class PostByIdSpec : Specification<Post, PostDto>
{
    public PostByIdSpec(long id)
    {
        Query.Where(post => post.Id == id);
        Query.Include(post => post.Attachments);
        Query.Include(post => post.Reactions);
        Query.Include(post => post.Comments);
    }
}