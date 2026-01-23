using Application.Common.UnitOfWork;
using Application.Interfaces.Services;
using Domain.Common.Enums;
using Domain.Entities.Social;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Infrastructure.Services.Social;

public class InMemoryPresence
{
    public HashSet<string> Connections { get; } = new();
    public DateTimeOffset LastActiveAt { get; set; }
    public UserPresenceStatusEnum Status { get; set; } = UserPresenceStatusEnum.Online;
}

public static class InMemoryPresenceStore
{
    private static readonly ConcurrentDictionary<long, InMemoryPresence> _store
        = new();

    public static IReadOnlyDictionary<long, InMemoryPresence> All => _store;

    public static InMemoryPresence GetOrAdd(long userId)
        => _store.GetOrAdd(userId, _ => new InMemoryPresence());

    public static bool TryGet(long userId, out InMemoryPresence presence)
        => _store.TryGetValue(userId, out presence!);

    public static bool TryRemove(long userId, out InMemoryPresence presence)
        => _store.TryRemove(userId, out presence!);
}

public class InMemoryPresenceService(ILogger<InMemoryPresenceService> logger, IServiceScopeFactory scopeFactory) : IPresenceService
{
    public Task UserConnectedAsync(long userId, string connectionId)
    {
        var presence = InMemoryPresenceStore.GetOrAdd(userId);

        lock (presence)
        {
            presence.Connections.Add(connectionId);
            presence.LastActiveAt = DateTime.UtcNow;
            presence.Status = UserPresenceStatusEnum.Online;
        }

        return Task.CompletedTask;
    }

    public Task<bool> UserDisconnectedAsync(long userId, string connectionId)
    {
        if (!InMemoryPresenceStore.TryGet(userId, out var presence))
            return Task.FromResult(false);

        lock (presence)
        {
            presence.Connections.Remove(connectionId);

            if (presence.Connections.Count == 0)
            {
                InMemoryPresenceStore.TryRemove(userId, out _);
                PersistLastSeen(userId, presence.LastActiveAt);
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    public Task HeartBeatAsync(long userId)
    {
        if (InMemoryPresenceStore.TryGet(userId, out var presence))
            presence.LastActiveAt = DateTime.UtcNow;

        return Task.CompletedTask;
    }

    public Task<UserPresenceSnapshot> GetPresenceAsync(long userId)
    {
        if (InMemoryPresenceStore.TryGet(userId, out var p))
        {
            return Task.FromResult(UserPresenceSnapshot.Create(userId, p.Status, p.LastActiveAt));
        }

        return Task.FromResult(UserPresenceSnapshot.Create(userId, UserPresenceStatusEnum.Offline, DateTimeOffset.UtcNow));
    }

    public async Task<IReadOnlyDictionary<long, UserPresenceSnapshot>> GetPresencesAsync(IEnumerable<long> userIds)
    {
        var result = new Dictionary<long, UserPresenceSnapshot>();

        foreach (var id in userIds)
        {
            result[id] = await GetPresenceAsync(id);
        }

        return result;
    }

    private void PersistLastSeen(long userId, DateTimeOffset lastSeen)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var repo = unitOfWork.GetRepository<UserPresenceSnapshot>();

                var snapshot = await repo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == userId,
                    disableTracking: false);

                if (snapshot == null)
                {
                    logger.LogError("");
                    return;
                }

                snapshot.Update(snapshot.Status, lastSeen);
                repo.Update(snapshot);

                await unitOfWork.SaveChangesAsync();
            }
            catch
            {
                logger.LogError("Something went wrong while update UserPresence");
            }
        });
    }
}