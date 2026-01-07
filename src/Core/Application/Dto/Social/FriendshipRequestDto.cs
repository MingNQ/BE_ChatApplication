using Application.Common.Interfaces;
using Application.Dto.Persistence.Catalog.User;
using Domain.Common.Enums;

namespace Application.Dto.Social;

public class FriendshipRequestDto : IDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long FriendId { get; set; }
    public FriendshipEnum Status { get; set; }
    public SortUserInfo? User { get; set; }
    public SortUserInfo? Friend { get; set; }
}