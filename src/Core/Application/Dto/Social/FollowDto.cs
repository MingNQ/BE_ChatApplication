using Application.Common.Interfaces;
using Application.Dto.Persistence.Catalog.User;

namespace Application.Dto.Social;

public class FollowDto : IDto
{
    public long Id { get; set; }
    public long FollowerId { get; set; }
    public long FollowingId { get; set; }
    public SortUserInfo? Follower { get; set; }
    public SortUserInfo? Following { get; set; }
}