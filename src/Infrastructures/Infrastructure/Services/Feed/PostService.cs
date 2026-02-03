using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Feed.Posts.Specs;
using Application.Interfaces.Services;
using Domain.Common.Enums;
using Domain.Entities.Feed;

namespace Infrastructure.Services.Feed;

public class PostService(
    ICurrentUser currentUser,
    IReadRepository<Post> postRepository,
    IFriendshipService friendshipService) : IPostService
{
    public async Task<List<Post>> GetRelevantPostAsync(CancellationToken cancellationToken)
    {
        var friends = await friendshipService.GetFriendsAsync(currentUser.UserId, cancellationToken);
        var posts = await postRepository.ListAsync(new PostsSpec(), cancellationToken);

        var friendIds = friends
            .Select(f => f.Id)
            .Where(id => id != currentUser.UserId)
            .ToHashSet();

        var feedPosts = posts
            .Where(p => p.AuthorId == currentUser.UserId
                || friendIds.Contains(p.AuthorId) && p.Visibility != PostVisibilityEnum.Private).ToList();

        return feedPosts;
    }
}