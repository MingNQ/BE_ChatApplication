using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.Cqrs.Feed.Posts.Specs;
using Application.Cqrs.Social.Friendships.Specs;
using Application.Interfaces.Services;
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
        //var interests = await postRepository.ListAsync(new UserInterestSpec(currentUser.UserId), cancellationToken);

        var friendIds = friends.Select(friend => new
        {
            friend.UserId,
            friend.FriendId
        }).ToList();

        var feedPosts = posts.Where(p => friendIds.Any(f => f.UserId == p.AuthorId || f.FriendId == p.AuthorId)).ToList();

        return feedPosts;
    }
}