using Domain.Entities.Feed;

namespace Application.Interfaces.Services;

public interface IPostService
{
    Task<List<Post>> GetRelevantPostAsync(CancellationToken cancellationToken);
}