using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Feed.Posts.Specs;
using Application.Cqrs.Social.Friendships.Specs;
using Application.Interfaces.Services;
using Domain.Common.Enums;
using Domain.Entities.Feed;
using Domain.Entities.Social;

namespace Infrastructure.Services.Feed;

public class PostService(
    ICurrentUser currentUser,
    IReadRepository<FriendshipRequest> friendshipRequest,
    IReadRepository<Post> postRepository) : IPostService
{
    public async Task<List<Post>> GetRelevantPostAsync(CancellationToken cancellationToken)
    {
        var friends = await friendshipRequest.ListAsync(new FriendsSpec(currentUser.UserId), cancellationToken);
        var posts = await postRepository.ListAsync(new PostsSpec(), cancellationToken);

        var friendIds = friends
            .SelectMany(f => new[] { f.UserId, f.FriendId })
            .Where(id => id != currentUser.UserId)
            .ToHashSet();

        var feedPosts = posts
            .Where(p => p.AuthorId == currentUser.UserId
                || friendIds.Contains(p.AuthorId) && p.Visibility != PostVisibilityEnum.Private).ToList();

        return feedPosts;
    }
}