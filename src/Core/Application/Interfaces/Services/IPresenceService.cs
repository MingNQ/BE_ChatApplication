using Domain.Entities.Social;

namespace Application.Interfaces.Services;

public interface IPresenceService
{
    Task UserConnectedAsync(long userId, string connectionId);

    Task<bool> UserDisconnectedAsync(long userId, string connectionId);

    Task HeartBeatAsync(long userId);

    Task<UserPresenceSnapshot> GetPresenceAsync(long userId);

    Task<IReadOnlyDictionary<long, UserPresenceSnapshot>> GetPresencesAsync(
        IEnumerable<long> userIds);
}