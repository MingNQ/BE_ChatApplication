using Application.Common.Interfaces;
using Application.Dto.Persistence.Catalog.User;
using Application.Dto.Social;
using Application.Interfaces.Services;
using Mapster;
using MediatR;

namespace Application.Cqrs.Social.Presences.Queries;

public class GetPresencesQuery : IRequest<List<UserPresenceDto>>;

public class GetPresencesQueryHandler(
    IPresenceService presenceService,
    IFriendshipService friendshipService,
    ICurrentUser currentUser)
    : IRequestHandler<GetPresencesQuery, List<UserPresenceDto>>
{
    public async Task<List<UserPresenceDto>> Handle(GetPresencesQuery request, CancellationToken cancellationToken)
    {
        var friendships = await friendshipService.GetFriendsAsync(currentUser.UserId, cancellationToken).ConfigureAwait(false);
        if (friendships == null || friendships.Count == 0)
        {
            return [];
        }

        var friendIds = friendships
            .Select(f => f.Id)
            .Where(id => id != currentUser.UserId)
            .ToHashSet();

        if (friendIds.Count == 0)
        {
            return friendships.Adapt<List<UserPresenceDto>>();
        }

        var presences = await presenceService.GetPresencesAsync(friendIds).ConfigureAwait(false);
        if (presences == null || presences.Count == 0)
        {
            return friendships.Adapt<List<UserPresenceDto>>();
        }

        var presenceMap = presences.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Adapt<PresenceDto>());

        var friendDtos = friendships.Adapt<List<UserPresenceDto>>();
        foreach (var friend in friendDtos)
        {
            if (presenceMap.TryGetValue(friend.Id, out var presence))
            {
                friend.Presence = presence;
            }
        }

        return friendDtos;
    }
}