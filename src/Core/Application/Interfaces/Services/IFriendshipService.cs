using Domain.Entities.Identity;

namespace Application.Interfaces.Services;

public interface IFriendshipService
{
    Task<List<User>> GetFriendsAsync(long userId, CancellationToken cancellationToken);
}