using Application.Common.UnitOfWork;
using Domain.Entities.Social;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Social;

public class PresenceCleanupJob(ILogger<PresenceCleanupJob> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
    private static readonly TimeSpan OfflineThreshold = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan ScanInterval = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupAsync();
            }
            catch (Exception ex)
            {
                logger.LogError("Clean up error: {error}", ex.Message);
            }

            await Task.Delay(ScanInterval, stoppingToken);
        }
    }

    private async Task CleanupAsync()
    {
        var now = DateTime.UtcNow;

        foreach (var (userId, presence) in InMemoryPresenceStore.All)
        {
            if (now - presence.LastActiveAt < OfflineThreshold)
                continue;

            lock (presence)
            {
                // double check inside lock
                if (now - presence.LastActiveAt < OfflineThreshold)
                    continue;

                InMemoryPresenceStore.TryRemove(userId, out _);
            }

            await PersistLastSeen(userId, presence.LastActiveAt);
        }
    }

    private async Task PersistLastSeen(long userId, DateTimeOffset lastActiveAt)
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userPresenceSnapshotRepository = unitOfWork.GetRepository<UserPresenceSnapshot>();

        var userPresenceSnapshot = await userPresenceSnapshotRepository.GetFirstOrDefaultAsync(
            predicate: x => x.UserId == userId,
            disableTracking: false);

        if (userPresenceSnapshot == null)
        {
            logger.LogWarning(
                "UserPresenceSnapshot not found for UserId {UserId}",
                userId
            );
            return;
        }

        userPresenceSnapshot.Update(userPresenceSnapshot.Status, lastActiveAt);

        userPresenceSnapshotRepository.Update(userPresenceSnapshot);
        await unitOfWork.SaveChangesAsync();
    }
}